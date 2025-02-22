# 网关的设计

## 网关的作用

在我们的项目中，网关承担了比传统网关更为复杂的职责。它不仅负责请求的转发，还实现了服务发现功能，能够自动注册节点到网关。通过这种机制，网关能够自动发现所有的服务节点，并根据请求的路径和方法，将请求路由到相应的节点。

在此基础上，网关还遵循了一套固定的协议。

### 网关服务发现的实现原理

在节点服务中，我们需要依赖 `EarthChat.Gateway.Sdk` 这个 SDK，它提供了节点注册到网关的功能。我们只需提供以下配置项：

- **Address**：网关的地址，可以是内网 IP 或 Docker Compose 名称。节点必须注册到一个网关，因此需要填写网关的地址。
- **Service**：当前节点的名称，这是必填项。节点名称同时代表了该节点的访问路径。例如，`auth` 节点的访问路径为 `/api/auth`，`chat` 节点的访问路径为 `/api/chat`。网关将根据这个信息进行请求路由。
- **Token**：网关的访问令牌，必填项。它用于验证节点的合法性，如果令牌不正确，网关将拒绝节点的注册。

在多个节点同时注册的情况下，如果服务名称一致，网关会自动进行负载均衡。

示例配置如下：
```json
  "Gateway": {
    "Service": "auth",
    "Address": "https://localhost:58101",
    "Token": "token010426"
  }
```

其工作机制如下：节点会首先建立一个 HTTP/2（h2）连接，用于在网关和节点之间传递信息。当网关接收到请求时，会通过这个 h2 连接向节点发送消息。节点接收到消息后，会自动创建一个隧道，这个隧道也是一个 h2 连接，并与 Yarp 的 HttpClient 的 RequestCallback 进行绑定。Yarp 内部维护了一个 HttpClient 连接池，因此我们不需要担心每个请求都会创建新的隧道，因为创建隧道的成本较高，我们会尽可能地复用现有隧道。

## 业务核心网关职责

网关的核心职责包括但不限于：

- **负载均衡**：合理分配请求到各个服务节点。
- **服务发现**：自动发现并注册服务节点。
- **接口鉴权**：验证请求的合法性，确保安全性。
- **限流**：控制请求的速率，保护服务不被过载。
- **日志管理**：
    - 请求日志：记录每个请求的详细信息。
    - 错误日志：记录系统错误和异常。
- **监控**：
    - 请求监控：实时监控请求的状态和性能。
- **Scalar管理**：收集集群中所有节点服务的指标数据，并统一提供。
- **服务节点监控**：
    - 服务状态：监控各个服务节点的运行状态。
    - 服务负载：评估节点的负载情况。
    - 服务监控：实时跟踪服务的健康状况。