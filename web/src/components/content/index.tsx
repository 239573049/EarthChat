import React, { Component, RefObject } from 'react';
import { ChatGroupDto, FriendRegistrationInput, GetUserDto } from '../../dto';
import moment from 'moment/moment';
import { Avatar, Button, Card, Icon, Image, Tag, Notification, Toast, Badge, Tooltip, Spin, List, Popover, Modal, Input, TextArea, Form } from '@douyinfe/semi-ui';
import './index.scss';
import Mention from '../mentions/index';
import ChatHubService from '../../services/chatHubService';
import fileService from '../../services/fileService';
import PubSub from 'pubsub-js';
import copy from 'copy-to-clipboard';
import ChatService from '../../services/chatService';
import emojiService from '../../services/emojiService';
import { IconPlus, IconSmallTriangleTop } from '@douyinfe/semi-icons';
import { GetUserInfos } from '../../store/user-store'
import { emoji } from '../../store/emoji';
import FriendService from '../../services/friendService'
import VideoPlayer from '../video';
import chatService from '../../services/chatService';
import RenderText from '../render-text';
import FlipMove from 'react-flip-move';

interface IProps {
    group: ChatGroupDto;
}

interface IState {
    height: number;
    data: any[],
    unread: number,
    loading: boolean,
    page: number,
    groupinUsers: any[],
    users: GetUserDto[],
    custom: any[],
    onLineUserLoading: boolean,
    groudUserPage: number,
    groupLoading: boolean,
    emojiKey: number,
    updateState: boolean,
    addFriend: {
        visible: boolean,
        id: string;
        user: GetUserDto
    },
    groupinUser: {
        page: number,
        pageSize: number,
        loading: boolean,
    },
    revertValue: any
}

const user = JSON.parse(localStorage.getItem('user') || '{}');

const invitationIcon = () => {
    return <svg className='icon-function' viewBox="0 0 1029 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="12791" width="20" height="20"><path d="M991.601847 386.035547c-22.031805-13.410664-49.811038-13.410664-71.842844 0l-347.719364 229.8971c-36.400374 24.905519-84.295603 24.905519-120.695977 0L107.455916 388.909261c-22.031805-14.368569-49.811038-16.284378-71.842844-3.831618C12.623362 396.572498-1.745206 421.478017 0.170603 447.341441V871.693171c0 58.43218 47.895229 106.327409 105.369504 106.327409h819.008419c27.779233 0 53.642657-10.53695 73.758653-30.652947 20.115996-20.115996 31.610851-46.937325 31.610851-74.716557V447.341441c0-24.905519-14.368569-49.811038-38.316183-61.305894z m-31.610851 70.884939v402.319926c0 22.031805-18.200187 49.811038-38.316184 49.811038H122.78239c-22.031805 0-55.558466-29.695042-55.558466-49.811038v-402.319926l347.719364 230.855005c59.390084 38.316183 136.022451 38.316183 194.45463 0l350.593078-230.855005z m66.095416 415.73059z" p-id="12792"></path><path d="M175.467142 387.951356l7.663236 0.957905c16.284378 0 29.695042-12.45276 32.568756-28.737138V170.507016c0-22.98971 19.158092-42.147802 42.147802-42.147802h513.436857c10.53695 0 21.073901 3.831618 29.695042 12.45276 7.663237 7.663237 12.45276 18.200187 12.452759 29.695042v189.665107c0 17.242283 22.031805 28.737138 36.400375 28.737138s36.400374-12.45276 36.400374-28.737138V154.222638c0-26.821328-10.53695-51.726848-29.695042-70.884939s-44.063611-28.737138-69.927035-28.737138H243.478367c-54.600561 0-99.622077 45.021515-99.622077 100.579982v205.949485c0.957905 15.326473 15.326473 27.779233 31.610852 26.821328z" p-id="12793"></path><path d="M514.565364 525.889616c13.410664 0 36.400374-5.747428 36.400374-42.147801v-85.253508h75.674463c35.44247 0 41.189897-22.98971 41.189897-36.400374S662.08267 325.687558 626.640201 325.687558h-75.674463v-69.927034c0-36.400374-22.98971-42.147802-36.400374-42.147802s-36.400374 5.747428-36.400374 42.147802V325.687558h-75.674462c-35.44247 0-41.189897 22.98971-41.189897 36.400375s5.747428 36.400374 41.189897 36.400374h75.674462v85.253508c0 36.400374 22.98971 42.147802 36.400374 42.147801z" p-id="12794"></path></svg>
}

const menuFunctionIcon = () => {
    return <svg className='icon-function' viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="7571" width="20" height="20"><path d="M820.8 512c0 44.8 36 80.8 80.8 80.8s80.8-36 80.8-80.8-36-80.8-80.8-80.8-80.8 36-80.8 80.8zM431.2 512c0 44.8 36 80.8 80.8 80.8S592.8 556.8 592.8 512 556.8 431.2 512 431.2 431.2 467.2 431.2 512zM40.8 512c0 44.8 36 80.8 80.8 80.8S203.2 556.8 203.2 512s-36-80.8-80.8-80.8S41.6 467.2 40.8 512z" fill="" p-id="7572"></path></svg>
}

const Emoji = ({ symbol, label, onClick }: any) => (
    <span
        className="emoji-item"
        onClick={(e) => onClick(e)}
        role="img"
        aria-label={label ? label : ""}
        aria-hidden={label ? "false" : "true"}
    >
        {symbol}
    </span>
);

var positioningSize = 0;

export default class Content extends Component<IProps, IState> {
    private resizableRef: RefObject<HTMLDivElement>;

    private mentionRef = React.createRef<Mention>();

    menuFunction = () => {
        return <Tooltip
            content='功能'
            arrowPointAtCenter={false}
            position='bottom'
        ><Icon svg={menuFunctionIcon()} />
        </Tooltip>
    }

    invitaionFunction = () => {
        return <Tooltip
            content='邀请'
            arrowPointAtCenter={false}
            position='bottom'
        ><Icon style={{
            marginRight: '15px'
        }} onClick={() => this.invitation()} svg={invitationIcon()} />
        </Tooltip>
    }

    state: Readonly<IState> = {
        height: 270,
        custom: [],
        data: [],
        unread: 0,
        onLineUserLoading: false,
        page: 1,
        groupLoading: false,
        loading: false,
        groudUserPage: 1,
        groupinUsers: [],
        emojiKey: 0,
        updateState: false,
        users: [],
        addFriend: {
            visible: false,
            id: '',
            user: {} as GetUserDto
        },
        revertValue: undefined,
        groupinUser: {
            page: 1,
            pageSize: 50,
            loading: false
        }
    }


    constructor(props: IProps) {
        super(props);

        this.handleMouseDown = this.handleMouseDown.bind(this);
        this.download = this.download.bind(this);
        this.onScroll = this.onScroll.bind(this);
        this.rowRenderer = this.rowRenderer.bind(this);
        this.onScrollGroupInUser = this.onScrollGroupInUser.bind(this);
        this.loadingMessage = this.loadingMessage.bind(this);
        this.onNotification = this.onNotification.bind(this);
        this.resizableRef = React.createRef();
        this.loadingCustom()
        this.loadingGroupUser();

    }

    componentDidMount() {
        PubSub.subscribe('changeGroup', this.onMessage)
        PubSub.subscribe('Notification', this.onNotification)
        this.loadingGroupUser();
    }

    // 监听props变化
    componentWillReceiveProps(nextProps: any) {
        const { group } = nextProps;
        if (group.id !== this.props.group.id) {
            this.setState({
                data: [],
                groupinUser: {
                    page: 1,
                    pageSize: 50,
                    loading: false,
                },
                groupinUsers: [],
                page: 1,
                users: [],
                groudUserPage: 1,
            }, () => {
                this.loadingGroupUser();
            });
        }

    }

    getGroupInUser(groupId: string, page: number, pageSize: number) {

        const { groupinUser } = this.state;
        if (groupinUser.loading) {
            return
        }

        ChatService.getGroupInUser(groupId, page, pageSize)
            .then(async (res: any[]) => {
                if (res) {
                    if (res.length > 0) {

                        const { users, groupinUsers } = this.state;
                        const userids = res.map(x => x.userId)

                        // 这里是为了保证用户的基本信息完整
                        const userInfo = await GetUserInfos(userids)

                        for (let i = 0; i < userInfo.length; i++) {
                            // 如果不存在才添加
                            if (users.findIndex(y => y.id === userInfo[i].id) === -1) {
                                users.push(userInfo[i])
                            }

                        }

                        groupinUsers.push(...res)

                        this.setState({
                            groupinUsers,
                            users: users,
                            groupinUser: {
                                page: page + 1,
                                pageSize: pageSize,
                                loading: false,
                            }
                        })

                    }
                }
            })

    }

    loadingGroupUser() {
        const { group } = this.props;
        const { groupinUser } = this.state;

        if (group.group) {
            this.getGroupInUser(group.id, groupinUser.page, groupinUser.pageSize)
            this.loadingMessage();
        } else {
            GetUserInfos([group.creator, user.id])
                .then(userInfo => {
                    var userIds = [] as any[];
                    userIds.push({
                        userId: group.creator,
                        onLine: false,
                    });
                    this.setState({
                        groupinUsers: userIds,
                        users: userInfo
                    }, () => {
                        this.loadingMessage()
                    })
                })
        }
    }

    componentWillUnmount() {
        PubSub.unsubscribe('changeGroup');
        PubSub.unsubscribe('Notification');

        this.setState({
            updateState: false
        })
    }

    async onNotification(_: any, messageData: any) {

        if (messageData.type === "GroupUserNew") {
            debugger
            // 如果是当前用户的推送则忽略
            if (messageData.data === user.id) {
                return;
            }
            var users = await GetUserInfos([messageData.data])
            const { groupinUsers } = this.state;

            var index = groupinUsers.findIndex((x: any) => x.userId === messageData.data);
            if (index === -1) {
                this.setState({
                    groupinUsers: [{
                        userId: messageData.data,
                        onLine: true,
                    }, ...groupinUsers]
                })
            } else {
                this.setState({
                    groupinUsers: [{
                        userId: messageData.data,
                        onLine: true,
                    }, ...groupinUsers.filter((d, i) => i !== index || d.userId !== messageData.data)]
                })
            }
        }
        else if (messageData.type === "GroupInOffLine") {
            const { groupinUsers } = this.state;
            var index = groupinUsers.findIndex((x: any) => x.userId === messageData.data);
            if (index !== -1) {
                groupinUsers.splice(index, 1)
                groupinUsers.push({
                    userId: messageData.data,
                    onLine: false,
                })
            }

            this.setState({
                groupinUsers: [...groupinUsers]
            })
        }
        else if (messageData.type === "FriendRequest") {
            Notification.info({
                content: messageData.content
            })
        } else if (messageData.type === "System") {
            Notification.info({
                content: messageData.content
            })
        } else if (messageData.type === "GroupAppendUser") {
            this.loadingGroupUser();
        } else if (messageData.type === "Countermand") {
            debugger;
            const { data } = this.state;

            data.forEach(x => {
                if (x.id === messageData.data) {
                    x.countermand = true;
                }
            })

            this.setState({
                data: [...data]
            })
        }
    }

    onMessage = (_: any, data: any) => {
        const { group } = this.props;
        if (group.id === data.groupId) {
            this.setState({
                data: [...this.state.data, data]
            }, () => {
                this.scrollToBottom(false);
            })
        }
    }


    handleMouseDown = (e: any) => {
        const initialY = e.clientY;
        const initialHeight = this.state.height;

        const handleMouseMove = (event: MouseEvent) => {
            const newHeight = initialHeight - (event.clientY - initialY);
            if (newHeight >= 230 && newHeight <= 350) {
                this.setState({ height: newHeight });
            }
        };

        const handleMouseUp = () => {
            window.removeEventListener('mousemove', handleMouseMove);
            window.removeEventListener('mouseup', handleMouseUp);
        };

        window.addEventListener('mousemove', handleMouseMove);
        window.addEventListener('mouseup', handleMouseUp);
    };

    async sendMessage() {
        const { group } = this.props
        const { revertValue } = this.state;
        const value = this.mentionRef.current?.getValue();

        let revertId: null = null
        if (revertValue) {
            revertId = revertValue.id;
        }

        this.setState({
            revertValue: undefined
        })

        if (value && value.content.length > 0) {
            await ChatHubService.send('SendMessage', value.content, group.id, 0, revertId);

            this.mentionRef.current?.setValue('')
            for (let i = 0; i < value.base64.length; i++) {
                fileService.uploadBase64(value.base64[i], 'image.png')
                    .then((res: any) => {
                        if (res.code === '200') {
                            ChatHubService.send('SendMessage', res.data, group.id, 1, revertId);
                        }
                    })
            }
        } else if (value && value.base64.length > 0) {
            this.mentionRef.current?.setValue('')
            for (let i = 0; i < value.base64.length; i++) {
                fileService.uploadBase64(value.base64[i], 'image.png')
                    .then((res: any) => {
                        if (res.code === '200') {
                            ChatHubService.send('SendMessage', res.data, group.id, 1, revertId);
                        }
                    })
            }
        }
    }

    addEmoji(item: any) {
        emojiService.create(item.content)
            .then(res => {
                if (res.code === "200") {
                    Toast.success("添加成功")
                    this.loadingCustom();
                } else {
                    Toast.error(res.message)
                }
            })
    }

    /**
     * 传入时间是否小于当前时间的俩分钟
     * @param date 
     * @returns 
     */
    isTimestampLessThanTwoMinutes(timestamp: string) {
        var currentTime = new Date().getTime();
        var date = new Date(timestamp);
        var timestampValue = date.getTime();
        var timeDifference = currentTime - timestampValue;
        var timeDifferenceInMinutes = Math.floor(timeDifference / 1000 / 60);

        return timeDifferenceInMinutes < 2;
    }

    revert(item: any) {
        this.setState({
            revertValue: item
        })
    }

    countermand(item: any) {
        chatService.countermandMessage(item.id)
            .then(res => {
                if (res.code !== "200") {
                    Toast.error(res.message);
                }
            })
    }

    revertRender(item: any) {
        if (item.type === "Text" || item.type === 0) {
            return <RenderText content={item.content} />
        } else if (item.type === "Image" || item.type === 1) {
            return <div >
                <Image
                    width={'100%'}
                    style={{
                        width: 'auto',
                        height: 'auto',
                        marginBottom: '20px',
                        marginTop: '8px',
                        maxWidth: '45%',
                        borderRadius: '8px',
                        marginLeft: '10px'
                    }}
                    height={'100%'}
                    src={item.content + "?reduction=true"}
                />
            </div>
        } else if (item.type === "File" || item.type === 2) {
            return <Card
                className='message-item-content '
                style={{
                    width: 300, display: 'inline-block', marginBottom: '20px', marginLeft: '10px'
                }}
            >
                <span>
                    {item.content.substring(item.content.lastIndexOf("/") + 1)}
                </span>
                <Button onClick={() => this.download(item.content)} style={{
                    float: 'right'
                }}>下载</Button>
            </Card>
        } else if (item.type === "Video" || item.type === 3) {
            return <VideoPlayer float={null} url={item.content} />
        }
    }

    positioning(id: string) {
        id = id;
        const dom = document.getElementById("row_" + id);

        const container = document.getElementById('message-list');

        if (dom) {
            var domRect = dom.getBoundingClientRect();
            var containerRect = container!.getBoundingClientRect();

            // Check if the element is not fully visible
            if (domRect.top < containerRect.top || domRect.bottom > containerRect.bottom) {
                container!.scrollTop += domRect.top - containerRect.top;
            }

            positioningSize = 0;
        } else {
            container!.scrollTop = 0;

            if (positioningSize > 10) {
                Toast.success("定位超时！");
                positioningSize = 0;
                return;
            }

            positioningSize++;
            setTimeout(() => {
                this.positioning(id);
            }, 500);
        }
    }

    rendetContent = (item: any, u: any, iscurren: boolean) => {
        const className = user?.id === u?.id ? 'message-item-content-user' : '';

        const { users } = this.state;

        const float = iscurren ? "right" : '' as any;

        const revertUser = users.find(x => x.id === item?.revert?.userId)

        if (item.type === "Text" || item.type === 0) {
            return (
                <Tooltip style={{
                    backgroundColor: 'var(--message-item-content-background-color)'
                }} content={<div className='image-menu'>
                    <div className='image-menu-item' onClick={() => this.revert(item)}>回复</div>
                    {iscurren && <div className='image-menu-item' onClick={() => this.countermand(item)}>撤回</div>}
                </div>} position='rightTop' trigger="contextMenu" >
                    <div className={'message-item-content ' + className} style={{
                        display: 'inline-block', marginBottom: '20px', marginLeft: '10px', whiteSpace: 'pre-wrap',
                        float: float
                    }}>
                        {revertUser && <div className='revert-item'>
                            {revertUser?.name}
                            <Button theme='borderless' size='small' onClick={() => this.positioning(item.revert.id)} icon={<IconSmallTriangleTop />}></Button>
                            <div>
                                {this.revertRender(item.revert)}
                            </div>
                        </div>}
                        <RenderText content={item.content} />
                    </div>
                </Tooltip>
            )
        } else if (item.type === "Image" || item.type === 1) {
            return (
                <div >
                    <Tooltip style={{
                        backgroundColor: 'var(--message-item-content-background-color)'
                    }} content={<div className='image-menu'>
                        <div className='image-menu-item' onClick={() => this.addEmoji(item)}>添加到表情</div>
                        <div className='image-menu-item' onClick={() => this.revert(item)}>回复</div>
                        {iscurren && <div className='image-menu-item' onClick={() => this.countermand(item)}>撤回</div>}
                    </div>} position='rightTop' trigger="contextMenu" >
                        {revertUser ? <div style={{
                            width: 'auto',
                            height: '300px',
                            marginBottom: '20px',
                            marginTop: '8px',
                            maxWidth: '60%',
                            borderRadius: '8px',
                            marginLeft: '10px',
                            float: float,
                            padding: '8px',
                            backgroundColor: 'var(--message-item-content-background-color)'
                        }}>
                            {revertUser && <div style={{
                                float: float
                            }} className='revert-item'>
                                {revertUser?.name}
                                <Button theme='borderless' size='small' onClick={() => this.positioning(item.revert.id)} icon={<IconSmallTriangleTop />}></Button>
                                <div>
                                    {this.revertRender(item.revert)}
                                </div>
                            </div>}
                            <Image
                                width={'100%'}
                                className={className}
                                height={'100%'}
                                style={{
                                    maxHeight: '300px',
                                }}
                                src={item.content + "?reduction=true"}
                            />
                        </div> : <Image
                            width={'100%'}
                            className={className}
                            style={{
                                width: 'auto',
                                height: '300px',
                                marginBottom: '20px',
                                marginTop: '8px',
                                maxWidth: '60%',
                                borderRadius: '8px',
                                marginLeft: '10px',
                                float: float
                            }}
                            height={'100%'}
                            src={item.content + "?reduction=true"}
                        />}

                    </Tooltip>
                </div>
            )
        } else if (item.type === "File" || item.type === 2) {
            return (
                <Tooltip style={{
                    backgroundColor: 'var(--message-item-content-background-color)'
                }} content={<div className='image-menu'>
                    <div className='image-menu-item' onClick={() => this.revert(item)}>回复</div>
                    {iscurren && <div className='image-menu-item' onClick={() => this.countermand(item)}>撤回</div>}
                </div>} position='rightTop' trigger="contextMenu" >
                    <Card
                        className='message-item-content '
                        style={{
                            width: 300, display: 'inline-block', marginBottom: '20px', marginLeft: '10px',
                            float: float
                        }}
                    >
                        <span>
                            {item.content.substring(item.content.lastIndexOf("/") + 1)}
                        </span>
                        <Button onClick={() => this.download(item.content)} style={{
                            float: 'right'
                        }}>下载</Button>
                    </Card>
                </Tooltip>
            )
        } else if (item.type === "Video" || item.type === 3) {
            return (
                <Tooltip style={{
                    backgroundColor: 'var(--message-item-content-background-color)'
                }} content={<div className='image-menu'>
                    {iscurren && <div className='image-menu-item' onClick={() => this.countermand(item)}>撤回</div>}
                </div>} position='rightTop' trigger='hover'>
                    <VideoPlayer float={float} url={item.content} />
                </Tooltip>
            )
        }
    }

    formatTime(dateStr: string) {
        var now = new Date();
        var date = new Date(dateStr);
        var diff = now.getTime() - date.getTime();

        // 超过一天，显示完整时间
        if (diff >= 24 * 60 * 60 * 1000) {
            return moment(dateStr).format('YYYY-MM-DD HH:mm:ss');
        }

        var hour = date.getHours();
        var minute = date.getMinutes()
        var ampm = hour >= 12 ? '下午' : '上午';
        hour = hour % 12;
        hour = hour ? hour : 12;

        return ampm + ' ' + hour + ':' + minute;
    }

    isSameDateTime(str1: string, str2: string) {
        // 将字符串转换为日期对象
        var date1 = new Date(str1);
        var date2 = new Date(str2);

        // 获取年、月、日、小时和分钟
        var year1 = date1.getFullYear();
        var month1 = date1.getMonth();
        var day1 = date1.getDate();
        var hour1 = date1.getHours();
        var minute1 = date1.getMinutes();

        var year2 = date2.getFullYear();
        var month2 = date2.getMonth();
        var day2 = date2.getDate();
        var hour2 = date2.getHours();
        var minute2 = date2.getMinutes();

        // 判断年、月、日、小时是否相同
        if (year1 === year2 && month1 === month2 && day1 === day2 && hour1 === hour2) {
            return true;
        } else {
            return false;
        }
    }

    rowRenderer(item: any, index: number) {
        try {
            const { data, users } = this.state;
            let date = null;
            if (index === 0 || !this.isSameDateTime(data[index - 1].creationTime, item.creationTime)) {
                date = this.formatTime(item.creationTime)
            }

            let userItem = users.find(x => x.id === item.userId)

            if (item.countermand) {
                return (<div style={{
                    textAlign: 'center',
                    marginTop: '8px',
                    marginBottom: '8px'
                }}>
                    {userItem?.name + "撤回一条消息。"}
                </div>)
            }

            item.user = userItem

            // 判断是否为当前用户
            const iscurren = item?.userId === user?.id;

            if (iscurren) {
                return <>
                    {date && <div style={{
                        textAlign: 'center'
                    }}>{date}</div>}
                    <div style={{
                        float: 'right',
                        width: '100%',
                    }} id={"row_" + item.id}>
                        <Tooltip position='right' content={() => this.renderInfo(userItem)} trigger="click" >
                            <Avatar size='small' style={{ float: 'right' }} src={userItem?.avatar + "?reduction=true"} />
                        </Tooltip>
                        <div style={{ paddingRight: '10px', float: 'right', width: "calc(100% - 50px)", textAlign: 'end' }}>
                            {userItem?.name}
                        </div>
                        {this.rendetContent(item, userItem, iscurren)}
                    </div>
                </>
            } else {
                return <> {date && <div style={{
                    textAlign: 'center'
                }}>{date}</div>}
                    <div style={{
                        margin: '8px',
                    }} id={"row_" + item.id}>
                        <Tooltip position='right' content={() => this.renderInfo(userItem)} trigger="click" >
                            <Avatar size='small' style={{ float: 'left' }} src={userItem?.avatar + "?reduction=true"} />
                        </Tooltip>
                        <div style={{ paddingLeft: '40px', width: 'calc(100% - 50px)' }}>
                            {userItem?.name}
                        </div>
                        {this.rendetContent(item, userItem, iscurren)}
                    </div>
                </>
            }
        } catch (error) {
            console.log(error);
        }
    }

    /**
     * 加载记录
     */
    loadingMessage() {
        const { group } = this.props;
        const { page, users } = this.state;
        ChatService.getList(group.id, page, 20)
            .then(async (res: any) => {
                if (res.code === "200") {

                    var userids = res.data.result.map((x: any) => x.userId)
                    var userinfos = await GetUserInfos(userids);
                    userinfos.forEach(x => {
                        // 如果不存在才添加
                        if (users.findIndex(y => y.id === x.id) === -1) {
                            users.push(x)
                        }
                    })

                    this.setState({
                        data: res.data.result,
                        users: users
                    }, () => {
                        this.scrollToBottom(true);
                    })
                }
            })

    }

    onScroll(_: any) {
        var element = document.getElementById('message-list')!;
        const { data } = this.state;
        if (element.scrollTop === 0 && data.length !== 0) {

            const { group } = this.props;
            const { page } = this.state;

            this.setState({
                loading: true,
            })

            const height = element.scrollHeight;

            ChatService.getList(group.id, page + 1, 20)
                .then((res: any) => {
                    if (res.data.result.length === 0) {
                        return;
                    }
                    this.setState({
                        data: [...res.data.result, ...this.state.data],
                        page: page + 1,
                    }, () => {
                        setTimeout(() => {
                            const newHeight = element.scrollHeight;
                            if (height !== newHeight) {
                                // 移动当之前定位
                                element.scrollTop = newHeight - height
                            }
                        }, 10);
                    })
                }).finally(() => {

                    this.setState({
                        loading: false,
                    })
                })
        }
    }

    /**
     * 下载文件
     * @param url 
     */
    download(url: string) {
        var a = document.createElement('a');
        a.href = url;
        a.target = '_blank'
        a.download = url.split('/')[url.split('/').length - 1];
        a.click();
    }

    /**
     * 慢慢移动到尾部
     */
    scrollToBottom(value: boolean) {
        var element = document.getElementById('message-list')!;
        var scrollHeight = element.scrollHeight;
        var scrollTop = element.scrollTop;

        var scrollCount = 0;
        var scrollMargin;

        function scroll() {
            // 计算滚动条，当滚动条位置位于最下面的高度的百分之三十则自动滚动。
            if ((element.scrollTop + element.clientHeight + (element.scrollHeight * 0.3) > element.scrollHeight) || value) {
                scrollMargin = scrollHeight - scrollTop;
                element.scrollTop += scrollMargin / 20;
                scrollCount++;
                if (scrollCount < 30) {
                    requestAnimationFrame(scroll);
                }
            }
        }

        scroll();
    }

    selectPicture() {
        var input = document.createElement('input');
        input.type = 'file';
        input.accept = 'image/png, image/jpeg, image/gif';
        input.multiple = false;
        input.click();
        input.onchange = (e: any) => {
            var files = e.target.files;
            // 判断文件是否图片
            if (files[0].type.indexOf('image') === -1) {
                Notification.warning({
                    title: '提示',
                    content: '请选择图片',
                    duration: 1000,
                });
                return;
            }
            // 将文件放到form
            var formData = new FormData();
            formData.append('file', files[0]);
            // 上传图片
            fileService.upload(formData)
                .then((res: any) => {
                    if (res.code === '200') {
                        ChatHubService.send('SendMessage', res.data, this.props.group.id, 1, null);
                    }
                })

        }
    }

    /**
     * 选择上传文件
     */
    selectFile() {
        var input = document.createElement('input');
        input.type = 'file';
        input.multiple = false;
        input.click();
        input.onchange = (e: any) => {
            var files = e.target.files;

            // 默认 2 文件
            let type = 2;

            const file = files[0] as File
            if (this.isVideo(file.name)) {
                type = 3; // 视频
            }
            // 将文件放到form
            var formData = new FormData();
            formData.append('file', files[0]);
            // 上传图片
            fileService.upload(formData)
                .then((res: any) => {
                    if (res.code === '200') {
                        ChatHubService.send('SendMessage', res.data, this.props.group.id, type, null);
                    }
                })

        }
    }

    /**
     * 是否视频
     * @param name 
     * @returns 
     */
    isVideo(name: string) {
        var videoExtensions = /\.(mp4|mov|avi|mkv|flv)$/i;
        return videoExtensions.test(name);
    }

    /**
     * 邀请点击
     */
    invitation() {
        const { group } = this.props;
        if (group.group) {
            const url = window.location.origin + "/invitation-group?code=" + group.id;
            copy(url)
            Toast.success('邀请地址已经复制');
        } else {
            Toast.error('暂未实现功能')
        }
    }

    async clickFriends(id: string) {

        var isfriend = await FriendService.FriendState(id);

        if (isfriend.data) {
            Notification.error({
                content: "已经是好友关系"
            })
            return;
        }

        var user = (await GetUserInfos([id]))[0]

        this.setState({
            addFriend: {
                visible: true,
                id: id,
                user
            }
        })
    }

    /**
     * 渲染用户基本信息
     * @param dto 
     * @returns 
     */
    renderInfo(dto: GetUserDto | undefined) {

        // 判断是否为当前用户或智能助手
        const iscurren = dto?.id === user.id || dto?.id === "00000000-0000-0000-0000-000000000000";

        return (<>
            <div style={{
                width: "320px",
                height: '110px',
            }}>
                <Avatar size="large" src={dto?.avatar + "?reduction=true"} style={{ margin: 4, float: 'left' }} alt='User'>
                </Avatar>
                <div style={{
                    float: 'left',
                    margin: "5px"
                }}>
                    <div style={{
                        fontSize: '20px',
                        fontWeight: 'bold',
                        color: "red",
                        marginBottom: '10px',
                        width: "120px"
                    }}>
                        昵称：{dto?.name}
                    </div>
                    <div style={{
                        marginBottom: '10px',
                        width: "120px"
                    }}>
                        账号：{dto?.account}
                    </div>
                </div>
            </div>
            <div style={{
                padding: '20px',
            }}>
                {!iscurren ? <><Button block style={{
                    marginBottom: '5px'
                }}>联系</Button>
                    <Button onClick={async () => await this.clickFriends(dto!.id)} block>添加好友</Button></> : <></>}
            </div></>)
    }

    onScrollGroupInUser() {

        var element = document.getElementById('group-in-user')!;

        if (element.scrollTop + element.clientHeight === element.scrollHeight) {

            const { groupinUser } = this.state;

            if (groupinUser.loading) {
                return;
            }

            this.setState({
                groupinUser: {
                    ...groupinUser,
                    loading: true
                }
            })

            this.getGroupInUser(this.props.group.id, groupinUser.page + 1, groupinUser.pageSize)

        }
    }

    async emojiClick(v: any) {
        const { group } = this.props
        await ChatHubService.send('SendMessage', v, group.id, 0, null);
    }

    renderDefault() {
        return (<div style={{
            height: "100%",
            overflow: 'auto'
        }}>
            {emoji.map((x, index) => {
                return <Emoji key={'emoji_' + index} onClick={() => this.emojiClick(x)} symbol={x} label="smile" />
            })}
        </div>)
    }

    loadingCustom() {
        emojiService.get()
            .then(res => {
                if (res.code === '200') {
                    this.setState({
                        custom: res.data
                    })
                }
            })
    }

    uploadEmoji() {
        var input = document.createElement('input');
        input.type = 'file';
        input.accept = 'image/png, image/jpeg, image/gif';
        input.multiple = false;
        input.click();
        input.onchange = (e: any) => {
            var files = e.target.files;
            // 判断文件是否图片
            if (files[0].type.indexOf('image') === -1) {
                Notification.warning({
                    title: '提示',
                    content: '请选择图片',
                    duration: 1000,
                });
                return;
            }
            // 将文件放到form
            var formData = new FormData();
            formData.append('file', files[0]);
            // 上传图片
            fileService.upload(formData)
                .then((res: any) => {
                    if (res.code === '200') {
                        emojiService.create(res.data)
                            .then(res => {
                                if (res.code === "200") {
                                    Toast.success("添加成功")
                                    this.loadingCustom();
                                } else {
                                    Toast.error(res.message)
                                }
                            })
                    }
                })

        }
    }

    async sendEmoji(item: any) {
        const { group } = this.props
        await ChatHubService.send('SendMessage', item.path, group.id, 1, null);
    }

    removeEmoji(item: string) {
        emojiService.delete(item)
            .then((res) => {
                if (res.code === '200') {
                    Toast.success("删除成功")
                    this.loadingCustom();
                } else {
                    Toast.error(res.message)
                }
            })
    }

    renderCustom() {
        const { custom } = this.state;
        return (<div style={{
            height: "100%",
            overflow: 'auto'
        }}>
            {custom.map(x => {
                return <Tooltip style={{
                    backgroundColor: 'var(--message-item-content-background-color)'
                }} content={<div className='image-menu'>
                    <div className='image-menu-item' onClick={() => this.removeEmoji(x.id)}>删除表情包</div>
                </div>} position='rightTop' trigger="contextMenu" >
                    <div style={{
                        float: 'left',
                        margin: '5px',
                        cursor: 'pointer',
                    }} onClick={() => this.sendEmoji(x)}>
                        <Image
                            width={50}
                            height={50}
                            src={x.path}
                            preview={false}
                        />
                    </div>
                </Tooltip>

            })}
            <Button size='large' style={{
                margin: '10px',
                float: 'left'
            }} theme='borderless' onClick={() => this.uploadEmoji()} icon={<IconPlus />}></Button>
        </div>)
    }

    renderEmoji() {
        const { emojiKey } = this.state;

        let render = null;

        if (emojiKey === 0) {
            render = this.renderDefault()
        } else if (emojiKey === 1) {
            render = this.renderCustom()
        }

        return (<div className='emoji'>
            <div style={{
                height: "calc(100% - 50px)"
            }}>
                {render}
            </div>
            <div style={{
                height: '50px'
            }}>
                <div onClick={() => this.setState({ emojiKey: 0 })} className={'emoji-menu ' + (emojiKey === 0 ? "emoji-select" : "")}>
                    <Icon style={{
                        margin: '10px',
                    }} svg={
                        <svg viewBox="0 0 1025 1024" width="20" height="20"><path d="M512.016 1024C229.232 1024 0.016 794.784 0.016 512 0.016 229.216 229.232 0 512.016 0 794.784 0 1024 229.216 1024 512 1024 794.784 794.784 1024 512.016 1024ZM512.016 64C264.976 64 64.016 264.96 64.016 512 64.016 759.024 264.976 960 512.016 960 759.04 960 960 759.024 960 512 960 264.96 759.04 64 512.016 64ZM510.336 833.456C509.056 833.456 507.744 833.488 506.448 833.488 310.992 833.488 229.024 657.12 225.616 649.552 218.336 633.424 225.488 614.496 241.584 607.216 257.712 599.968 276.576 607.088 283.888 623.088 286.64 629.12 352.928 769.488 506.576 769.488 507.584 769.488 508.576 769.456 509.584 769.456 672.896 767.552 738.368 624.768 739.024 623.344 746.176 607.216 765.024 599.872 781.264 607.152 797.392 614.336 804.672 633.248 797.456 649.408 794.176 656.8 714.208 831.056 510.336 833.456ZM671.504 479.84C636.224 479.84 607.664 451.232 607.664 415.984 607.664 380.768 636.224 352.176 671.504 352.176 706.768 352.176 735.344 380.768 735.344 415.984 735.344 451.232 706.768 479.84 671.504 479.84ZM351.504 479.84C316.224 479.84 287.664 451.232 287.664 415.984 287.664 380.768 316.224 352.176 351.504 352.176 386.768 352.176 415.344 380.768 415.344 415.984 415.344 451.232 386.768 479.84 351.504 479.84Z" p-id="2484"></path></svg>
                    } />
                </div>
                <div onClick={() => this.setState({ emojiKey: 1 })} className={'emoji-menu ' + (emojiKey === 1 ? "emoji-select" : "")} >
                    <Icon style={{
                        margin: '10px',
                    }} svg={
                        <svg viewBox="0 0 1025 1024" width="20" height="20"><path d="M667.786667 117.333333C832.864 117.333333 938.666667 249.706667 938.666667 427.861333c0 138.250667-125.098667 290.506667-371.573334 461.589334a96.768 96.768 0 0 1-110.186666 0C210.432 718.368 85.333333 566.112 85.333333 427.861333 85.333333 249.706667 191.136 117.333333 356.213333 117.333333c59.616 0 100.053333 20.832 155.786667 68.096C567.744 138.176 608.170667 117.333333 667.786667 117.333333z m0 63.146667c-41.44 0-70.261333 15.189333-116.96 55.04-2.165333 1.845333-14.4 12.373333-17.941334 15.381333a32.32 32.32 0 0 1-41.770666 0c-3.541333-3.018667-15.776-13.536-17.941334-15.381333-46.698667-39.850667-75.52-55.04-116.96-55.04C230.186667 180.48 149.333333 281.258667 149.333333 426.698667 149.333333 537.6 262.858667 675.242667 493.632 834.826667a32.352 32.352 0 0 0 36.736 0C761.141333 675.253333 874.666667 537.6 874.666667 426.698667c0-145.44-80.853333-246.218667-206.88-246.218667z" fill="#000000" p-id="3586"></path></svg>
                    } />
                </div>
            </div>
        </div>)
    }

    addFriends(v: FriendRegistrationInput) {
        const { addFriend } = this.state;
        v.beAppliedForId = addFriend.id;
        FriendService.FriendRegistration(v)
            .then(res => {
                if (res.code === '200') {
                    Notification.success({
                        content: "添加成功"
                    });

                    this.setState({
                        addFriend: {
                            visible: false,
                            id: '',
                            user: {} as GetUserDto
                        }
                    })
                } else {
                    Notification.error({
                        content: res.message
                    });
                    this.setState({
                        addFriend: {
                            visible: false,
                            id: '',
                            user: {} as GetUserDto
                        }
                    })
                }
            })
    }

    render() {
        const { groupinUsers, users, addFriend, data, height, revertValue } = this.state;
        const { group } = this.props;

        return (
            <>
                <div className="content-header ">
                    <div
                        style={{
                            fontSize: '20px',
                            fontWeight: 'bold',
                            paddingTop: '25px',
                            float: 'left',
                            paddingLeft: '10px',
                        }}
                    >
                        {this.props.group.name}
                    </div>
                    <div style={{
                        float: 'right',
                        paddingTop: '25px',
                        paddingRight: '10px',
                    }}>
                        {this.invitaionFunction()}
                        {this.menuFunction()}
                    </div>
                </div>
                <div className="content-divider">
                </div>
                <div className="main-content" style={{
                    float: 'left',
                    width: 'calc(100% - 180px)',
                }}>
                    <div className="content-box " style={{ flexBasis: `calc(100% - ${height}px - 10px)`, }}>
                        <div onScroll={this.onScroll} id='message-list' style={{
                            height: '100%',
                            overflow: 'auto',
                            maxHeight: `calc((100vh - ${height}px))`
                        }}>
                            {data.map((x, i) => {
                                return (<div key={"ListComponent_" + i}>
                                    {this.rowRenderer(x, i)}
                                </div>)
                            })}
                        </div>
                    </div>
                    <div className="draggable-line " onMouseDown={this.handleMouseDown}></div>
                    <div
                        className="resizable-box slide-in-bottom"
                        ref={this.resizableRef}
                        style={{ height: `${height}px` }}
                    >
                        <div className='content-function'>
                            <Popover position='top' content={this.renderEmoji()} trigger="click">
                                <Icon style={{
                                    float: 'left',
                                }} svg={
                                    <svg className='icon-function' viewBox="0 0 1025 1024" width="20" height="20"><path d="M512.016 1024C229.232 1024 0.016 794.784 0.016 512 0.016 229.216 229.232 0 512.016 0 794.784 0 1024 229.216 1024 512 1024 794.784 794.784 1024 512.016 1024ZM512.016 64C264.976 64 64.016 264.96 64.016 512 64.016 759.024 264.976 960 512.016 960 759.04 960 960 759.024 960 512 960 264.96 759.04 64 512.016 64ZM510.336 833.456C509.056 833.456 507.744 833.488 506.448 833.488 310.992 833.488 229.024 657.12 225.616 649.552 218.336 633.424 225.488 614.496 241.584 607.216 257.712 599.968 276.576 607.088 283.888 623.088 286.64 629.12 352.928 769.488 506.576 769.488 507.584 769.488 508.576 769.456 509.584 769.456 672.896 767.552 738.368 624.768 739.024 623.344 746.176 607.216 765.024 599.872 781.264 607.152 797.392 614.336 804.672 633.248 797.456 649.408 794.176 656.8 714.208 831.056 510.336 833.456ZM671.504 479.84C636.224 479.84 607.664 451.232 607.664 415.984 607.664 380.768 636.224 352.176 671.504 352.176 706.768 352.176 735.344 380.768 735.344 415.984 735.344 451.232 706.768 479.84 671.504 479.84ZM351.504 479.84C316.224 479.84 287.664 451.232 287.664 415.984 287.664 380.768 316.224 352.176 351.504 352.176 386.768 352.176 415.344 380.768 415.344 415.984 415.344 451.232 386.768 479.84 351.504 479.84Z" p-id="2484"></path></svg>
                                } />
                            </Popover>

                            <Icon style={{
                                float: 'left',
                                marginLeft: '15px',
                            }} svg={
                                <svg className='icon-function' viewBox="0 0 1025 1024" width="20" height="20"><path d="M437.76 430.08L170.496 79.36C156.672 61.44 159.232 35.84 176.64 20.48c16.896-14.848 42.496-12.8 56.832 4.096L512 344.576l278.528-320c14.848-16.896 39.936-18.432 56.832-4.096 17.408 14.848 19.968 40.448 6.144 58.88L586.24 430.08l165.888 190.976c92.672-33.792 196.096 4.096 245.248 89.6 49.152 85.504 29.184 194.048-47.104 256.512-76.288 62.464-186.368 61.44-260.608-3.072-74.752-64.512-92.16-173.056-40.96-257.536-1.536-1.536-3.072-3.584-4.096-5.12L512 527.872 437.76 430.08zM383.488 492.544l77.824 101.888L379.904 701.44c-1.536 1.536-2.56 3.584-4.096 5.12 50.688 84.48 33.792 193.024-40.96 257.536-74.752 64.512-184.832 65.536-260.608 3.072-76.288-62.464-95.744-171.008-47.104-256.512 49.152-85.504 152.576-123.392 245.248-89.6l111.104-128.512zM215.04 931.84c44.032-3.584 82.432-30.72 100.352-70.656 17.92-39.936 13.312-86.528-12.8-122.368-26.112-35.328-69.12-53.76-112.64-48.64-65.536 8.192-112.64 67.584-105.472 133.12 6.656 66.048 64.512 114.176 130.56 108.544z m593.92 0c43.52 5.632 86.528-13.312 112.64-48.64 26.112-35.328 30.72-81.92 12.8-121.856-17.92-39.936-56.32-67.072-100.352-70.656-66.048-5.632-124.416 42.496-131.072 108.032-6.656 65.536 40.448 124.928 105.984 133.12z m0 0" p-id="3601"></path></svg>
                            } />
                            <Icon onClick={() => this.selectFile()} style={{
                                float: 'left',
                                marginLeft: '15px',
                            }} svg={
                                <svg className='icon-function' viewBox="0 0 1025 1024" width="20" height="20"><path d="M912 208H427.872l-50.368-94.176A63.936 63.936 0 0 0 321.056 80H112c-35.296 0-64 28.704-64 64v736c0 35.296 28.704 64 64 64h800c35.296 0 64-28.704 64-64v-608c0-35.296-28.704-64-64-64z m-800-64h209.056l68.448 128H912v97.984c-0.416 0-0.8-0.128-1.216-0.128H113.248c-0.416 0-0.8 0.128-1.248 0.128V144z m0 736v-96l1.248-350.144 798.752 1.216V784h0.064v96H112z" p-id="4942"></path></svg>
                            } />
                            <Icon onClick={() => this.selectPicture()} style={{
                                float: 'left',
                                marginLeft: '15px',
                            }} svg={
                                <svg className='icon-function' viewBox="0 0 1025 1024" width="20" height="20"><path d="M938.666667 553.92V768c0 64.8-52.533333 117.333333-117.333334 117.333333H202.666667c-64.8 0-117.333333-52.533333-117.333334-117.333333V256c0-64.8 52.533333-117.333333 117.333334-117.333333h618.666666c64.8 0 117.333333 52.533333 117.333334 117.333333v297.92z m-64-74.624V256a53.333333 53.333333 0 0 0-53.333334-53.333333H202.666667a53.333333 53.333333 0 0 0-53.333334 53.333333v344.48A290.090667 290.090667 0 0 1 192 597.333333a286.88 286.88 0 0 1 183.296 65.845334C427.029333 528.384 556.906667 437.333333 704 437.333333c65.706667 0 126.997333 16.778667 170.666667 41.962667z m0 82.24c-5.333333-8.32-21.130667-21.653333-43.648-32.917333C796.768 511.488 753.045333 501.333333 704 501.333333c-121.770667 0-229.130667 76.266667-270.432 188.693334-2.730667 7.445333-7.402667 20.32-13.994667 38.581333-7.68 21.301333-34.453333 28.106667-51.370666 13.056-16.437333-14.634667-28.554667-25.066667-36.138667-31.146667A222.890667 222.890667 0 0 0 192 661.333333c-14.464 0-28.725333 1.365333-42.666667 4.053334V768a53.333333 53.333333 0 0 0 53.333334 53.333333h618.666666a53.333333 53.333333 0 0 0 53.333334-53.333333V561.525333zM320 480a96 96 0 1 1 0-192 96 96 0 0 1 0 192z m0-64a32 32 0 1 0 0-64 32 32 0 0 0 0 64z" p-id="5947"></path></svg>
                            } />
                            <Icon style={{
                                float: 'right',
                                right: '35px',
                                position: 'relative',
                            }} svg={
                                <svg className='icon-function' viewBox="0 0 1025 1024" width="20" height="20"><path d="M508.135996 823.23551c-169.794128 0-308.717154-138.923026-308.717154-308.717154s138.923026-308.717154 308.717154-308.717154 308.717154 138.923026 308.717154 308.717154S677.930123 823.23551 508.135996 823.23551zM508.135996 128.621403c-212.242915 0-385.895931 173.653015-385.895931 385.895931s173.653015 385.895931 385.895931 385.895931 385.895931-173.653015 385.895931-385.895931S720.378911 128.621403 508.135996 128.621403z" p-id="6977"></path><path d="M527.431457 321.56988 469.546096 321.56988 469.546096 553.107233 670.212348 676.594709 701.084473 626.427122 527.431457 522.236132Z" p-id="6978"></path></svg>
                            } />

                        </div>
                        <Mention onCloseRevert={() => this.setState({
                            revertValue: undefined
                        })} revertValue={revertValue} ref={this.mentionRef} style={{
                            height: revertValue ? 'calc(100% - 238px)' : 'calc(100% - 160px)',
                        }} onSubmit={async () => await this.sendMessage()} />
                        <div style={{
                            float: 'right',
                        }}>
                            <Button onClick={async () => await this.sendMessage()} style={{
                                backgroundColor: '#1472D0',
                                color: 'var(--semi-color-text-0)',
                                borderRadius: '4px',
                                marginRight: '20px',
                            }}>发送</Button>
                        </div>
                    </div>
                </div>
                <div style={{
                    float: 'left',
                    width: '1px',
                    height: '100%',
                    backgroundColor: 'var(--verticle-division-color)',
                }} className='verticle-division'></div>
                <div style={{
                    float: 'left',
                    width: '179px',
                    height: '100%'
                }}>
                    <div style={{
                        flexBasis: '100%',
                        flexShrink: 0,
                        height: 'calc(100% - 67px)',
                        borderBottom: '1px solid var(--semi-color-border)',
                        overflow: 'auto',
                    }}
                        id='group-in-user'
                        onScroll={this.onScrollGroupInUser}
                        className='user-group'>
                        <FlipMove
                            staggerDurationBy="30"
                            enterAnimation='accordionHorizontal'
                            appearAnimation='accordionHorizontal'
                            duration={500}>

                            {groupinUsers.map((item, index) => {
                                const user = users.find(x => x.id == item.userId)
                                return (
                                    <div key={"test" + index} className='grou-user-item  '>
                                        <div className='grou-user-item-content'>
                                            <div style={{
                                                float: 'left'
                                            }}>
                                                <Tooltip position='leftTop' content={() => this.renderInfo(user)} trigger="click" >
                                                    {item?.onLine ? <Badge dot >
                                                        <Avatar size='extra-small' src={user?.avatar + "?reduction=true"} />
                                                    </Badge> :
                                                        <Avatar size='extra-small' src={user?.avatar + "?reduction=true"} />}
                                                </Tooltip>
                                            </div>
                                            <div style={{
                                                marginLeft: '10px',
                                                userSelect: 'none',
                                                fontSize: '14px',
                                                width: "70px",
                                                float: 'left',
                                                whiteSpace: 'nowrap',
                                                overflow: 'hidden',
                                                textOverflow: 'ellipsis'
                                            }}>
                                                {user?.name}
                                            </div>
                                            {(user?.id === '00000000-0000-0000-0000-000000000000' || !user?.id) ?
                                                <Tag style={{
                                                    boxSizing: 'content-box',
                                                    float: 'right',
                                                }} color="blue" >机器人{JSON.stringify(user)}</Tag> : (
                                                    user?.id === group.creator ?
                                                        <Tag style={{
                                                            boxSizing: 'content-box',
                                                            float: 'right',
                                                        }} color='red'>频道主</Tag> :
                                                        <Tag style={{
                                                            boxSizing: 'content-box',
                                                            float: 'right',
                                                        }} color="grey">成员</Tag>

                                                )}
                                        </div>
                                    </div>)
                            })}
                        </FlipMove>
                    </div>
                </div>
                <Modal
                    title="添加好友"
                    visible={addFriend.visible}
                    onOk={() => this.setState({
                        addFriend: {
                            visible: false,
                            id: '',
                            user: {} as GetUserDto
                        }
                    })}
                    onCancel={() => this.setState({
                        addFriend: {
                            visible: false,
                            id: '',
                            user: {} as GetUserDto
                        }
                    })}
                    closeOnEsc={true}
                >
                    <Form onSubmit={(v) => this.addFriends(v)}>
                        <Avatar style={{
                            marginLeft: '45%',
                        }} src={addFriend.user.avatar + "?reduction=true"} />
                        <div style={{
                            textAlign: 'center',

                        }}>
                            {addFriend.user.name}
                        </div>
                        <div>
                            <Form.TextArea field='description' label='描述' placeholder='描述' style={{
                                margin: '8px'
                            }} />
                            <Button style={{
                                margin: '8px'
                            }} htmlType='submit' type='secondary' block>添加好友</Button>
                        </div>
                    </Form>

                </Modal>
            </>
        );
    }
}
