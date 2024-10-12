namespace alert_service.RabbitMQManagement
{
   using System.Text;
   using RabbitMQ.Client;
   using alert_service.DTOs;
   using System.Text.Json;
   // Nécessaires pour les tests
   // NON UTLISES POUR L'INSTANT (peuvent servir de base si besoin de producers
   public class RabbitMQPublisher : IDisposable
   {
      private readonly IModel _channel;
      private readonly IConnection _connection;
      private bool _disposed = false;

      private string _queue { get; set; }

      public RabbitMQPublisher(string queue)
      {
         _queue = queue;
         /* Creation de la connexion socket vers RabbitMQ */
         ConnectionFactory factory = new ConnectionFactory
         {
            HostName = "localhost",
            UserName = "user",
            Password = "password",
         };
         _connection = factory.CreateConnection();
         _channel = _connection.CreateModel();

         /* La connexion etant établi à ce niveau, on déclare la queue "alert-ad-update" */
         _channel.QueueDeclare(queue: _queue,
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);
      }

      /// <summary>
      /// Dans le contexte de notre app, cette méthode serait déclenchée par un consumer quand une annonce est mise à jour.
      /// Conversion du message en binaire pour permettre une compatibilité 
      /// et une interopérabilité entre le producer et le consumer, ça augmente aussi les perfs !
      /// Puis publication du binaire dans la queue "alert-ad-update"<br>
      /// La RoutingKey porte le nom de la queue dans une utilisation classique. Elle permet de publier sur le nom de la queue.
      /// Dans une utilisation avancée (notamment avec un exchange), elle peut permettre de distribuer des messages a d'autres 
      /// queues génériques comme "create" pour les crud (pas sur qu'on en ai besoin)
      /// </summary>
      public void SendMessageToQueueSubAndCreate(SubscribeDto subscribeDto)
      {
         string message = JsonSerializer.Serialize(subscribeDto);
         var body = Encoding.UTF8.GetBytes(message);

         _channel.BasicPublish(exchange: string.Empty,
                              routingKey: _queue,
                              basicProperties: null,
                              body: body);
         Console.WriteLine($" [x] Sent: {subscribeDto}");
      }
      public void SendMessageToQueueUnsub(UnsubscribeDto unsubscribeDto)
      {
         string message = JsonSerializer.Serialize(unsubscribeDto);
         var body = Encoding.UTF8.GetBytes(message);

         _channel.BasicPublish(exchange: string.Empty,
                              routingKey: _queue,
                              basicProperties: null,
                              body: body);
         Console.WriteLine($" [x] Sent: {unsubscribeDto}");
      }
      public void SendMessageToQueueAlertUpdated(AlertUpdatedDto alertUpdatedDto)
      {
         string message = JsonSerializer.Serialize(alertUpdatedDto);
         var body = Encoding.UTF8.GetBytes(message);

         _channel.BasicPublish(exchange: string.Empty,
                              routingKey: _queue,
                              basicProperties: null,
                              body: body);
         Console.WriteLine($" [x] Sent: {alertUpdatedDto}");
      }

      /// <summary>
      /// Cette méthode est l'implémentation de l'interface IDisposable. 
      /// Elle est appelée lorsque l'objet n'est plus nécessaire et que les ressources qu'il utilise doivent être libérées.
      /// <br /> Libère les ressources en appelant la méthode virtuelle Dispose(true) qui implémente les méthode Dispose sur les objets 
      /// _channel et _connection. Cela permet de libérer les ressources associées à ces objets.
      /// </summary>
      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }

      protected virtual void Dispose(bool disposing)
      {
         if (!_disposed && disposing)
         {
            _channel.Dispose();
            _connection.Dispose();
            _disposed = true;
         }
      }
   }
}