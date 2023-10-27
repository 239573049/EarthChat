import * as signalR from "@microsoft/signalr";
import { HubConnectionBuilder } from "@microsoft/signalr";
import * as msgpack from "@microsoft/signalr-protocol-msgpack";
import PubSub from "pubsub-js";
import config from "../config";
import { Notification } from "@douyinfe/semi-ui";

const connection = new HubConnectionBuilder()
  .withUrl(config.API + "/api/chatHub", {
    accessTokenFactory: () => localStorage.getItem("token")!,
  })
  .withAutomaticReconnect()
  .withHubProtocol(new msgpack.MessagePackHubProtocol())
  .build();

connection.on("ReceiveMessage", (_, message) => {
  PubSub.publish("changeGroup", message);
});

connection.on("Notification", (message) => {
  PubSub.publish("Notification", message);
});

connection.onreconnecting(()=>{

  console.log('onclose');
  
  Notification.error({
    content:"断开服务链接！"
  })
});

export default connection;
