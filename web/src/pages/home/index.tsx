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
    createGroupFormApi: any
}


class Home extends Component<{}, AppState> {

    constructor(props: any) {
        super(props);

        this.state ={
            
            middleWidth: 230,
            selectid: 0,
            user: {} as GetUserDto,
            groups: [],
            selectGroup: {} as ChatGroupDto,
            createGroupVisible: false,
            createGroupUpload: React.createRef<Upload>(),
            createGroupAvatar: '',
            createGroupFormApi: undefined
        }

        if (!localStorage.getItem('token')) {
            ChatHubService.stop();
            window.location.href = '/login'
        }


        this.handleMouseDown = this.handleMouseDown.bind(this);
        this.selectGroup = this.selectGroup.bind(this)
        this.getFormApi = this.getFormApi.bind(this);
        this.createGroup = this.createGroup.bind(this)
    }

    selectGroup(name:string,value:any){
        console.log(name);
        
        const {groups} = this.state;
        const item = groups.find(x=>x.id == value.id);
        if(item){
            this.selectChat(item)
        }
    }

    componentWillUnmount(){
        PubSub.unsubscribe('selectGroupInfo')
    }

    componentDidMount() {

        this.setState({
            selectGroup:this.state.groups[0]
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
        PubSub.subscribe('selectGroupInfo',this.selectGroup)
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
            description:value.description,
            name:value.name
        }

        chatService.createGroup(v)
            .then(res=>{
                if(res.code === "200"){
                    this.loadingGroups()
                    this.createGroupClose();
                    Toast.success("添加成功");
                }
            })
    }

    render() {
        const { middleWidth, createGroupAvatar, selectGroup, groups, createGroupVisible } = this.state;
        const rightWidth = `calc(100% - 60px - ${middleWidth}px)`;

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
