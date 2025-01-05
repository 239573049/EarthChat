var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.EarthChat_Gateway>("earthchat-gateway");

builder.AddProject<Projects.EarthChat_InstantMessage_Service>("instantmessage");

builder.AddProject<Projects.EarthChat_AuthServer>("auth");

builder.Build().Run();
