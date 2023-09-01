import * as signalR from '@microsoft/signalr';
import { HubConnectionBuilder } from '@microsoft/signalr';
import * as msgpack from '@microsoft/signalr-protocol-msgpack';
import PubSub from 'pubsub-js';


const connection = new HubConnectionBuilder()
    .withUrl(import.meta.env.VITE_API+"/chathub", {
        accessTokenFactory: () => localStorage.getItem('token')!
    })
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .withHubProtocol(new msgpack.MessagePackHubProtocol())
    .build();

connection.on("ReceiveMessage", (_, message) => {
    PubSub.publish('changeGroup', message);
});


export default connection;