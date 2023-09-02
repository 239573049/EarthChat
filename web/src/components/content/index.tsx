import React, { Component, RefObject } from 'react';
import { ChatGroupDto } from '../../dto';

import moment from 'moment/moment';
import { List, CellMeasurerCache, CellMeasurer } from 'react-virtualized';
import { Avatar, Input, List as SList, Button, Card, Icon, Image, Tag, Notification, Toast } from '@douyinfe/semi-ui';
import './index.scss';
import Mention from '../Mention';
import { IconFile, IconSearch } from '@douyinfe/semi-icons';
import ChatHubService from '../../services/chatHubService';
import fileService from '../../services/fileService';
import PubSub from 'pubsub-js';
import ChatService from '../../services/chatService';
import AutoSizer from "react-virtualized-auto-sizer";
import Theme from '../Theme';
import copy from 'copy-to-clipboard';

interface IProps {
    group: ChatGroupDto;
}

interface IState {
    height: number;
    data: any[],
    unread: number,
    page: number,
    groupinUsers: any[],
    pageSize: number,
}

const user = JSON.parse(localStorage.getItem('user') || '{}');

const cache = new CellMeasurerCache({
    defaultHeight: 100,
    fixedWidth: true,
});


export default class Content extends Component<IProps, IState> {
    private resizableRef: RefObject<HTMLDivElement>;

    private listRef = React.createRef<List>();
    private mentionRef = React.createRef<Mention>();

    private groupinUsers: any[] = [];

    state: Readonly<IState> = {
        height: 270,
        data: [],
        unread: 0,
        page: 1,
        groupinUsers: [],
        pageSize: 20
    }


    constructor(props: IProps) {
        super(props);
        this.state = {
            height: 270,
            data: [],
            unread: 0,
            page: 1,
            groupinUsers: [],
            pageSize: 20
        }

        this.handleMouseDown = this.handleMouseDown.bind(this);
        this.download = this.download.bind(this);
        this.onScroll = this.onScroll.bind(this);
        this.rowRenderer = this.rowRenderer.bind(this);
        this.loadingMessage = this.loadingMessage.bind(this);
        this.resizableRef = React.createRef();

    }

    componentDidMount() {
        PubSub.subscribe('changeGroup', this.onMessage)

        this.loadingMessage();
    }

    // 监听props变化
    componentWillReceiveProps(nextProps: any) {
        const { group } = nextProps;
        if (group.id !== this.props.group.id) {
            console.log(group.id, this.props.group.id);

            this.setState({
                data: [],
                page: 1,
                pageSize: 20,
            }, () => {
                this.loadingMessage();
            });
        }

    }

    loadingGroupUser() {
        const { group } = this.props;
        ChatService.getGroupInUser(group.id)
            .then((res: any) => {
                this.groupinUsers = res;
                this.setState({
                    groupinUsers: res
                })
            })
    }

    componentWillUnmount() {
        PubSub.unsubscribe('changeGroup');
    }

    onMessage = (_: any, data: any) => {
        const { group } = this.props;
        if (group.id === data.groupId) {
            this.setState({
                data: [...this.state.data, data]
            },()=>{
                if(data.user.id === user.id){
                    this.scrollToBottom();
                }
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
                        preview={false}
                        className={className}
                        style={{
                            width: 'auto',
                            // 图片显示自适应
                            height: 'auto',
                            marginBottom: '20px',
                            marginLeft: '10px'
                        }}
                        src={item.content}
                    />
                </span>
            )
        } else if (item.type === "File" || item.type === 2) {
            var name = item.content.split('/')[item.content.split('/').length - 1];
            return (
                <Card
                    className='message-item-content '
                    title={name}
                    style={{ width: 300, display: 'inline-block', marginBottom: '20px', marginLeft: '10px' }}
                >
                    <Avatar>
                        <IconFile />
                    </Avatar>
                    <Button onClick={() => this.download(item.content)} style={{
                        float: 'right'
                    }}>下载</Button>
                </Card>
            )
        }
    }

    rowRenderer({
        key, // Unique key within array of rows
        index, // Index of row within collection
        style, // Style object to be applied to row (to position it)
        parent
    }: any) {
        const { data } = this.state;
        const item = data[index];
        item.creationTime = moment(item.creationTime).format('YYYY-MM-DD HH:mm:ss');
        return (
            <CellMeasurer
                cache={cache}
                columnIndex={10}
                key={key}
                parent={parent}
                rowIndex={index}
            >
                {({ measure }: any) => (
                    <div key={item.Id} onLoad={measure} style={{ margin: '15px', ...style }}>
                        <Avatar size='small' style={{ float: 'left' }} src={item.user.avatar} />
                        <div style={{ paddingLeft: '40px', width: 'calc(100% - 50px)' }}>
                            {item.user.name}
                        </div>
                        {this.rendetContent(item)}
                    </div>
                )}
            </CellMeasurer>
        );
    }

    loadingMessage() {
        const { group } = this.props;
        const { page, pageSize } = this.state;
        ChatService.getList(group.id, page, pageSize)
            .then((res: any) => {
                this.loadingGroupUser();
                this.setState({
                    data: res.data.result,
                }, () => {
                    this.scrollToBottom();
                })
            })

    }

    onScroll(onScrollProps: any) {
        if (onScrollProps.scrollTop === 0) {
            const { group } = this.props;
            const { page, pageSize } = this.state;
            ChatService.getList(group.id, page + 1, pageSize)
                .then((res: any) => {
                    if (res.data.result.length === 0) {
                        return;
                    }
                    this.setState({
                        data: [...res.data.result, ...this.state.data],
                        page: page + 1,
                    }, () => {
                        // this.listRef.current?.scrollToRow(res.data.result.length);
                    })
                })
        }
    }

    ListComponent = () => {
        return (
            <>
                <div style={{ height: '100%' }}>
                    <AutoSizer>
                        {({ height, width }: any) => (
                            <List
                                id='message-list'
                                ref={this.listRef}
                                onFocus={() => this.setState({
                                    unread: 0
                                })}
                                width={width}
                                onScroll={this.onScroll}
                                height={height}
                                rowCount={this.state.data.length}
                                rowHeight={cache.rowHeight}
                                rowRenderer={this.rowRenderer}
                            />
                        )}
                    </AutoSizer>
                </div >
            </>
        );
    };
    download(url: string) {
        var a = document.createElement('a');
        a.href = url;
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
    scrollToBottom() {
        var element = document.getElementById('message-list')!;
        var scrollHeight = element.scrollHeight;
        var scrollTop = element.scrollTop;
        var scrollCount = 0;
        var scrollMargin;

        function scroll() {
            scrollMargin = scrollHeight - scrollTop;
            element.scrollTop += scrollMargin / 20;
            scrollCount++;

            if (scrollCount < 20) {
                requestAnimationFrame(scroll);
            }
        }

        scroll();
    }


    selectPicture() {
        // 打开文件选择器，选择图片
        console.log('selectPicture');

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

    invitation() {
        const { group } = this.props;
        const url = window.location.origin + "/invitation-group?code=" + group.id;
        copy(url)
        Toast.success('邀请地址已经复制');
    }

    render() {
        const { groupinUsers } = this.state;
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
                        <Button theme='borderless' onClick={() => this.invitation()}>邀请</Button>
                        <Theme />
                    </div>
                </div>
                <div className="content-divider">
                </div>
                <div className="main-content" style={{
                    float: 'left',
                    width: 'calc(100% - 180px)',
                }}>
                    <div className="content-box " style={{ flexBasis: `calc(100% - ${this.state.height}px - 10px)` }}>
                        {this.ListComponent()}
                    </div>
                    <div className="draggable-line" onMouseDown={this.handleMouseDown}></div>
                    <div
                        className="resizable-box"
                        ref={this.resizableRef}
                        style={{ height: `${this.state.height}px` }}
                    >
                        <div className='content-function'>
                            <Icon style={{
                                float: 'left',
                            }} svg={
                                <svg className='icon-function' viewBox="0 0 1025 1024" width="20" height="20"><path d="M512.016 1024C229.232 1024 0.016 794.784 0.016 512 0.016 229.216 229.232 0 512.016 0 794.784 0 1024 229.216 1024 512 1024 794.784 794.784 1024 512.016 1024ZM512.016 64C264.976 64 64.016 264.96 64.016 512 64.016 759.024 264.976 960 512.016 960 759.04 960 960 759.024 960 512 960 264.96 759.04 64 512.016 64ZM510.336 833.456C509.056 833.456 507.744 833.488 506.448 833.488 310.992 833.488 229.024 657.12 225.616 649.552 218.336 633.424 225.488 614.496 241.584 607.216 257.712 599.968 276.576 607.088 283.888 623.088 286.64 629.12 352.928 769.488 506.576 769.488 507.584 769.488 508.576 769.456 509.584 769.456 672.896 767.552 738.368 624.768 739.024 623.344 746.176 607.216 765.024 599.872 781.264 607.152 797.392 614.336 804.672 633.248 797.456 649.408 794.176 656.8 714.208 831.056 510.336 833.456ZM671.504 479.84C636.224 479.84 607.664 451.232 607.664 415.984 607.664 380.768 636.224 352.176 671.504 352.176 706.768 352.176 735.344 380.768 735.344 415.984 735.344 451.232 706.768 479.84 671.504 479.84ZM351.504 479.84C316.224 479.84 287.664 451.232 287.664 415.984 287.664 380.768 316.224 352.176 351.504 352.176 386.768 352.176 415.344 380.768 415.344 415.984 415.344 451.232 386.768 479.84 351.504 479.84Z" p-id="2484"></path></svg>
                            } />

                            <Icon style={{
                                float: 'left',
                                marginLeft: '15px',
                            }} svg={
                                <svg className='icon-function' viewBox="0 0 1025 1024" width="20" height="20"><path d="M437.76 430.08L170.496 79.36C156.672 61.44 159.232 35.84 176.64 20.48c16.896-14.848 42.496-12.8 56.832 4.096L512 344.576l278.528-320c14.848-16.896 39.936-18.432 56.832-4.096 17.408 14.848 19.968 40.448 6.144 58.88L586.24 430.08l165.888 190.976c92.672-33.792 196.096 4.096 245.248 89.6 49.152 85.504 29.184 194.048-47.104 256.512-76.288 62.464-186.368 61.44-260.608-3.072-74.752-64.512-92.16-173.056-40.96-257.536-1.536-1.536-3.072-3.584-4.096-5.12L512 527.872 437.76 430.08zM383.488 492.544l77.824 101.888L379.904 701.44c-1.536 1.536-2.56 3.584-4.096 5.12 50.688 84.48 33.792 193.024-40.96 257.536-74.752 64.512-184.832 65.536-260.608 3.072-76.288-62.464-95.744-171.008-47.104-256.512 49.152-85.504 152.576-123.392 245.248-89.6l111.104-128.512zM215.04 931.84c44.032-3.584 82.432-30.72 100.352-70.656 17.92-39.936 13.312-86.528-12.8-122.368-26.112-35.328-69.12-53.76-112.64-48.64-65.536 8.192-112.64 67.584-105.472 133.12 6.656 66.048 64.512 114.176 130.56 108.544z m593.92 0c43.52 5.632 86.528-13.312 112.64-48.64 26.112-35.328 30.72-81.92 12.8-121.856-17.92-39.936-56.32-67.072-100.352-70.656-66.048-5.632-124.416 42.496-131.072 108.032-6.656 65.536 40.448 124.928 105.984 133.12z m0 0" p-id="3601"></path></svg>
                            } />
                            <Icon style={{
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
                    <SList
                        dataSource={groupinUsers}
                        split={false}
                        header={<Input onCompositionEnd={(v: any) => this.onSearch(v.target.value)} onChange={(v) => !v ? this.onSearch() : null} placeholder='搜索' prefix={<IconSearch />} />}
                        size='small'
                        style={{
                            flexBasis: '100%',
                            flexShrink: 0,
                            height: 'calc(100% - 50px)',
                            borderBottom: '1px solid var(--semi-color-border)'
                        }}
                        renderItem={item =>
                            <div className='grou-user-item'>
                                <div className='grou-user-item-content'>
                                    <Avatar size='extra-small' src={item.avatar} />
                                    <span style={{
                                        marginLeft: '10px',
                                        userSelect: 'none',
                                        fontSize: '14px',
                                    }}>
                                        {item.name}
                                    </span>
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
                            </div>
                        }
                    />
                </div>
            </>
        );
    }
}
