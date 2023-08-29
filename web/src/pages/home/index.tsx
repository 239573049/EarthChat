import React, { Component, MouseEvent, RefObject } from 'react';
import './index.scss'
import { Avatar, Badge, Button, Input, List } from '@douyinfe/semi-ui';
import { ChatGroupDto, GetUserDto } from '../../dto';
import UserService from '../../services/userService';
import ChatService from '../../services/chatService';

import { IconSearch, IconPlus } from '@douyinfe/semi-icons';
import Content from '../../components/content';
import ChatHubService from '../../services/chatHubService';

const body = document.body;

interface AppState {
    middleWidth: number;
    selectid: number;
    user: GetUserDto,
    groups: ChatGroupDto[],
    selectGroup: ChatGroupDto
}

class Home extends Component<{}, AppState> {
    state: AppState = {
        middleWidth: 230,
        selectid: 0,
        user: {} as GetUserDto,
        groups: [],
        selectGroup: {} as ChatGroupDto
    };

    constructor(props: any) {
        super(props);

        body.setAttribute('theme-mode', 'dark');

        if (!localStorage.getItem('token')) {
            ChatHubService.stop();
            window.location.href = '/login'
        }


        this.handleMouseDown = this.handleMouseDown.bind(this);
    }

    componentDidMount() {
        UserService.get()
            .then((res: any) => {
                if (res.code === '200') {
                    localStorage.setItem('user', JSON.stringify(res.data));
                    this.setState({
                        user: res.data
                    })
                }
            })

        this.loadingGroups()
    }

    handleMouseDown = (e: React.MouseEvent) => {
        const startX = e.clientX;
        const startWidth = this.state.middleWidth;

        const onMouseMove = (e: any) => {
            const newWidth = Math.max(200, Math.min(350, startWidth + (e.clientX - startX)));
            this.setState({ middleWidth: newWidth });
        };

        const onMouseUp = () => {
            window.removeEventListener('mousemove', onMouseMove);
            window.removeEventListener('mouseup', onMouseUp);
        };

        window.addEventListener('mousemove', onMouseMove);
        window.addEventListener('mouseup', onMouseUp);
    };

    selectMenu(id: number) {
        const { selectid } = this.state;
        if (selectid === id) {
            return
        }
        this.setState({
            selectid: id
        })
    }

    onSearch(v: any) {
        console.log(v)
    }

    loadingGroups() {
        ChatService.getUserGroup()
            .then((res: ChatGroupDto[]) => {
                res.forEach(x => {
                    x.lastMessage = '最新回复';
                })
                this.setState({
                    groups: res,
                    selectGroup: res[0]
                })
            })

    }

    selectChat(dto: ChatGroupDto) {
        this.setState({
            selectGroup: dto
        })
    }

    render() {
        const { middleWidth, selectid, user, selectGroup, groups } = this.state;
        const rightWidth = `calc(100% - 60px - ${middleWidth}px)`;

        const renderContent = () => {
            if (selectGroup.avatar) {
                return <Content group={selectGroup} />
            } else {
                return <div>
                    {/* 居中显示图片 */}
                    <div style={{
                        margin: '0 auto',
                        width: '100%',
                        textAlign: 'center',
                        marginTop: '300px',
                    }}>
                        <img style={{
                            width: '200px',
                            height: '200px',
                        }} src="/favicon.png" alt="" />
                    </div>
                </div>
            }
        }

        return (
            <>

                <div className="middle" style={{ width: `${middleWidth}px` }}>
                    <div style={{
                        marginTop: '30px',
                        fontSize: '20px',
                        fontWeight: 'bold',
                        height: "100%"
                    }}>
                        <List
                            dataSource={groups}
                            split={false}
                            header={<div style={{
                                height: '40px',
                            }}>
                                <Input
                                    style={{
                                        width: 'calc(100% - 40px)',
                                        borderRadius: '5px',
                                        float: 'left',
                                    }}
                                    placeholder="搜索"
                                    prefix={<IconSearch />}
                                />
                                <Button style={{
                                    float: 'right',
                                    borderRadius: '5px',
                                }} icon={<IconPlus />}></Button>
                            </div>}
                            size='small'
                            style={{
                                width: '100%',
                            }}
                            renderItem={item =>
                                <div onClick={() => { this.selectChat(item) }} className={'chat-item ' + (selectGroup === item ? "chat-item-select" : "")}>
                                    <div style={{
                                        padding: '15px',
                                    }}>
                                        <Avatar style={{
                                            height: '40px',
                                            width: '40px',
                                            float: 'left',
                                            marginRight: '10px',
                                        }} size="small" alt='User' src={item.avatar}>
                                        </Avatar>
                                        <div style={{
                                            userSelect: 'none',
                                            float: 'left',
                                        }}>
                                            <span style={{
                                                fontSize: '16px',
                                                fontWeight: 'bold',
                                            }}>
                                                {item.name}
                                            </span>
                                            <span style={{
                                                display: 'block',
                                                fontSize: '12px',

                                            }}>
                                                {item.lastMessage}
                                            </span>
                                        </div>
                                    </div>
                                </div>
                            }
                        />
                    </div>
                </div>
                <div className="resizer" onMouseDown={this.handleMouseDown}></div>
                <div className="right" style={{ width: rightWidth }}>
                    {renderContent()}
                </div>
            </>
        );
    }
}

export default Home;
