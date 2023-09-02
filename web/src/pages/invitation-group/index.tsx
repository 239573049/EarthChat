import { Component } from 'react'
import './index.scss';
import { Avatar, Button, Row, Toast } from '@douyinfe/semi-ui';
import { ChatGroupDto } from '../../dto';
import chatService from '../../services/chatService';


interface State {
    group: ChatGroupDto
}


class InvitationGroup extends Component<{}, State> {

    state: Readonly<State> = {
        group: {

        } as ChatGroupDto
    }

    constructor(props: {}) {
        super(props);

        this.goGroup = this.goGroup.bind(this);
        this.goHome = this.goHome.bind(this);

        if (!localStorage.getItem('token')) {
            location.href = window.location.origin + '/login?redirect=' + location.href
        }

        this.loading();
    }

    loading() {
        // 获取url的参数
        const queryString = window.location.search;

        // 创建URLSearchParams对象
        const params = new URLSearchParams(queryString);

        // 获取指定参数的值
        var code = params.get('code');

        chatService.getGroup(code!)
            .then(x => {
                this.setState({
                    group: x.data
                })
            })
    }


    goGroup() {
        // 获取url的参数
        const queryString = window.location.search;

        // 创建URLSearchParams对象
        const params = new URLSearchParams(queryString);

        // 获取指定参数的值
        var code = params.get('code');
        chatService.invitationGroup(code!)
            .then(x => {
                if (x.code === '200') {
                    Toast.success("加入成功，等待跳转到首页");

                    // 等待1秒后跳转
                    setTimeout(() => {
                        window.location.href = '/';
                    }, 1000);
                } else {
                    Toast.error(x.message)
                }
            })
    }

    goHome() {
        window.location.href = '/';
    }

    render() {
        const { group } = this.state;
        return (
            <div className="invitation-group-container">
                <div className="form">
                    <Avatar size="large"
                        style={{
                            margin: 4,
                            top: '18%',
                            left: '50%',
                            transform: 'translate(-50%, -50%)'
                        }}
                        alt='User' src={group?.avatar}>
                    </Avatar>

                    <Row
                        style={{
                            margin: '60px 10px 0px 10px',
                            top: '20px',

                        }}>
                        <div style={{
                            textAlign: 'center',
                            fontSize: "18px"
                        }}>
                            <span>邀请您加入</span>
                            <span style={{
                                fontWeight: 'bold',
                            }}>
                                {group.name}
                            </span>
                        </div>
                        <Button onClick={this.goGroup} theme='borderless' style={{
                            marginLeft: "40%",
                            marginTop: "20px"
                        }}>加入群聊</Button>
                        <Button onClick={this.goHome} theme='borderless' style={{
                            marginLeft: "40%",
                            marginTop: "20px"
                        }}>返回首页</Button>
                    </Row>
                </div>
            </div>
        );
    }
}

export default InvitationGroup;
