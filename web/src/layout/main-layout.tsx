import React, { Component, MouseEvent, RefObject } from 'react';
import './main-layout.scss'
import { Avatar, Badge, Button, Input, List } from '@douyinfe/semi-ui';
import { ChatGroupDto, GetUserDto } from '../dto';
import UserService from '../services/userService';
import ChatService from '../services/chatService';

import { IconSearch, IconPlus } from '@douyinfe/semi-icons';
import Content from '../components/content';
import ChatHubService from '../services/chatHubService';
import { Outlet, useNavigate } from 'react-router-dom';

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



interface state {
    middleWidth: number;
    selectid: number;
    user: GetUserDto,
    groups: ChatGroupDto[],
    selectGroup: ChatGroupDto,
    menu: any[]
}


class App extends Component<any, state> {
    state: state = {
        middleWidth: 230,
        selectid: 0,
        user: {} as GetUserDto,
        groups: [],
        selectGroup: {} as ChatGroupDto,
        menu: [{
            id: 0,
            name: '消息',
            icon: Message(),
            path: '/'
        }, {
            id: 1,
            name: '用户',
            icon: User(),
            path: '/user'
        }]
    };

    constructor(props: any) {
        super(props);

        body.setAttribute('theme-mode', 'dark');

        if (!localStorage.getItem('token')) {
            ChatHubService.stop();
            this.props.navigation('/login')
        }

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

    }

    selectMenu(item: any) {
        const { selectid } = this.state;
        if (selectid === item.id) {
            return
        }
        
        this.setState({
            selectid: item.id
        })
        this.props.navigation(item.path)
    }

    onSearch(v: any) {
        console.log(v)
    }

    render() {
        const { selectid, user, menu } = this.state;

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
                        {menu.map(x => {
                            return (
                                <div onClick={() => this.selectMenu(x)} className={"left-item " + (selectid === x.id ? " left-item-select" : null)}>
                                    {x.icon}
                                </div>)
                        })}
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
                <Outlet />
            </div>
        );
    }
}

function Main() {
    const navigation = useNavigate();
    return (
        <App navigation={navigation} />
    )
}

export default Main;
