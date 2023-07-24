import React, { useState } from 'react';

import { Layout, Menu, Button, theme, Modal } from 'antd';
import './App.css';
import OnLine from './components/on-line';
import Message from './components/message';

// 获取url参数code
const code = new URLSearchParams(window.location.search).get('code');
if(code){
  console.log(code);
}

const { Sider, Content } = Layout;
const App = () => {
  const {
    token: { colorBgContainer },
  } = theme.useToken();

  const [login, setLogin] = useState(true)

  const [loginModal, setLoginModal] = useState(false)

  const onGithub = () => {
    open('https://github.com/login/oauth/authorize?client_id=8b0837ef77373394ce51&redirect_uri=' + window.location.origin + '?type=github', '_self');

  }

  const onGitee = () => {
    open(`https://gitee.com/oauth/authorize?client_id=89f52a53bde7f0df920498da747aa553c9e1e9f7c06fca754c0292decd334814&redirect_uri=${(window.location.origin + '?type=gitee')}&response_type=code`, '_self');
  }

  return (
    <Layout style={{
      height: '100vh',
    }}>
      <Sider trigger={null} collapsible>
        <div className="demo-logo-vertical" />
        <Menu
          theme="dark"
          mode="inline">
          <div style={{
            textAlign: 'center',
            marginTop: '20px',
          }}>
            <div style={{
              color: '#fff',
              fontSize: '20px',
            }}>
              世界Chat
            </div>
            <div style={{
              position: 'absolute',
              bottom: '10px',
              width: '100%',
              padding: '10px',
            }}>
              {login ? <Button type='primary' onClick={() => setLoginModal(true)} block>登录</Button> : <></>}
            </div>


          </div>
        </Menu>
      </Sider>
      <Layout>
        <Content
          style={{
            margin: '24px 16px',
            padding: 24,
            minHeight: 280,
            background: colorBgContainer,
          }}
        >
          <Message />
          <OnLine />
        </Content>
      </Layout>
      <Modal
        open={loginModal}
        title="登录界面"
        onCancel={() => setLoginModal(false)}
        footer={[]}
        style={{
          textAlign: 'center',
        }}
      >
        <h2>
          世界Chat登录
        </h2>
        <Button style={{
          margin: '10px',
        }} onClick={() => onGithub()}>Github</Button>
        <Button style={{
          margin: '10px',
        }} onClick={() => onGitee()}>Gitee</Button>
      </Modal>
    </Layout>
  );
};
export default App;