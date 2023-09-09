import React, { Component, RefObject } from 'react';
import { ChatGroupDto } from '../../dto';

import moment from 'moment/moment';
import { Avatar, Input, List as SList, Button, Card, Icon, Image, Tag, Notification, Toast, Badge, Tooltip, Spin, List, Popover } from '@douyinfe/semi-ui';
import './index.scss';
import Mention from '../Mention';
import { IconSearch, IconLoading } from '@douyinfe/semi-icons';
import ChatHubService from '../../services/chatHubService';
import fileService from '../../services/fileService';
import PubSub from 'pubsub-js';
import copy from 'copy-to-clipboard';
import ChatService from '../../services/chatService';
import chatService from '../../services/chatService';

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
    groudUserPage: number,
    groupLoading: boolean,
}

const user = JSON.parse(localStorage.getItem('user') || '{}');

const invitationIcon = () => {
    return <svg className='icon-function' viewBox="0 0 1029 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="12791" width="20" height="20"><path d="M991.601847 386.035547c-22.031805-13.410664-49.811038-13.410664-71.842844 0l-347.719364 229.8971c-36.400374 24.905519-84.295603 24.905519-120.695977 0L107.455916 388.909261c-22.031805-14.368569-49.811038-16.284378-71.842844-3.831618C12.623362 396.572498-1.745206 421.478017 0.170603 447.341441V871.693171c0 58.43218 47.895229 106.327409 105.369504 106.327409h819.008419c27.779233 0 53.642657-10.53695 73.758653-30.652947 20.115996-20.115996 31.610851-46.937325 31.610851-74.716557V447.341441c0-24.905519-14.368569-49.811038-38.316183-61.305894z m-31.610851 70.884939v402.319926c0 22.031805-18.200187 49.811038-38.316184 49.811038H122.78239c-22.031805 0-55.558466-29.695042-55.558466-49.811038v-402.319926l347.719364 230.855005c59.390084 38.316183 136.022451 38.316183 194.45463 0l350.593078-230.855005z m66.095416 415.73059z" p-id="12792"></path><path d="M175.467142 387.951356l7.663236 0.957905c16.284378 0 29.695042-12.45276 32.568756-28.737138V170.507016c0-22.98971 19.158092-42.147802 42.147802-42.147802h513.436857c10.53695 0 21.073901 3.831618 29.695042 12.45276 7.663237 7.663237 12.45276 18.200187 12.452759 29.695042v189.665107c0 17.242283 22.031805 28.737138 36.400375 28.737138s36.400374-12.45276 36.400374-28.737138V154.222638c0-26.821328-10.53695-51.726848-29.695042-70.884939s-44.063611-28.737138-69.927035-28.737138H243.478367c-54.600561 0-99.622077 45.021515-99.622077 100.579982v205.949485c0.957905 15.326473 15.326473 27.779233 31.610852 26.821328z" p-id="12793"></path><path d="M514.565364 525.889616c13.410664 0 36.400374-5.747428 36.400374-42.147801v-85.253508h75.674463c35.44247 0 41.189897-22.98971 41.189897-36.400374S662.08267 325.687558 626.640201 325.687558h-75.674463v-69.927034c0-36.400374-22.98971-42.147802-36.400374-42.147802s-36.400374 5.747428-36.400374 42.147802V325.687558h-75.674462c-35.44247 0-41.189897 22.98971-41.189897 36.400375s5.747428 36.400374 41.189897 36.400374h75.674462v85.253508c0 36.400374 22.98971 42.147802 36.400374 42.147801z" p-id="12794"></path></svg>
}

const menuFunctionIcon = () => {
    return <svg className='icon-function' viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="7571" width="20" height="20"><path d="M820.8 512c0 44.8 36 80.8 80.8 80.8s80.8-36 80.8-80.8-36-80.8-80.8-80.8-80.8 36-80.8 80.8zM431.2 512c0 44.8 36 80.8 80.8 80.8S592.8 556.8 592.8 512 556.8 431.2 512 431.2 431.2 467.2 431.2 512zM40.8 512c0 44.8 36 80.8 80.8 80.8S203.2 556.8 203.2 512s-36-80.8-80.8-80.8S41.6 467.2 40.8 512z" fill="" p-id="7572"></path></svg>
}


export default class Content extends Component<IProps, IState> {
    private resizableRef: RefObject<HTMLDivElement>;

    private mentionRef = React.createRef<Mention>();

    private groupinUsers: any[] = [];

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
        data: [],
        unread: 0,
        page: 1,
        groupLoading: false,
        groupinUsers: [],
        groudUserPage: 1,
        loading: false
    }


    constructor(props: IProps) {
        super(props);
        this.state = {
            height: 270,
            data: [],
            unread: 0,
            page: 1,
            groupLoading: false,
            loading: false,
            groudUserPage: 1,
            groupinUsers: []
        }

        this.handleMouseDown = this.handleMouseDown.bind(this);
        this.download = this.download.bind(this);
        this.onScroll = this.onScroll.bind(this);
        this.rowRenderer = this.rowRenderer.bind(this);
        this.onScrollGroupInUser = this.onScrollGroupInUser.bind(this);
        this.loadingMessage = this.loadingMessage.bind(this);
        this.onNotification = this.onNotification.bind(this)
        this.resizableRef = React.createRef();

    }

    componentDidMount() {
        PubSub.subscribe('changeGroup', this.onMessage)
        PubSub.subscribe('Notification', this.onNotification)

        this.loadingMessage();
    }

    // 监听props变化
    componentWillReceiveProps(nextProps: any) {
        const { group } = nextProps;
        if (group.id !== this.props.group.id) {
            this.setState({
                data: [],
                page: 1,
                groudUserPage: 1,
            }, () => {
                this.loadingMessage();
            });
        }

    }


    getOnLineUserIds() {
        const { group } = this.props;
        chatService.getOnLineUserIds(group.id)
            .then(res => {
                if (res.code === "200") {
                    const { groupinUsers } = this.state;
                    const updatedGroupinUsers = groupinUsers.map(user => {
                        if (res.data.includes(user.id)) {
                            return {
                                ...user,
                                onLine: true
                            };
                        } else {
                            return user;
                        }
                    });

                    this.setState({ groupinUsers: updatedGroupinUsers.sort((a, b) => {
                        if (a.onLine && !b.onLine) {
                            return -1;
                        } else if (!a.onLine && b.onLine) {
                            return 1;
                        } else {
                            return 0;
                        }
                    }) });

                } else {
                    Toast.error(res.message)
                }


            })
    }


    loadingGroupUser() {
        const { group } = this.props;
        const { groudUserPage } = this.state;
        ChatService.getGroupInUser(group.id)
            .then((res: any) => {
                if (res) {
                    this.groupinUsers = res;
                    this.setState({
                        groupinUsers: res
                    },()=>{
                        // 获取群聊所有的用户成功以后获取用户状态
                        this.getOnLineUserIds()
                    })
                }
            })
    }

    componentWillUnmount() {
        PubSub.unsubscribe('changeGroup');
        PubSub.unsubscribe('Notification');
    }

    onNotification(_: any, data: any) {
        if (data.type === "GroupUserNew") {
            // 当存在新用户登录则刷新状态。
            this.getOnLineUserIds()
        } else if (data.type === "FriendRequest") {
            Notification.info({
                content: data.content
            })
        } else if (data.type === "System") {
            Notification.info({
                content: data.content
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
        const value = this.mentionRef.current?.getValue();

        if (value && value.content.length > 0) {
            await ChatHubService.send('SendMessage', value.content, group.id, 0);

            this.mentionRef.current?.setValue('')
            for (let i = 0; i < value.base64.length; i++) {
                fileService.uploadBase64(value.base64[i], 'image.png')
                    .then((res: any) => {
                        if (res.code === '200') {
                            ChatHubService.send('SendMessage', res.data, group.id, 1);
                        }
                    })
            }
        } else if (value && value.base64.length > 0) {
            this.mentionRef.current?.setValue('')
            for (let i = 0; i < value.base64.length; i++) {
                fileService.uploadBase64(value.base64[i], 'image.png')
                    .then((res: any) => {
                        if (res.code === '200') {
                            ChatHubService.send('SendMessage', res.data, group.id, 1);
                        }
                    })
            }
        }
    }

    rendetContent = (item: any) => {
        const className = user?.id === item.user.id ? ' message-item-content-user' : '';

        if (item.type === "Text" || item.type === 0) {
            return (
                <div className={'message-item-content' + className} style={{ display: 'inline-block', marginBottom: '20px', marginLeft: '10px', whiteSpace: 'pre-wrap' }}>
                    {item.content}
                </div>
            )
        } else if (item.type === "Image" || item.type === 1) {
            return (
                <span>
                    <Image
                        width={'100%'}
                        className={className}
                        style={{
                            width: 'auto',
                            height: 'auto',
                            marginBottom: '20px',
                            marginTop: '8px',
                            borderRadius: '8px',
                            marginLeft: '10px',
                        }}
                        height={'100%'}
                        src={item.content}
                    />
                </span>
            )
        } else if (item.type === "File" || item.type === 2) {
            return (
                <Card
                    className='message-item-content '
                    style={{ width: 300, display: 'inline-block', marginBottom: '20px', marginLeft: '10px' }}
                >
                    <span>
                        {item.content.substring(item.content.lastIndexOf("/") + 1)}
                    </span>
                    <Button onClick={() => this.download(item.content)} style={{
                        float: 'right'
                    }}>下载</Button>
                </Card>
            )
        }
    }

    rowRenderer(item: any) {
        item.creationTime = moment(item.creationTime).format('YYYY-MM-DD HH:mm:ss');
        return (
            <div key={item.Id} style={{ margin: '15px' }}>
                <Tooltip position='right' content={() => this.renderInfo(item.user)} trigger="click" >
                    <Avatar size='small' style={{ float: 'left' }} src={item.user.avatar} />
                </Tooltip>
                <div style={{ paddingLeft: '40px', width: 'calc(100% - 50px)' }}>
                    {item.user.name}
                </div>
                {this.rendetContent(item)}
            </div>
        );
    }

    loadingMessage() {
        const { group } = this.props;
        const { page } = this.state;
        ChatService.getList(group.id, page, 20)
            .then((res: any) => {
                this.loadingGroupUser();
                if (res.code === "200") {
                    this.setState({
                        data: res.data.result,
                    }, () => {
                        this.scrollToBottom(true);
                    })
                }
            })

    }

    onScroll(_: any) {
        var element = document.getElementById('message-list')!;
        if (element.scrollTop === 0) {
            // 这是当前滚动条的高度
            console.log(element.scrollHeight);
            const height = element.scrollHeight;

            const { group } = this.props;
            const { page } = this.state;

            this.setState({
                loading: true,
            })

            ChatService.getList(group.id, page + 1, 20)
                .then((res: any) => {
                    if (res.data.result.length === 0) {
                        return;
                    }
                    this.setState({
                        data: [...res.data.result, ...this.state.data],
                        page: page + 1,
                    }, () => {
                        console.log(element.scrollHeight);
                        const newHeight = element.scrollHeight;
                        if (height !== newHeight) {
                            // 移动当之前定位
                            element.scrollTop = newHeight - height
                        }
                    })
                }).finally(() => {

                    this.setState({
                        loading: false,
                    })
                })
        }
    }

    ListComponent = (height: any) => {
        const { data } = this.state;
        return (
            <div onScroll={this.onScroll} id='message-list' style={{ height: '100%', overflow: 'auto', maxHeight: `calc((100vh - ${height}px))` }}>
                {data.map(x => {
                    return (<div>
                        {this.rowRenderer(x)}
                    </div>)
                })}
            </div>
        );
    };

    download(url: string) {
        var a = document.createElement('a');
        a.href = url;
        a.target = '_blank'
        a.download = url.split('/')[url.split('/').length - 1];
        a.click();
    }

    onSearch(value?: string) {
        const { groupinUsers } = this.state;

        if (value) {
            this.setState({
                groupinUsers: groupinUsers.filter(item => item.name.includes(value))
            })
        } else {
            this.setState({
                groupinUsers: this.groupinUsers
            })
        }
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
                        ChatHubService.send('SendMessage', res.data, this.props.group.id, 1);
                    }
                })

        }
    }

    selectFile() {
        var input = document.createElement('input');
        input.type = 'file';
        input.multiple = false;
        input.click();
        input.onchange = (e: any) => {
            var files = e.target.files;
            // 将文件放到form
            var formData = new FormData();
            formData.append('file', files[0]);
            // 上传图片
            fileService.upload(formData)
                .then((res: any) => {
                    if (res.code === '200') {
                        ChatHubService.send('SendMessage', res.data, this.props.group.id, 2);
                    }
                })

        }
    }

    invitation() {
        const { group } = this.props;
        const url = window.location.origin + "/invitation-group?code=" + group.id;
        copy(url)
        Toast.success('邀请地址已经复制');
    }

    renderInfo(item: any) {
        console.log(item);

        return (<div>
            <div style={{
                width: "320px",
                height: '120px',
            }}>
                <Avatar size="large" src={item.avatar} style={{ margin: 4, float: 'left' }} alt='User'>
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
                        昵称：{item.name}
                    </div>
                    <div>
                        账号：{item.account}
                    </div>
                </div>
                <div>

                </div>
            </div>
        </div>)
    }

    onScrollGroupInUser(value: any) {
        console.log(value);

        var element = document.getElementById('group-in-user')!;

        if (element.scrollTop + element.clientHeight === element.scrollHeight) {
            console.log('到底底部');

        }
    }

    renderEmoji() {
        return (<div className='emoji'>
            <div style={{
                height: "calc(100% - 50px)"
            }}> test</div>
            <div style={{
                height: '50px'
            }}>
                <div style={{
                    height: '40px',
                    width: '40px'
                }}>

                </div>
            </div>
        </div>)
    }

    render() {
        const { groupinUsers, loading } = this.state;
        const { group } = this.props;

        return (
            <>
                <div className="content-header">
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
                    <div className="content-box " style={{ flexBasis: `calc(100% - ${this.state.height}px - 10px)`, }}>
                        {this.ListComponent(this.state.height)}
                    </div>
                    <div className="draggable-line" onMouseDown={this.handleMouseDown}></div>
                    <div
                        className="resizable-box"
                        ref={this.resizableRef}
                        style={{ height: `${this.state.height}px` }}
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
                        <Mention ref={this.mentionRef} style={{
                            height: 'calc(100% - 160px)',
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
                        {groupinUsers.map(item => {
                            return (
                                <div className='grou-user-item'>
                                    <div className='grou-user-item-content'>
                                        <div style={{
                                            float: 'left'
                                        }}>
                                            <Tooltip position='leftTop' content={() => this.renderInfo(item)} trigger="click" >
                                                {item.onLine ? <Badge dot type='success' >
                                                    <Avatar size='extra-small' src={item.avatar} />
                                                </Badge> :
                                                    <Avatar size='extra-small' src={item.avatar} />}
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
                                            {item.name}
                                        </div>
                                        {item.id === "00000000-0000-0000-0000-000000000000" ?
                                            <Tag style={{
                                                boxSizing: 'content-box',
                                                float: 'right',
                                            }} color="blue">机器人</Tag> : (
                                                item.id === group.creator ?
                                                    <Tag style={{
                                                        boxSizing: 'content-box',
                                                        float: 'right',
                                                    }} color='red'>频道主人</Tag> :
                                                    <Tag style={{
                                                        boxSizing: 'content-box',
                                                        float: 'right',
                                                    }} color="grey">成员</Tag>

                                            )}
                                    </div>
                                </div>)
                        })}
                    </div>
                </div>
            </>
        );
    }
}
