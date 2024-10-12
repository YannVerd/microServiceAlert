using System.Text.Json.Serialization;
using alert_service.Repositories;
using alert_service.RabbitMQManagement;
using alert_service.Services;
using alert_service.DTOs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<EntityDetailsRepository>();
builder.Services.AddScoped<AlertRepository>();
builder.Services.AddScoped<NotifyRepository>();
builder.Services.AddScoped<EventService>();

builder.Services.AddDbContext<DBContext>();

builder.Services.AddControllers()
.AddJsonOptions(options =>
    {
        // Gestion des références circulaires afin d'éviter les problèmes de sérialisation JSON
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHsts();
}

// var publisher = new RabbitMQPublisher("user-subscribe-alert");
// publisher.SendMessageToQueueSubAndCreate(new SubscribeDto
// {
//     IdEntityDetails = 1,
//     IdDMUser = 1,
//     IdDMEntity = 1,
//     Name = "Une maison en bord de mer",
//     Type = "annonce"
// });
// var publisher2 = new RabbitMQPublisher("user-unsubscribe-alert");
// publisher2.SendMessageToQueueUnsub(new UnsubscribeDto
// {
//     IdDMUser = 1,
//     IdDMEntity = 1
// });
// var publisher3 = new RabbitMQPublisher("alert-updated");
// publisher3.SendMessageToQueueAlertUpdated(new AlertUpdatedDto
// {
//     IdAlert = 1
// });


RabbitMQReceiver receiverSub = new RabbitMQReceiver("user-subscribe-alert");
receiverSub.getMessageToQueueAdUpdate();
RabbitMQReceiver receiverUnsub = new RabbitMQReceiver("user-unsubscribe-alert");
receiverUnsub.getMessageToQueueAdUpdate();
RabbitMQReceiver receiverSendNotif = new RabbitMQReceiver("alert-updated");
receiverSendNotif.getMessageToQueueAdUpdate();

app.UseAuthentication();
app.UseAuthorization();

app.UseRouting();
app.MapControllers();

app.Run();


