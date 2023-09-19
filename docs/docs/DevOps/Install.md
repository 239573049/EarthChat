# Docker Compose 部署

## 创建`Docker`网络

```bash
docker network create chat
```

## 使用`Docker Compose`构建镜像

在根目录下执行

```bash
docker-compose build
```

## 推送镜像到`Docker Hub`

```bash
docker-compose push
```

## 启动容器

```bash
docker-compose up -d
```

## 停止容器

```bash
docker-compose down
```

