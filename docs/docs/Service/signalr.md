# SignalR原理讲解

## SignalR是什么？

SignalR 是 Microsoft 开发的一个库，用于 ASP.NET 开发人员实现实时 web 功能。这意味着服务端代码可以实时地推送内容到连接的客户端，而不需要客户端定期请求或轮询服务器以获取新数据。SignalR 可以用于各种应用程序，如实时聊天、通知、实时数据更新等。

SignalR 提供了以下特点：

1. **抽象层的连接**：SignalR 提供了一种高级API，隐藏了底层实时通讯的复杂性。开发者不需要担心具体使用 WebSockets、Server-Sent Events、长轮询等，因为 SignalR 会根据客户端和服务器的能力自动选择最佳的通讯方式。

2. **连接管理**：自动处理连接、重连和断开连接的复杂性。

3. **组播**：可以广播消息到所有连接的客户端，或者只给特定的客户端或客户端组发送消息。

4. **扩展性**：支持可插拔的组件，允许开发者自定义或扩展其功能。

5. **跨平台**：除了在网页客户端上使用，还提供了客户端库支持各种平台，如 .NET、JavaScript、Java、Swift 和 Objective-C 等。

要使用 SignalR，开发者需要安装相应的 NuGet 包并按照文档中的指导进行配置和开发。

在近几年，SignalR 核心 (SignalR Core) 成为了主流，它是为 .NET Core 重新设计和实现的 SignalR 版本，提供了更好的性能和跨平台支持。

## SignalR MessagePack

### 什么是 MessagePack？

[MessagePack](https://msgpack.org/index.html)
是一种快速而紧凑的二进制序列化格式。 当担忧性能和带宽问题时，这很有用，因为它创建的消息比 [JSON](https://www.json.org/) 创建的小。 查看网络跟踪和日志时，二进制消息不可读取，除非这些字节是通过 MessagePack 分析器传递的。 SignalR 为 MessagePack 格式提供内置支持，并提供 API 供客户端和服务器使用。

### 在服务器上配置 MessagePack

若要在服务器上启用 MessagePack 中心协议，请在应用中安装 `Microsoft.AspNetCore.SignalR.Protocols.MessagePack` 包。 在 `Startup.ConfigureServices` 方法中，将 `AddMessagePackProtocol` 添加到 `AddSignalR` 调用以在服务器上启用 MessagePack 支持。

```csharp
services.AddSignalR()
    .AddMessagePackProtocol();
```

:::info 小知识

JSON 默认启用。 添加 MessagePack 可同时支持 JSON 和 MessagePack 客户端。

:::

当启用了`MessagePack`，客户端会发送协议消息和版本

```json
{"protocol":"messagepack","version":1}
```

后续会使用二进制传输，

![Alt text](./img/signalr-0001.png)

:::tip 小知识

MessagePack在序列化中对比json序列化性能更好，并且体积更小，所以用于作为消息传输再合适不过了，但它不适合作为可读性的格式，所以在某些不需要可读性，需要性能的场景更合适。

:::

## 如何使用SignalR进行横向扩展

### 首先讲一下什么是横向扩展

横向扩展（Horizontally Scaling），也常称为“扩展出”或“扩展宽”，是一种增加系统容量的方法，通过在现有的硬件集群中添加更多的机器或节点来实现。与之相对的是纵向扩展（Vertically Scaling）或称为“扩展高”，它涉及增加单一机器的资源，如CPU、RAM或存储。

横向扩展的主要特点和优势：

1. **弹性扩展**：能够根据需求动态地添加或减少节点，这在云计算环境中特别受欢迎。
2. **容错性**：由于存在多个节点，即使某个节点出现故障，系统也可以继续运行。
3. **负载分散**：请求可以在多个服务器或节点之间进行分配，避免了单一节点的瓶颈。
4. **通常更经济**：与购买一个大型、昂贵的超级服务器相比，购买多台中低规格的机器往往更为经济。

总的来说，当我们的单体服务器无法支撑我们现有用户的时候，只需要在添加节点便可支持更多用户。但是横向扩展也一样会有缺点，

1. **复杂性**：管理和维护多个节点可能会比维护一个高性能的节点更加复杂。
2. **数据一致性**：在多个节点上分散数据可能导致数据同步和一致性问题。
3. **网络开销**：节点间的通信可能增加网络延迟。
4. **软件兼容性**：并不是所有软件都能轻松地进行横向扩展，某些应用可能需要特定的设计或配置。

### 为什么要实现横向扩展

由于一个服务器的资源是有限的，虽然说在使用的时候并没有达到硬件的上线但也存在Tcp连接数的限制，以下是官方介绍

Web 服务器可以支持的并发 TCP 连接数受到限制。 标准 HTTP 客户端使用临时连接。 这些连接可以在客户端进入空闲状态时关闭，并在以后重新打开。 另一方面，SignalR 连接是持久性的。 SignalR 连接即使在客户端进入空闲状态时也保持打开状态。 在为许多客户端提供服务的高流量应用中，这些持久性连接可能会导致服务器达到其最大连接数。

持久性连接还会占用一些额外内存来跟踪每个连接。

SignalR 大量使用连接相关资源可能会影响在同一服务器上托管的其他 Web 应用。 SignalR 打开并保持最后一个可用 TCP 连接时，同一服务器上其他 Web 应用也不再有可用连接。

如果服务器的连接用完，则你会看到随机套接字错误和连接重置错误。 例如：

复制

```shell
An attempt was made to access a socket in a way forbidden by its access permissions...
```

若要防止 SignalR 资源使用在其他 Web 应用中导致错误，请在与其他 Web 应用不同的服务器上运行 SignalR。

若要防止 SignalR 资源使用在 SignalR 应用中导致错误，请横向扩展以限制服务器必须处理的连接数。

### Signalr是如何实现横向扩展的？

SignalR 通过一种称为“后端”或“后台”存储的机制实现横向扩展。在 SignalR 中，为了支持跨多个服务器或节点的连接和消息传递，需要一个中心的后台存储来确保消息在所有服务器之间都能正确地传递。

以下是 SignalR 实现横向扩展的几种常见方式：

1. [**Redis 后端**](https://learn.microsoft.com/zh-cn/aspnet/core/signalr/redis-backplane?view=aspnetcore-7.0)：Redis 是一个非常受欢迎的键值存储，SignalR 可以使用 Redis 作为后台存储来支持其横向扩展。当 SignalR 使用 Redis 时，所有的 SignalR 服务器都连接到同一个 Redis 实例或集群，并使用 Redis 的发布/订阅功能来传递消息。
2. [**SQL Server 后端**](https://github.com/IntelliTect/IntelliTect.AspNetCore.SignalR.SqlServer)：SignalR 也支持使用 SQL Server 作为后台存储，但这种方式的性能和可扩展性可能不如 Redis。
3. [**Azure Service Bus 后端**](https://learn.microsoft.com/zh-cn/aspnet/core/signalr/publish-to-azure-web-app?view=aspnetcore-7.0)：对于在 Azure 上运行的 SignalR 应用程序，Azure Service Bus 可以作为一个后台存储选项。
4. **自定义后端存储**：开发人员也可以为 SignalR 创建自定义的后端存储解决方案。

当 SignalR 使用后端存储进行横向扩展时，以下几点是需要考虑的：

- **负载均衡**：要确保所有的 SignalR 服务器之间的客户端连接请求能够均匀分配。
- **服务器亲和性**：在某些情况下，可能需要确保客户端总是连接到同一个 SignalR 服务器，这称为“服务器亲和性”或“会话亲和性”。但是，当使用后端存储如 Redis 时，这种亲和性往往不是必需的，因为所有的服务器都可以接收并广播消息。
- **资源和成本**：后端存储引入了额外的资源和成本，尤其是当使用付费服务（如 Azure Service Bus）或需要管理和维护的服务（如 Redis 或 SQL Server）时。

### Redis横向扩展

SignalR 使用 Redis 作为后端存储来实现横向扩展的方式是基于 Redis 的发布/订阅 (pub/sub) 功能。这使得在多个 SignalR 服务器实例之间同步和传递消息成为可能。以下是 SignalR 如何使用 Redis 实现横向扩展的过程：

1. **连接到 Redis**：每个 SignalR 服务器实例在启动时都会与配置好的 Redis 服务器或集群建立连接。
2. **订阅**：SignalR 服务器实例使用 Redis 的发布/订阅功能进行订阅。每当有一个新的 SignalR 集群加入时，它都会订阅相关的通道，以便接收消息。
3. **发布消息**：当一个 SignalR 服务器实例需要发送消息给它的客户端时（这可能是因为一个客户端向另一个客户端发送消息，而这两个客户端可能连接到不同的服务器实例），该服务器实例会将消息发布到 Redis。
4. **接收消息**：由于所有 SignalR 服务器实例都订阅了 Redis 的通道，因此它们都会接收到该消息。收到消息的每个服务器实例都会检查该消息是否针对其上的任何客户端，如果是，则将消息转发给这些客户端。
5. **负载均衡**：在使用 Redis 进行横向扩展时，还需要一个负载均衡器来确保新的客户端连接请求在所有 SignalR 服务器实例之间进行均衡分配。这样，不同的客户端可能连接到不同的服务器实例。
6. **持久连接和组**：SignalR 的 Redis 后端不仅支持持久连接（如 Hubs）的消息传递，还支持分组操作。例如，如果你在一个服务器实例上将客户端加入一个特定的组，并且稍后想向该组发送消息，即使发送请求来自另一个服务器实例，Redis 也能确保消息正确地发送给该组的所有成员。

要使用 Redis 作为 SignalR 的后端存储，开发者需要安装相应的 SignalR Redis 包，并在应用程序的配置中指定 Redis 作为后端存储。

总的来说，通过使用 Redis 的发布/订阅功能，SignalR 能够在多个服务器实例之间同步和传递消息，从而实现横向扩展。

### SqlServer横向扩展

SignalR 使用 SQL Server 作为后端来实现横向扩展主要是通过 SQL Server 的消息队列功能，特别是 SQL Server 的 Service Broker。以下是 SignalR 使用 SQL Server 进行横向扩展的基本原理：

1. **设置 Service Broker**：为了使用 SQL Server 作为 SignalR 的后端，首先需要确保 SQL Server 数据库启用了 Service Broker。
2. **消息队列**：SignalR 使用 Service Broker 提供的消息队列功能。当一个 SignalR 服务器实例需要广播消息到其他服务器实例时，它会将消息发布到 SQL Server 的一个特定队列中。
3. **消息通知**：当消息被放入队列时，Service Broker 会通知所有订阅了该队列的 SignalR 服务器实例。每个服务器实例随后可以从队列中检索并处理消息，然后将其转发给连接到该实例的客户端。
4. **持久化**：使用 SQL Server 作为后端的另一个优点是消息会持久化，这意味着即使所有的 SignalR 服务器都崩溃，消息仍然可以在系统恢复后被处理和传递。

要使用 SQL Server 作为 SignalR 的后端进行横向扩展，需要进行一些配置：

- 安装适当的 NuGet 包，例如 `Microsoft.AspNet.SignalR.SqlServer`。
- 在 SignalR 的配置中，指定使用 SQL Server 作为后端并提供适当的连接字符串。
- 确保使用的 SQL Server 数据库启用了 Service Broker。

尽管 SQL Server 可以作为 SignalR 的后端，并提供了持久化和横向扩展的能力，但使用它可能会引入一些性能考虑。例如，与内存中的解决方案（如 Redis）相比，使用 SQL Server 可能会导致更高的延迟。此外，还需要确保 SQL Server 自身具有足够的性能和资源来处理大量的 SignalR 消息流量。
