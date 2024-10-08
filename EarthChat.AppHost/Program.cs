var builder = DistributedApplication.CreateBuilder(args);

var instantMessage = builder.AddProject<Projects.EarthChat_InstantMessage_Service>("earthchat-instantmessage");

var gateway = builder.AddProject<Projects.EarthChat_Infrastructure_Gateway>("earthchat-gateway")
    .WithReference(instantMessage);

instantMessage
    .WithReference(gateway);


builder.Build().Run();
