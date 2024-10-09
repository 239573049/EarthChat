using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var instantMessage = builder.AddProject<EarthChat_InstantMessage_Service>("earthchat-instantmessage");

var gateway = builder.AddProject<EarthChat_Gateway>("earthchat-gateway")
    .WithReference(instantMessage);

instantMessage
    .WithReference(gateway);

builder.AddProject<EarthChat_AuthServer>("earthchat-authserver")
    .WithReference(gateway);

builder.Build().Run();