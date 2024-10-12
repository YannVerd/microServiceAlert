namespace alert_service.RabbitMQManagement
{
    using System.Text;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using alert_service.DTOs;
    using System.Text.Json;
    using System.Net.Http;

    public class RabbitMQReceiver : IDisposable
    {
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private string _message = "";
        private List<ReturnUserDto> _returnUserDto = [];
        private string _queue { get; set; }
        private bool _disposed = false;
        private HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Créé la connexion au conteneur rabbitMq
        /// Le Switch/Case permet de rediriger l'event vers la route correspondant à ce que l'on souhaite :
        /// Ajouter un case permet d'ajouter de nouveau event (à condition d'avoir créé methode et contrôleur correspondant)
        /// 
        /// </summary>
        public RabbitMQReceiver(string queue)
        {
            _queue = queue;

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "user",
                Password = "password",
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queue,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);
        }

        public void getMessageToQueueAdUpdate()
        {
            Console.WriteLine(" [*] Waiting for messages.");

            EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                Console.WriteLine($" => Nom de la RoutingKey: {ea.RoutingKey}");
                //formatage du message
                _message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Received: {_message}");

                switch (_queue)
                {
                    case "user-subscribe-alert":
                        try
                        {
                            StringContent? content = new StringContent(_message, Encoding.UTF8, "application/json");
                            await _httpClient.PostAsync("http://localhost:5186/alert/managmentEvent/createandsubscribealert", content);
                            Console.WriteLine("Envoyé au controleur createandsub");
                        }
                        catch (HttpRequestException e)
                        {
                            Console.WriteLine("\nException Caught send user-subscribe-alert-event!");
                            Console.WriteLine("Message :{0} ", e.Message);
                        }
                        break;

                    case "user-unsubscribe-alert":
                        try
                        {
                            StringContent? content2 = new StringContent(_message, Encoding.UTF8, "application/json");
                            await _httpClient.PostAsync("http://localhost:5186/alert/managmentevent/unsubscribealert", content2);
                            Console.WriteLine("Envoyé au controleur unsub");
                        }
                        catch (HttpRequestException e)
                        {
                            Console.WriteLine("\nException Caught send user-unsubscribe-alert-event!");
                            Console.WriteLine("Message :{0} ", e.Message);
                        }

                        break;

                    case "send-notification":
                        AlertUpdatedDto? alertUpdatedDto = JsonSerializer.Deserialize<AlertUpdatedDto>(_message);
                        try
                        {
                            if (alertUpdatedDto != null)
                            {
                                HttpResponseMessage? response = await _httpClient.GetAsync($"http://localhost:5186/alert/managmentevent/alertupdated/{alertUpdatedDto.IdDMEntity.ToString()}");
                                response.EnsureSuccessStatusCode();

                                string responseBody = await response.Content.ReadAsStringAsync();
                                Console.WriteLine(responseBody);
                                // implémentation de la methode de retour de la liste de  diffusion ici 
                            }
                        }
                        catch (HttpRequestException e)
                        {
                            Console.WriteLine("\nException Caught send notification event!");
                            Console.WriteLine("Message :{0} ", e.Message);
                        }
                        break;

                    default:
                        break;
                }
            };

            _channel.BasicConsume(queue: _queue,
                                    autoAck: true,
                                    consumer: consumer);
        }

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