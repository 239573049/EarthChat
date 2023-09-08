set REDIS_CONNECTION_STRING = 127.0.0.1
set SQLTYPE=sqlite
set CONNECTION_STRING=Data Source=./Chat.db
dotnet Chat.Service.dll --urls="http://*:23348"