import * as signalR from '@microsoft/signalr';
import { HubConnectionBuilder } from '@microsoft/signalr';
import * as msgpack from '@microsoft/signalr-protocol-msgpack';
import PubSub from 'pubsub-js';
import config from '../config';


const connection = new HubConnectionBuilder()
    .withUrl(config.API+"/chathub", {
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