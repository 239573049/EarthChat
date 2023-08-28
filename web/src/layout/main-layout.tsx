import React, { Component, MouseEvent, RefObject } from 'react';
import './main-layout.scss'
import { Avatar, Badge, Button, Input, List } from '@douyinfe/semi-ui';
import { ChatGroupDto, GetUserDto } from '../dto';
import UserService from '../services/userService';
import ChatService from '../services/chatService';

import { IconSearch, IconPlus } from '@douyinfe/semi-icons';
import Content from '../components/content';
import ChatHubService from '../services/chatHubService';

const body = document.body;


const Message = (color: string = '#A4A4A4') => {
    return <svg
        style={{
            fill: color,
            margin: '10px',
        }}
        className='icon-menu'
        viewBox="0 0 1024 1024">
        <path d="M682.666667 85.333333H341.333333C200.149333 85.333333 85.333333 200.149333 85.333333 341.333333v512a42.666667 42.666667 0 0 0 42.666667 42.666667h554.666667c141.184 0 256-114.816 256-256V341.333333c0-141.184-114.816-256-256-256z m170.666666 554.666667c0 94.122667-76.544 170.666667-170.666666 170.666667H170.666667V341.333333c0-94.122667 76.544-170.666667 170.666666-170.666666h341.333334c94.122667 0 170.666667 76.544 170.666666 170.666666v298.666667z" p-id="6573"></path><path d="M405.333333 490.666667m-64 0a64 64 0 1 0 128 0 64 64 0 1 0-128 0Z" p-id="6574"></path><path d="M618.666667 490.666667m-64 0a64 64 0 1 0 128 0 64 64 0 1 0-128 0Z" p-id="6575">
        </path>
    </svg>
}

const User = (color: string = '#A4A4A4') => {
    return <svg
        style={{
            fill: color,
            margin: '10px',
        }}
        className='icon-menu'
        viewBox="0 0 1024 1024"><path d="M691.2 608c-57.4 0-85 32-179.2 32-94.2 0-121.6-32-179.2-32C184.4 608 64 728.4 64 876.8V928c0 53 43 96 96 96h704c53 0 96-43 96-96v-51.2c0-148.4-120.4-268.8-268.8-268.8zM864 928H160v-51.2c0-95.2 77.6-172.8 172.8-172.8 29.2 0 76.6 32 179.2 32 103.4 0 149.8-32 179.2-32 95.2 0 172.8 77.6 172.8 172.8V928zM512 576c159 0 288-129 288-288S671 0 512 0 224 129 224 288s129 288 288 288z m0-480c105.8 0 192 86.2 192 192s-86.2 192-192 192-192-86.2-192-192 86.2-192 192-192z" p-id="8207"></path></svg>
}

const Function = (color: string = '#A4A4A4') => {
    return <svg
        style={{
            fill: color,
            margin: '10px',
        }}
        className='icon-menu'
        viewBox="0 0 1024 1024">
        <path d="M904 160H120c-4.4 0-8 3.6-8 8v64c0 4.4 3.6 8 8 8h784c4.4 0 8-3.6 8-8v-64c0-4.4-3.6-8-8-8zM904 784H120c-4.4 0-8 3.6-8 8v64c0 4.4 3.6 8 8 8h784c4.4 0 8-3.6 8-8v-64c0-4.4-3.6-8-8-8zM904 472H120c-4.4 0-8 3.6-8 8v64c0 4.4 3.6 8 8 8h784c4.4 0 8-3.6 8-8v-64c0-4.4-3.6-8-8-8z" p-id="9196"></path>
    </svg>
}



interface AppState {
    middleWidth: number;
    selectid: number;
    user: GetUserDto,
    groups: ChatGroupDto[],
    selectGroup: ChatGroupDto
}

class App extends Component<{}, AppState> {
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
                return <Content group={selectGroup}/>
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
            <div className="container">
                <div className="left" style={{ width: '65px' }}>
                    <div style={{
                        margin: "30px 0 0 10px"
                    }}>
                        <Badge dot position='rightBottom' style={{
                            backgroundColor: '#2CEA8C',
                            top: '20px',
                            left: '20px',
                            height: '13px',
                            width: '13px',
                        }}>
                            <Avatar size="default" alt='User' src={user.avatar}>
                            </Avatar>
                        </Badge>
                    </div>
                    <div style={{
                        margin: "25px 0 0 12px"
                    }}>
                        <div onClick={() => this.selectMenu(0)} className={"left-item " + (selectid === 0 ? " left-item-select" : null)}>
                            {Message()}
                        </div>
                        <div onClick={() => this.selectMenu(1)} className={"left-item " + (selectid === 1 ? " left-item-select" : null)}>
                            {User()}
                        </div>
                    </div>
                    {/* 显示在最下面 */}
                    <div className="left-item " style={{
                        margin: "25px 0 10px 12px",
                        position: 'absolute',
                        bottom: '0',
                        left: '0',
                        textAlign: 'center',
                    }}>
                        {Function()}
                    </div>

                </div>
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
            </div>
        );
    }
}

export default App;
