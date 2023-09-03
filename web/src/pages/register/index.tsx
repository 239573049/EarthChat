
import React, { Component, RefObject } from 'react';
import './index.scss';
import { Avatar, Button, Form, Row, Toast, Upload } from '@douyinfe/semi-ui';
import { Link } from 'react-router-dom';
import { IconCamera } from '@douyinfe/semi-icons';
import userService from '../../services/userService';
import config from '../../config';
interface State {
    username: string;
    password: string;
    registerUpload: RefObject<Upload>,
    avatar: string
}

const body = document.body;


class Register extends Component<{}, State> {
    constructor(props: {}) {
        super(props);
        this.state = {
            username: '',
            password: '',
            avatar: '',
            registerUpload: React.createRef<Upload>(),
        };
        this.handleSubmit = this.handleSubmit.bind(this);
        body.removeAttribute('theme-mode');
    }

    handleSubmit = (e: any) => {
        const avatar = e?.avatar[0]?.response?.data;

        var value = {
            account: e.account,
            password: e.password,
            name: e.name,
            avatar: avatar
        };

        if(!value.avatar){
            Toast.error('请上传头像');
            return;
        }

        userService.create(value)
            .then((res: any) => {
                if (res.code === '200') {
                    Toast.success('注册成功');
                    
                    // 等待1秒后跳转
                    setTimeout(() => {
                        window.location.href = '/login';
                    }, 1000);
                } else {
                    Toast.error(res.message);
                }
            })

    };

    render() {
        const { avatar } = this.state;
        return (
            <div className="login-container">
                <div className="login-form">
                    <Form
                        style={{
                            margin: '60px 10px 0px 10px',
                            top: '20px',

                        }}
                        onSubmit={this.handleSubmit}
                    >
                        {
                            ({  }) =>

                            (
                                <Row>
                                    <Form.Upload field='avatar'
                                        className="avatar-upload"
                                        action={config.API + "/api/v1/Files/upload"}
                                        accept={'image/*'}
                                        ref={this.state.registerUpload}
                                        fileName='file'
                                        headers={{
                                            "Authorization": "Bearer " + localStorage.getItem('token')
                                        }}
                                        label=' '
                                        style={{
                                            left: '40%',
                                            position: 'relative',
                                        }}
                                        onSuccess={(file: any) => {
                                            if (file.code === '200') {
                                                this.setState({
                                                    avatar: file.data
                                                })
                                            } else {
                                                Toast.error(file.message);
                                            }
                                        }}
                                        showUploadList={false}
                                        onError={() => Toast.error('上传失败')}>
                                        <Avatar src={avatar} style={{ margin: 4 }} hoverMask={<IconCamera />} />
                                    </Form.Upload>
                                    <Form.Input style={{
                                        backgroundColor: '#F5F5F5',
                                        color: '#000000',
                                        borderRadius: '5px',
                                        height: '40px',
                                        width: '100%',
                                        fontSize: '20px',

                                    }} rules={[
                                        { required: true, message: '昵称不能为空' }
                                    ]} label=' ' placeholder={'请输入昵称'} field='name' />
                                    <Form.Input style={{
                                        backgroundColor: '#F5F5F5',
                                        color: '#000000',
                                        borderRadius: '5px',
                                        height: '40px',
                                        width: '100%',
                                        fontSize: '20px',

                                    }} label=' ' rules={[
                                        { required: true, message: '账号不能为空' },
                                        // @ts-ignore
                                        { validator: (rule, value) => value.length >= 5, message: '账号长度不能小于5位' },
                                    ]} placeholder={'请输入账号'} field='account' />
                                    <Form.Input style={{
                                        backgroundColor: '#F5F5F5',
                                        color: '#000000',
                                        borderRadius: '5px',
                                        height: '40px',
                                        width: '100%',
                                        fontSize: '20px',

                                    }} rules={[
                                        { required: true, message: '密码不能为空' },
                                        // @ts-ignore
                                        { validator: (rule, value) => value.length >= 5, message: '密码长度不能小于5位' },
                                    ]} type='password' placeholder={'请输入密码'} label=' ' field='password' />
                                    <Button block htmlType='submit' style={{
                                        backgroundColor: '#008DED',
                                    }} type="tertiary">注册</Button>
                                    <div style={{
                                        textAlign: 'right',
                                        margin: '10px 0 0 0',
                                        userSelect: 'none',
                                        cursor: 'pointer',
                                    }}>
                                        <Link to={'/login'}>登录</Link>
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

export default Register;
