import * as signalR from '@microsoft/signalr';
import { HubConnectionBuilder } from '@microsoft/signalr';
import * as msgpack from '@microsoft/signalr-protocol-msgpack';
import { Notification } from '@douyinfe/semi-ui'
import PubSub from 'pubsub-js';


const connection = new HubConnectionBuilder()
    .withUrl(import.meta.env.VITE_API+"/chathub", {
        accessTokenFactory: () => localStorage.getItem('token')!
    })
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .withHubProtocol(new msgpack.MessagePackHubProtocol())
    .build();

connection.start()
    .then(() => { })
    .catch((err) => {
        console.log(err);
        Notification.error({
            title: '连接失败',
        });
    });

connection.on("ReceiveMessage", (_, message) => {
    PubSub.publish('changeGroup', message);
});


export default connection;