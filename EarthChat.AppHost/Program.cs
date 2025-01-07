var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddConnectionString("postgres", "ConnectionStrings:Default");

var gateway = builder.AddProject<Projects.EarthChat_Gateway>("earthchat-gateway")
    .WithReference(postgres);

builder.AddProject<Projects.EarthChat_AuthServer_Host>("instantmessage")
    .WithReference(postgres)
    .WithReference(gateway);

builder.AddProject<Projects.EarthChat_AuthServer_Host>("auth")
    .WithReference(postgres)
    .WithReference(gateway);

builder.Build().Run();