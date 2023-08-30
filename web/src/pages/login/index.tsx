import React, { Component } from 'react';
import './index.scss';
import { Avatar, Button, Form, Row, Toast } from '@douyinfe/semi-ui';
import AuthService from '../../services/authService';

interface State {
    username: string;
    password: string;
}

const Gitee = () => {
    return <svg style={{
        cursor: 'pointer',
        margin: '0 20px 0 5px',
    }} viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="10182" width="30" height="30"><path d="M512 1024C229.222 1024 0 794.778 0 512S229.222 0 512 0s512 229.222 512 512-229.222 512-512 512z m259.149-568.883h-290.74a25.293 25.293 0 0 0-25.292 25.293l-0.026 63.206c0 13.952 11.315 25.293 25.267 25.293h177.024c13.978 0 25.293 11.315 25.293 25.267v12.646a75.853 75.853 0 0 1-75.853 75.853h-240.23a25.293 25.293 0 0 1-25.267-25.293V417.203a75.853 75.853 0 0 1 75.827-75.853h353.946a25.293 25.293 0 0 0 25.267-25.292l0.077-63.207a25.293 25.293 0 0 0-25.268-25.293H417.152a189.62 189.62 0 0 0-189.62 189.645V771.15c0 13.977 11.316 25.293 25.294 25.293h372.94a170.65 170.65 0 0 0 170.65-170.65V480.384a25.293 25.293 0 0 0-25.293-25.267z" fill="#C71D23" p-id="10183"></path></svg>
}

const GetHub = () => {
    return <svg style={{
        cursor: 'pointer',
        margin: '0 20px 0 5px',
    }} viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="10182" width="30" height="30"><path d="M64 512c0 195.2 124.8 361.6 300.8 422.4 22.4 6.4 19.2-9.6 19.2-22.4v-76.8c-134.4 16-140.8-73.6-150.4-89.6-19.2-32-60.8-38.4-48-54.4 32-16 64 3.2 99.2 57.6 25.6 38.4 76.8 32 105.6 25.6 6.4-22.4 19.2-44.8 35.2-60.8-144-22.4-201.6-108.8-201.6-211.2 0-48 16-96 48-131.2-22.4-60.8 0-115.2 3.2-121.6 57.6-6.4 118.4 41.6 124.8 44.8 32-9.6 70.4-12.8 112-12.8 41.6 0 80 6.4 112 12.8 12.8-9.6 67.2-48 121.6-44.8 3.2 6.4 25.6 57.6 6.4 118.4 32 38.4 48 83.2 48 131.2 0 102.4-57.6 188.8-201.6 214.4 22.4 22.4 38.4 54.4 38.4 92.8v112c0 9.6 0 19.2 16 19.2C832 876.8 960 710.4 960 512c0-246.4-201.6-448-448-448S64 265.6 64 512z" fill="#040000" p-id="11159"></path></svg>
}

const body = document.body;


class Login extends Component<{}, State> {
    constructor(props: {}) {
        super(props);
        this.state = {
            username: '',
            password: ''
        };
        this.handleSubmit = this.handleSubmit.bind(this);
        body.removeAttribute('theme-mode');
    }

    handleSubmit = (e: any) => {
        AuthService.Login(e)
            .then((res: any) => {
                if (res.code === '200') {
                    localStorage.setItem('token', res.data);

                    // 获取url的参数
                    const queryString = window.location.search;

                    // 创建URLSearchParams对象
                    const params = new URLSearchParams(queryString);

                    // 获取指定参数的值
                    var redirect = params.get('redirect');

                    Toast.success('登录成功');

                    // 等待1秒后跳转
                    setTimeout(() => {
                        if (redirect) {
                            window.location.href = redirect;
                        } else {

                            window.location.href = '/';
                        }
                    }, 1000);
                } else {
                    Toast.error(res.message);
                }
            })

    };

    render() {
        return (
            <div className="login-container">
                <div className="login-form">
                    <Avatar size="large"
                        style={{
                            margin: 4,
                            top: '18%',
                            left: '50%',
                            transform: 'translate(-50%, -50%)'
                        }}
                        alt='User' src='/favicon.png'>
                    </Avatar>

                    <Form
                        style={{
                            margin: '60px 10px 0px 10px',
                            top: '20px',

                        }}
                        onSubmit={this.handleSubmit}
                    >
                        {
                            ({ formState, values, formApi }) =>

                            (
                                <Row>
                                    <Form.Input style={{
                                        backgroundColor: '#F5F5F5',
                                        color: '#000000',
                                        borderRadius: '5px',
                                        height: '40px',
                                        width: '100%',
                                        fontSize: '20px',

                                    }} label=' ' placeholder={'请输入账号'} field='account' />
                                    <Form.Input style={{
                                        backgroundColor: '#F5F5F5',
                                        color: '#000000',
                                        borderRadius: '5px',
                                        height: '40px',
                                        width: '100%',
                                        fontSize: '20px',

                                    }} placeholder={'请输入密码'} label=' ' field='password' />
                                    <Form.Checkbox field='agree' noLabel>我已阅读并且同意服务协议和隐私安全保护</Form.Checkbox>
                                    <Button disabled={!values.agree} block htmlType='submit' style={{
                                        backgroundColor: '#008DED',
                                    }} type="tertiary">登录</Button>
                                    <div style={{
                                        textAlign: 'right',
                                        margin: '10px 0 0 0',
                                        userSelect: 'none',
                                        cursor: 'pointer',
                                    }}>
                                        <span>注册</span>
                                    </div>
                                    <div style={{
                                        margin: '25px 0 0 0',
                                    }}>
                                        <span>
                                            {Gitee()}
                                        </span>
                                        <span>
                                            {GetHub()}
                                        </span>
                                    </div>
                                </Row>
                            )
                        }
                    </Form>
                </div>
            </div>
        );
    }
}

export default Login;
