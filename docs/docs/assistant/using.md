# 使用SemanticServer服务

目前`SemanticServer`使用redis的消息订阅实现通信
所以我们实现需要定义Redis连接字符串，并且需要和`Chat.Service`保持一致即可，请注意还需要配置OpenAI相关的配置文件。

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "OpenAI": {
    "Key": "",
    "Endpoint": "",
    "Model": "gpt-3.5-turbo"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=postgre;Port=5432;Database=Chat;Username=token;Password=dd666666;",
    "Redis": "redis"
  },
  "ChatServiceOptions": {
    "Endpoint": "http://chat-api/"
  }
}

```

配置完成以后不需要其他操作，只需要将当前服务部署到外网服务器`能访问OpenAI服务的服务器`即可。
