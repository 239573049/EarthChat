import * as signalR from '@microsoft/signalr';
import { HubConnectionBuilder } from '@microsoft/signalr';
import * as msgpack from '@microsoft/signalr-protocol-msgpack';
import PubSub from 'pubsub-js';
import config from '../config';


const connection = new HubConnectionBuilder()
    .withUrl(config.API + "/api/chatHub", {
        accessTokenFactory: () => localStorage.getItem('token')!
    })
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .withHubProtocol(new msgpack.MessagePackHubProtocol())
    .build();

connection.on("ReceiveMessage", (_, message) => {
    PubSub.publish('changeGroup', message);
});

connection.on("Notification", message => {
    PubSub.publish('Notification', message);
})


export default connection;