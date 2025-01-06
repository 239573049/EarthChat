var builder = DistributedApplication.CreateBuilder(args);

var gateway = builder.AddProject<Projects.EarthChat_Gateway>("earthchat-gateway");

builder.AddProject<Projects.EarthChat_InstantMessage_Service>("instantmessage")
	.WithReference(gateway);

builder.AddProject<Projects.EarthChat_AuthServer>("auth")
	.WithReference(gateway);

builder.Build().Run();
