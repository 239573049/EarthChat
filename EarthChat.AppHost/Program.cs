var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddConnectionString("postgres", "ConnectionStrings:Default");
var rabbit = builder.AddConnectionString("rabbit", "RabbitMQ:ConnectionString");
var redis = builder.AddConnectionString("redis", "ConnectionStrings:Redis");

var gateway = builder.AddProject<Projects.EarthChat_Gateway>("earthchat-gateway")
    .WithReference(rabbit)
    .WithReference(redis)
    .WithReference(postgres);

builder.AddProject<Projects.EarthChat_AuthServer_Host>("instantmessage")
    .WithReference(postgres)
    .WithReference(rabbit)
    .WithReference(redis)
    .WithReference(gateway);

builder.AddProject<Projects.EarthChat_AuthServer_Host>("auth")
    .WithReference(postgres)
    .WithReference(rabbit)
    .WithReference(redis)
    .WithReference(gateway);

builder.Build().Run();