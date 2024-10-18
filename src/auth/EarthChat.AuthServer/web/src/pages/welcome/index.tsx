import { memo } from "react";
import { Button, Layout, Menu, theme } from 'antd';

const { Header, Content, Footer } = Layout;


const WelcomePage = memo(() => {
    const {
        token: { colorBgContainer, borderRadiusLG },
    } = theme.useToken();


    return (<Layout style={{
        minHeight: '100vh',
        margin: 0,
        padding: 0,
    }}>
        <Header
            style={{
                position: 'sticky',
                top: 0,
                zIndex: 1,
                width: '100%',
                display: 'flex',
                alignItems: 'center',
            }}
        >
            <Menu
                theme="dark"
                mode="horizontal"
                defaultSelectedKeys={['home']}
                items={[
                    {
                        label: '首页',
                        key: 'home',
                    },
                    {
                        label: '关于',
                        key: 'about',
                    }
                ]}
                style={{ flex: 1, minWidth: 0 }}
            />
        </Header>
        <Content style={{ padding: '0 48px' }}>
            <div
                style={{
                    padding: 24,
                    minHeight: 380,
                    background: colorBgContainer,
                    borderRadius: borderRadiusLG,
                }}
            >

            </div>
        </Content>
        <Footer style={{ textAlign: 'center' }}>
            @2024 Created by TokenAI
        </Footer>
    </Layout>
    )
});

export default WelcomePage