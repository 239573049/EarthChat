import { useState } from 'react';

import { Layout, Menu, Button, theme, Modal, message, Avatar } from 'antd';
import './App.css';
import OnLine from './components/on-line';
import Message from './components/message';
import UserInfo from './components/user-info';
import {
  githubAuth,
  giteeAuth
} from './services/authService'
import {
  GithubOutlined
} from '@ant-design/icons';

// 获取url参数code
const code = new URLSearchParams(window.location.search).get('code');
if (code) {
  const type = new URLSearchParams(window.location.search).get('type');
  if (type === 'github') {
    githubAuth(code)
      .then(res => {
        if (res.code === '200') {
          window.localStorage.setItem('token', res.data)
          window.location.href = '/'

        } else {
          message.error(res.message)
        }
      })
  } else if (type === 'gitee') {
    giteeAuth(code, window.location.origin + '/')
      .then(res => {
        if (res.code === '200') {
          window.localStorage.setItem('token', res.data)
          window.location.href = '/'
        } else {
          message.error(res.message)
        }
      })
  }
}

const token = localStorage.getItem('token')

const { Sider, Content } = Layout;
const App = () => {
  const {
    token: { colorBgContainer },
  } = theme.useToken();

  const [login] = useState(token === null)

  const [loginModal, setLoginModal] = useState(false)
  const onGithub = () => {
    open('https://github.com/login/oauth/authorize?client_id=8b0837ef77373394ce51&redirect_uri=' + window.location.origin + '?type=github', '_self');
  }

  const onGitee = () => {
    open(`https://gitee.com/oauth/authorize?client_id=89f52a53bde7f0df920498da747aa553c9e1e9f7c06fca754c0292decd334814&redirect_uri=${(window.location.origin + '?type=gitee')}&response_type=code`, '_self');
  }

  const GitHub=()=>{
    // 打开新标签
    open('https://gitee.com/hejiale010426/chat','_blank')
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
            {UserInfo()}
            <div style={{
              position: 'absolute',
              bottom: '10px',
              width: '100%',
              padding: '10px',
            }}>

              <Button block type='primary' onClick={GitHub} icon={<GithubOutlined/>} style={{
                marginBottom: '10px',
              }}>GitHub</Button>
              {login ? <Button type='primary' onClick={() => setLoginModal(true)} block>登录</Button> : <Button type='primary' onClick={() => {
                localStorage.removeItem('token')
                window.location.reload()
              }} block>退出登录</Button>}
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