import { Component } from 'react';
import './main-layout.scss'
import { Avatar, Badge, Button, Divider, Input, Modal, Popover, Toast, Tooltip, Upload } from '@douyinfe/semi-ui';
import { ChatGroupDto, GetUserDto } from '../dto';
import UserService from '../services/userService';
import ChatHubService from '../services/chatHubService';
import { Outlet, useNavigate } from 'react-router-dom';
import { IconEdit, IconCamera } from '@douyinfe/semi-icons';
import config from '../config';
import userService from '../services/userService';
import Setting from '../components/setting';
import PubSub from 'pubsub-js';

const body = document.body;

// 用于获取记忆的主题并且修改主题
const theme = localStorage.getItem('theme-mode');
if (theme) {
    if (theme === "light") {
        body.removeAttribute('theme-mode');
    } else if (theme === "dark") {
        body.setAttribute('theme-mode', 'dark');
    }
}

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


const style = {
    backgroundColor: 'var(--semi-color-overlay-bg)',
    height: '100%',
    width: '100%',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    color: 'var(--semi-color-white)',
};
const hoverMask = (<div style={style}>
    <IconCamera />
</div>);


interface state {
    middleWidth: number;
    selectid: number;
    user: GetUserDto,
    groups: ChatGroupDto[],
    selectGroup: ChatGroupDto,
    menu: any[],
    settingVisible: boolean,
    updateUserVisible: boolean,
}


class App extends Component<any, state> {
    state: state = {
        middleWidth: 230,
        selectid: 0,
        user: {
            name: "",
            avatar: "",

        } as GetUserDto,
        settingVisible: false,
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
        }],
        updateUserVisible: false
    };

    constructor(props: any) {
        super(props);


        ChatHubService.start()
            .then(() => { });

        if (!localStorage.getItem('token')) {
            ChatHubService.stop();
            this.props.navigation('/login')
        }

        this.selectGroup = this.selectGroup.bind(this)

    }

    selectGroup(name:string,value:any){
        debugger;
        if(value){
            const {menu} = this.state;
            this.selectMenu(menu[0])
            setTimeout(()=>{
                PubSub.publish('selectGroupInfo',value)
            },500);
        }
        
    }

    componentWillUnmount(){
        console.log('卸载组件');
        
        PubSub.unsubscribe('selectGroup')
    }

    componentDidMount() {
        PubSub.subscribe('selectGroup',this.selectGroup)
        const {menu} = this.state;
        const item = menu.find(x=>x.path == location.pathname);
        if(item){
            this.selectMenu(item)
        }

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


    onExit() {
        localStorage.removeItem('token');
        location.href = '/login'
    }

    renderFunction() {
        return (
            <div className='menu'>
                <div className='menu-item' onClick={()=>this.setState({
                    settingVisible:true
                })}>设置</div>
                <div className='menu-item'>关于</div>
                <div className='menu-item' onClick={() => this.onExit()}>退出登录</div>
            </div>
        )
    }

    

    renderInfo() {
        const { user } = this.state;
        return <div style={{
            height: '300px',
            width: '315px',
            backgroundColor: "var(--chat-list-background-color)",
            borderRadius: '8px',
            paddingTop: '30px',
            paddingLeft: '20px',

        }}>
            <div style={{
                height: "120px",
                width: "100%",
            }}>
                <Avatar style={{
                    float: "left",
                }} size="large" alt='User' src={user.avatar}>
                </Avatar>
                <div style={{
                    float: "left",
                    marginLeft: '15px'
                }}>
                    <div style={{
                        color: 'red',
                        fontSize: '17x'
                    }}>
                        {user.name}
                    </div>
                    <div style={{
                        color: 'var(--semi-color-text-0)',
                        marginTop: "5px",
                    }}>
                        账号 {user.account}
                    </div>
                </div>
            </div>
            <div>
                <div style={{
                    fontSize: "16px",
                    color: 'var(--semi-color-text-1)',
                    width: '80px',
                    float: 'left'
                }}>
                    状态
                </div>
                <div style={{
                    fontSize: "16px",
                    color: 'var(--semi-color-text-2)',
                }}>
                    在线
                </div>
            </div>
            <div style={{
                marginTop: "20px",
                marginBottom: "10px"
            }}>
                <div style={{
                    fontSize: "16px",
                    color: 'var(--semi-color-text-1)',
                    width: '80px',
                    float: 'left'
                }}>
                    签名
                </div>
                <div style={{
                    fontSize: "14px",
                    color: 'var(--semi-color-text-2)',
                }}>
                    不要浪费生命，你还有很多事情要做！
                </div>
            </div>
            <Divider></Divider>
            <div style={{
                border: '1px solid #878787',
                borderRadius: '500px',
                width: "40px",
                float: "right",
                margin: '10px'
            }} onClick={() => this.setState({ updateUserVisible: true })}>
                <Button theme='borderless' size='large' icon={<IconEdit size='large' />}></Button>
            </div>
        </div>
    }

    updateUser() {
        const { user } = this.state;

        userService.update(user)
            .then(res => {
                if (res.code === "200") {
                    Toast.success("编辑成功")
                } else {
                    Toast.success("错误编辑")
                }
            }).finally(() => {
                this.setState({
                    updateUserVisible: false,
                })
            })
    }

    render() {
        const { selectid, settingVisible, user, menu, updateUserVisible } = this.state;
        let imageOnly = 'image/*';

        return (
            <div className="container">
                <div className="left" style={{ width: '65px' }}>
                    <div style={{
                        margin: "30px 0 0 10px"
                    }}>
                        <Popover position='rightBottom' content={this.renderInfo()} trigger="click">
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
                        </Popover>
                    </div>
                    <div style={{
                        margin: "25px 0 0 12px"
                    }}>
                        {menu.map(x => {
                            return (
                                <div key={"menu" + x.id} onClick={() => this.selectMenu(x)} className={"left-item " + (selectid === x.id ? " left-item-select" : null)}>
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
                        <Tooltip style={{
                            backgroundColor: 'var(--select-background-color)',
                            color: 'var(--semi-color-text-0)'
                        }} position='rightBottom' content={this.renderFunction()}>
                            {Function()}
                        </Tooltip>
                    </div>

                </div>
                <Outlet />
                <Modal
                    title="编辑资料"
                    visible={updateUserVisible}
                    onOk={() => this.updateUser()}
                    onCancel={() => this.setState({ updateUserVisible: false })}
                >
                    <div style={{ display: "flex", justifyContent: "center", }}>

                        <Upload
                            className="avatar-upload"
                            action={config.API + "/api/v1/Files/upload"}
                            headers={{
                                "Authorization": "Bearer " + localStorage.getItem('token')
                            }}
                            fileName='file'
                            onSuccess={(e) => {
                                if (e.code === '200') {
                                    this.setState({
                                        user: {
                                            ...user,
                                            avatar: e.data
                                        }
                                    })
                                } else {
                                    Toast.error(e.message)
                                }
                            }}
                            accept={imageOnly}
                            showUploadList={false}
                        >
                            <Avatar size='large' src={user.avatar} style={{ margin: 4 }} hoverMask={hoverMask} />
                        </Upload>
                    </div>
                    <div>
                        <Input style={{
                            margin: "10px",
                        }} prefix='昵称' suffix={`${user.name.length}/36`} value={user.name} onChange={(e) => {
                            if (e.length > 36) {
                                return;
                            }
                            this.setState({
                                user: {
                                    ...user,
                                    name: e
                                }
                            })
                        }} showClear></Input>
                    </div>

                </Modal>
                <Setting onCancel={()=>{
                    this.setState({
                        settingVisible:false
                    })
                }} visible={settingVisible} />
            </div>
        );
    }
}

function Main() {
    const navigation = useNavigate();
    return (
        <App key={'Main_App'} navigation={navigation} />
    )
}

export default Main;
