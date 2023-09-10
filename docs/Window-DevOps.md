# 在Window中单机部署

## 使用SqlIte数据库部署

准备环境

- Redis [安装Redis](#安装Redis)
- .NET 7 [安装.NET7](https://dotnet.microsoft.com/zh-cn/download/dotnet/thank-you/sdk-7.0.400-windows-x64-installer)

使用根目录下的`window-build.bat`构建前端项目和后端项目，

执行前提安装`node 16`和`.NET 7`，

当执行完成以后在根目录下存在一个`DevOps/`目录，

存在`nginx`和`service`俩个目录，

- 第一步进入`nginx`，然后执行nginx.exe nginx默认端口为`8880`，如果需要修改请修改`DevOps/nginx/conf/nginx.conf`中配置的端口即可
- 第二步进入`DevOps/service`，然后执行`run.bat`，后端默认启动的端口`23348`，如果需要修改默认端口请修改`run.bat`配置的端口
- 第三步使用浏览器访问`http://localhost:8880/`



## 安装Redis

### Window下安装Redis

下载Redis安装包 [这里](https://code-token.oss-cn-beijing.aliyuncs.com/Redis-x64-5.0.14.1.msi)

安装流程全部使用默认安装，端口使用`6379`默认端口。

### Linux下安装Redis

