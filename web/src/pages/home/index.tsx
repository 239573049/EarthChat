import React, { Component, RefObject } from 'react';
import './index.scss'
import { Avatar, Button, Form, Input, List, Row, Toast, Upload } from '@douyinfe/semi-ui';
import { ChatGroupDto, GetUserDto } from '../../dto';
import UserService from '../../services/userService';
import ChatService from '../../services/chatService';

import { IconSearch, IconPlus, IconCamera } from '@douyinfe/semi-icons';
import Content from '../../components/content';
import ChatHubService from '../../services/chatHubService';
import FileService from '../../services/fileService';
import Modal from '../../components/modal';
import config from '../../config';
import { GetUserInfos } from '../../store/user-store'
import chatService from '../../services/chatService';

interface AppState {
    middleWidth: number;
    selectid: number;
    user: GetUserDto,
    groups: ChatGroupDto[],
    selectGroup: ChatGroupDto,
    createGroupVisible: boolean,
    createGroupUpload: RefObject<Upload>,
    createGroupAvatar: string,
    createGroupFormApi: any,
    users: any[]
}

var intervalId: any;

const user = JSON.parse(localStorage.getItem('user') || '{}');

class Home extends Component<{}, AppState> {

    state: Readonly<AppState> = {
        middleWidth: 230,
        selectid: 0,
        user: {} as GetUserDto,
        groups: [],
        selectGroup: {} as ChatGroupDto,
        createGroupVisible: false,
        createGroupUpload: React.createRef<Upload>(),
        createGroupAvatar: '',
        createGroupFormApi: undefined,
        users: []
    }

    constructor(props: any) {
        super(props);

        if (!localStorage.getItem('token')) {
            ChatHubService.stop();
            window.location.href = '/login'
        }

        this.handleMouseDown = this.handleMouseDown.bind(this);
        this.selectGroup = this.selectGroup.bind(this)
        this.getFormApi = this.getFormApi.bind(this);
        this.createGroup = this.createGroup.bind(this)
        this.onMessage = this.onMessage.bind(this)
        this.stopBlinking = this.stopBlinking.bind(this)
    }

    selectGroup(name: string, value: any) {
        console.log(name);

        const { groups } = this.state;
        const item = groups.find(x => x.id == value.id);
        if (item) {
            this.selectChat(item)
        }
    }

    componentWillUnmount() {
        PubSub.unsubscribe('selectGroupInfo')
        PubSub.unsubscribe('changeGroup')
        console.log('componentWillUnmount');
    }

    async componentDidMount() {


        this.setState({
            selectGroup: this.state.groups[0],
        })

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
        PubSub.subscribe('selectGroupInfo', this.selectGroup)
        PubSub.subscribe('changeGroup', this.onMessage)
    }


    stopBlinking() {
        document.title = 'Chat'

        if (intervalId) {
            clearInterval(intervalId);
        }

    }

    startBlinking() {
        intervalId = setInterval(function () {
            var title = document.title;
            document.title = (title === "Chat [新的消息]") ? "Chat" : "Chat [新的消息]";
        }, 1000);
    }



    onMessage = async (_: any, data: any) => {
        const { groups } = this.state;
        var v = groups.find(x => x.id === data.groupId)
        if (!v) {
            return;
        }

        var users = await GetUserInfos([data.userId]);

        if (data.type === "Text") {
            v.lastMessage = users[0].name + '：' + data.content.substring(0, 20)
        } else if (data.type === "Image") {
            v.lastMessage = users[0].name + '：图片'
        } else if (data.type === "File") {
            v.lastMessage = users[0].name + '：文件'
        } else if (data.type === "Video") {
            v.lastMessage = users[0].name + '：视频'
        } else if (data.type === "Audio") {
            v.lastMessage = users[0].name + '：语音'
        }

        // 将v插入到groups数组的最前面
        const index = groups.findIndex(x => x.id === data.groupId);
        if (index !== -1) {
            groups.splice(index, 1);
        }
        groups.unshift(v);

        this.setState({ groups });

        // 如果是自己发送的消息不需要更新状态
        if (data.userId === user.id) {
            return;
        }

        if (intervalId) {
            clearInterval(intervalId);
        }

        this.startBlinking()


        // 取消闪烁标题
        document.addEventListener("click", () => {
            this.stopBlinking()
        });


        document.addEventListener("mousemove", () => {
            this.stopBlinking()
        });
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
                    x.lastMessage = '';
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

    getFormApi(v: any) {
        this.setState({
            createGroupFormApi: v
        })
    }

    createGroupClose() {
        this.setState({ createGroupVisible: false });
        this.state.createGroupUpload.current?.clear();

        if (this.state.createGroupAvatar) {
            FileService.deleteFile(this.state.createGroupAvatar)
        }
    }

    createGroup(value: any) {
        if (!value.avatar) {
            Toast.error("未上传头像")
            return;
        }

        var avatar = value.avatar[0]?.response?.data;

        var v = {
            avatar,
            description: value.description,
            name: value.name
        }

        chatService.createGroup(v)
            .then(res => {
                if (res.code === "200") {
                    this.loadingGroups()
                    this.createGroupClose();
                    Toast.success("添加成功");
                }
            })
    }

    render() {
        const { middleWidth, createGroupAvatar, selectGroup, groups, createGroupVisible } = this.state;
        const rightWidth = `calc(100% - 60px - ${middleWidth}px)`;
        console.log('middleWidth', middleWidth);

        const renderContent = () => {
            if (selectGroup?.avatar) {
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
                <div className="middle " style={{ width: `${middleWidth}px` }}>
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
                                <Button onClick={() => this.setState({ createGroupVisible: true })} style={{
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
                                            <div style={{
                                                fontSize: '18px',
                                                fontWeight: 'bold',
                                            }}>
                                                {item.name}
                                            </div>
                                            <div style={{
                                                fontSize: '12px',
                                                whiteSpace: 'nowrap',
                                                overflow: 'hidden',
                                                width: `${middleWidth / 2}px`,
                                                textOverflow: 'ellipsis'
                                            }}>
                                                {item.lastMessage}
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            }
                        />
                    </div>
                </div>
                <div className="resizer " onMouseDown={this.handleMouseDown}></div>
                <div className="right " style={{ width: rightWidth }}>
                    {renderContent()}
                </div>

                <Modal width={300} title='添加群聊' isOpen={createGroupVisible} onClose={() => this.createGroupClose()}>
                    <Form
                        getFormApi={this.getFormApi}
                        onSubmit={this.createGroup}
                    >
                        <Row>
                            <Form.Upload field='avatar'
                                className="avatar-upload"
                                action={config.API + "/api/v1/Files/upload"}
                                accept={'image/*'}
                                ref={this.state.createGroupUpload}
                                fileName='file'
                                headers={{
                                    "Authorization": "Bearer " + localStorage.getItem('token')
                                }}
                                style={{
                                    left: '40%',
                                    position: 'relative',
                                }}
                                onSuccess={(file: any) => {
                                    if (file.code === '200') {
                                        this.setState({
                                            createGroupAvatar: file.data
                                        })
                                    } else {
                                        Toast.error(file.message);
                                    }
                                }}
                                showUploadList={false}
                                onError={() => Toast.error('上传失败')}>
                                <Avatar src={createGroupAvatar} style={{ margin: 4 }} hoverMask={<IconCamera />} />
                            </Form.Upload>
                            <Form.Input field='name' label='群聊名称' style={{ width: '100%' }} ></Form.Input>
                            <Form.TextArea rows={5} field='description' label='群聊描述' style={{ width: '100%' }} ></Form.TextArea>
                            <Button htmlType='submit' block>新增群聊</Button>
                        </Row>
                    </Form>
                </Modal>
            </>
        );
    }
}

export default Home;
