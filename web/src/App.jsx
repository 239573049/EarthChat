import React, { useState } from 'react';
import { UploadOutlined, UserOutlined, VideoCameraOutlined } from '@ant-design/icons';
import { Layout, Menu, theme, Avatar,Divider, List, Skeleton } from 'antd';
const { Header, Content, Footer, Sider } = Layout;
import InfiniteScroll from 'react-infinite-scroll-component';
const App = () => {
  const {
    token: { colorBgContainer },
  } = theme.useToken();

  // 定义一个menukey
  const [menuKey, setMenuKey] = useState({
    key: '1',
    label: '列表1',
  });

  const [menu, setMenu] = useState([{
    key: '1',
    label: '列表1',
  }, {
    key: '2',
    label: '列表2',
  }, {
    key: '3',
    label: '列表3',
  }, {
    key: '4',
    label: '列表4',
  }]);

  const [message, setMessage] = useState([{
    id: 1,
    content: '消息1',

  }]);

  function selectMenu(key) {
    const menuKey = menu.find((item) => item.key === key);
    setMenuKey(menuKey);
  }

  return (
    <Layout style={{
      height: 'calc(100vh - 20px)',
    }}>
      <Sider
        theme="light"
        breakpoint="lg"
        onBreakpoint={(broken) => {
          console.log(broken);
        }}
        onCollapse={(collapsed, type) => {
          console.log(collapsed, type);
        }}
      >
        <Menu
          theme="light"
          mode="inline"
          defaultSelectedKeys={[menuKey.key]}
          key={menuKey.key}
          selectedKeys={[menuKey.key]}
          onSelect={(item) => selectMenu(item.key)}
          items={menu.map(
            (item) => ({
              key: item.key,
              icon: <Avatar src='https://gw.alipayobjects.com/zos/rmsportal/KDpgvguMpGfqaHPjicRK.svg' />,
              label: item.label,
            }),
          )}
        />
      </Sider>
      <Layout>
        <Header
          style={{
            padding: 0,
            background: colorBgContainer,
          }}
        >
          {menuKey.label}
        </Header>
        <Content
          style={{
            margin: '24px 16px 0',
          }}
        >
          <div
            style={{
              padding: 24,
              minHeight: 360,
              background: colorBgContainer,
            }}
          >

            <InfiniteScroll
              dataLength={data.length}
              next={loadMoreData}
              hasMore={data.length < 50}
              loader={
                <Skeleton
                  avatar
                  paragraph={{
                    rows: 1,
                  }}
                  active
                />
              }
              endMessage={<Divider plain>It is all, nothing more 🤐</Divider>}
              scrollableTarget="scrollableDiv"
            >
              <List
                dataSource={data}
                renderItem={(item) => (
                  <List.Item key={item.email}>
                    <List.Item.Meta
                      avatar={<Avatar src={item.picture.large} />}
                      title={<a href="https://ant.design">{item.name.last}</a>}
                      description={item.email}
                    />
                    <div>Content</div>
                  </List.Item>
                )}
              />
            </InfiniteScroll>
          </div>
        </Content>
        <Footer
          style={{
            textAlign: 'center',
          }}
        >
          Ant Design ©2023 Created by Ant UED
        </Footer>
      </Layout>
    </Layout>
  );
};
export default App;