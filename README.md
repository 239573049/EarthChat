<p align="center">
    <a href="" target="_blank">
      <img src="./docs/static/img/docusaurus.png" width="280" />
    </a>
</p>

<h1 align="center">Earth Chat</h1>
<p align="center"><strong>一个支持百万用户的Chat系统！</strong></p>

<div align="center">
    <a href="#交流群"><img src="https://img.shields.io/badge/交流群-blue.svg?style=plasticr"></a>
    <a href="https://docs.chat.tokengo.top/docs/intro/"><img src="https://img.shields.io/badge/项目文档-green.svg?style=plasticr"></a>
    <a href="https://github.com/239573049/chat"><img src="https://img.shields.io/badge/github-项目地址-yellow.svg?style=plasticr"></a>
    <a href="https://gitee.com/hejiale010426/chat"><img src="https://img.shields.io/badge/码云-项目地址-orange.svg?style=plasticr"></a>
    <a href="https://github.com/239573049/chat/blob/master/LICENSE" target="_blank">
        <img alt="License: Apache-2.0" src="https://img.shields.io/badge/License-Apache--2.0-blue.svg">
    </a>
    <a href="https://github.com/239573049/chat" target="_blank">
        <img alt="License" src="https://img.shields.io/github/stars/239573049/chat">
    </a>
    <a href='https://gitee.com/hejiale010426/chat/stargazers'>
        <img src='https://gitee.com/hejiale010426/chat/badge/star.svg?theme=dark' alt='star'></img>
    </a>
</div>

## 项目介绍

基于SignalR实现的Chat聊天，支持横向扩展，以便支撑上万用户同时在线聊天。
采用DDD领域驱动设计模式，CQRS架构模式，分离读写模型，架构更清晰，并且维护成本更低。
![Alt text](image.png)

### 后端架构设计

- ASP.NET Core 7
- PostgreSQL
- Redis
- [MasaFramework](https://docs.masastack.com/framework/concepts/overview)
- DDD领域驱动设计模式 CQRS架构模式
- SignalR （实现对话并且使用redis支持横向扩展）

### 前端架构

- React
- [Semi UI](https://semi.design/zh-CN/start/getting-started)
- Axios
- Vite
- Avalonia

## 贡献

<a href="https://github.com/239573049/EarthChat/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=239573049/EarthChat" />
</a>

## 后端环境变量配置

当环境变量为空则读取配置文件的值

| 环境变量名称            | 环境变量值                                 |
| ----------------------- | ------------------------------------------ |
| REDIS_CONNECTION_STRING | Redis连接字符串                            |
| SQLTYPE                 | 数据库类型 `sqlite`|[`pgsql`|`postgresql`] |
| CONNECTION_STRING       | 数据库连接字符串                           |

## 交流群

![交流群](![输入图片说明](![输入图片说明](docs/static/img/ed1d8637a5ea540308d85523bd2a9f4.png)))
