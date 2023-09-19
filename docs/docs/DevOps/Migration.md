# 数据库迁移

默认使用的是`PostgreSQL`数据库

在`Debug`中运行项目会自动进行迁移，并且创建默认账号`admin`密码`123456`

需要注意的是默认创建的时候需要先在数据库执行以下命令，由于使用到了`hstore`的功能，但是可能默认数据库不存在`hstore`，所以需要先创建，然后在`Debug`模式下启动项目。

```sh
CREATE EXTENSION hstore;
```

