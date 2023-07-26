import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App.jsx'
import { ConfigProvider, message } from 'antd';
import dayjs from 'dayjs';
import 'dayjs/locale/zh-cn';
import zhCN from 'antd/locale/zh_CN';
import './index.css';

import * as signalR from '@microsoft/signalr';
import { MessagePackHubProtocol } from '@microsoft/signalr-protocol-msgpack';
import config from './config';

dayjs.locale('zh-cn');

const connection = new signalR.HubConnectionBuilder()
    .withUrl(config.API_URL+"/chatHub", { accessTokenFactory: () => localStorage.getItem('token') })
    .withHubProtocol(new MessagePackHubProtocol())
    .configureLogging(signalR.LogLevel.Information)
    .build();

window.connection = connection;


await connection.start();

connection.onclose(async () => {
  message.error('连接已断开，正在重连...');
});

ReactDOM.createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    <ConfigProvider locale={zhCN}>
      <App />
    </ConfigProvider>
  </React.StrictMode>,
)
