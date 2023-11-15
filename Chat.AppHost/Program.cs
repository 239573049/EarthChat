var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Chat_Service>("chat.service");

builder.Build().Run();
