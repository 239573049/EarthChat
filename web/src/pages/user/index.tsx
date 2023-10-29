import { Avatar, Button, Divider, Icon, Notification } from '@douyinfe/semi-ui';
import React, { Component } from 'react';
import './index.scss'
import chatService from '../../services/chatService';
import moment from 'moment/moment';
import { ChatGroupDto } from '../../dto';
import PubSub from 'pubsub-js';
import friendService from '../../services/friendService';
import { GetUserInfos } from '../../store/user-store';
import { IconMailStroked1 } from '@douyinfe/semi-icons'

interface IState {
    middleWidth: number;
    selectid: number;
    selectValue: any,
    slidingBlock: number;
    renderValue: any,
    page: number,
    pageSize: number,
    data: any[]
}


const message = () => {
    return (<svg viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="2477" width="25" height="25"><path d="M996.515019 454.715319c0 55.483662-12.576433 109.289102-37.371142 159.91206-23.822568 48.65821-57.878199 92.302219-101.204983 129.714294-43.11189 37.238113-93.253894 66.443271-149.044548 86.827531-57.601906 21.039175-118.754684 31.702026-181.759647 31.702026-11.307533 0-20.466124-9.15859-20.466124-20.466124 0-11.2973 9.15859-20.466124 20.466124-20.466124 58.205657 0 114.63076-9.82374 167.719886-29.215392 51.114145-18.675338 96.988961-45.373397 136.335085-79.357396 80.20674-69.27783 124.393101-161.129794 124.393101-258.650875 0-97.510848-44.186362-189.373045-124.393101-258.650875-39.346123-33.983999-85.22094-60.682058-136.335085-79.347163-53.089126-19.391652-109.514229-29.225625-167.719886-29.225625s-114.63076 9.833973-167.719886 29.225625c-51.124378 18.665105-96.988961 45.363164-136.335085 79.347163-80.216973 69.27783-124.393101 161.129794-124.393101 258.650875 0 59.024302 15.861246 115.39824 47.133483 167.535691 30.04427 50.101071 73.933873 94.338598 126.930901 127.923508 5.904477 3.755534 9.496282 10.263761 9.506515 17.263176 0 2.691295-0.470721 58.71731-42.395576 132.927475 33.850969-5.761214 66.586534-19.24839 97.715509-40.328497 32.694633-22.144346 51.216475-44.779879 51.40067-45.005007 7.101745-8.769734 19.98517-10.151197 28.765137-3.059686s10.181897 19.934005 3.121084 28.724205c-0.86981 1.084705-21.745257 26.841322-59.065234 52.362578-49.210795 33.666774-104.530728 51.441603-160.065556 51.441603l-1.166569 0c-7.633864-0.040932-14.602579-4.308119-18.102287-11.092639-3.489474-6.774287-2.926656-14.94027 1.463328-21.172205 29.379121-41.679261 43.418882-78.815043 50.019207-102.637612 3.735068-13.46671 5.577019-24.313755 6.487761-31.630395-53.713342-36.102243-98.401124-82.44778-129.724527-134.667096-34.638915-57.765635-52.956096-122.970706-52.956096-188.585099 0-55.483662 12.576433-109.289102 37.360909-159.91206 23.832801-48.647977 57.878199-92.302219 101.204983-129.714294 43.11189-37.227879 93.253894-66.443271 149.044548-86.827531 57.601906-21.039175 118.754684-31.702026 181.76988-31.702026 63.004963 0 124.157741 10.662851 181.759647 31.702026 55.790654 20.384259 105.932658 49.599651 149.044548 86.827531 43.326784 37.412075 77.382415 81.066317 101.204983 129.714294C983.938586 345.426217 996.515019 399.231657 996.515019 454.715319z" p-id="2478"></path></svg>)
}

class User extends Component<any, IState> {

    state: Readonly<IState> = {
        middleWidth: 230,
        selectid: 0,
        slidingBlock: 0,
        data: [],
        selectValue: undefined,
        renderValue: undefined,
        page: 1,
        pageSize: 10
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

    selectUser() {
        
        chatService.getUserGroup(false)
            .then((res: ChatGroupDto[]) => {
                res.forEach(x => {
                    x.type = 'group';
                })

                this.setState({
                    slidingBlock: 0,
                    data: res
                })
            })

    }

    selectGroup() {
        chatService.getUserGroup(true)
            .then((res: ChatGroupDto[]) => {
                res.forEach(x => {
                    x.type = 'group';
                })

                this.setState({
                    slidingBlock: 50,
                    data: res
                })

            })

    }

    onClick(item: any) {
        this.setState({
            selectValue: item
        }, async () => {
            await this.renderValue()
        })
    }

    Group(item: any) {

        PubSub.publish('selectGroup', item);

    }

    async renderValue() {
        const { selectValue } = this.state;
        let renderValue;
        if (!selectValue) {
            return;
        }

        if (selectValue.type === 'group') {
            renderValue = <div style={{
                height: "115px",
                margin: 'auto',
                width: '500px',
                paddingTop: "50px"
            }}>
                <div style={{
                    marginBottom: '20px',
                    height: '75px'
                }}>
                    <Avatar style={{
                        float: 'left'
                    }} size='large' src={selectValue.avatar} />
                    <div style={{
                        margin: '8px',
                        float: 'left'
                    }}>
                        <div style={{
                            fontSize: '18px',

                        }}>
                            {selectValue.name}
                        </div>
                        <div style={{
                            marginTop: "10px"
                        }}>
                            {selectValue.id}
                        </div>
                    </div>
                    <div style={{
                        float: 'left',
                        borderRadius: '500px',
                        backgroundColor: '#1472D0',
                        width: '50px',
                        height: '50px',
                        cursor: "pointer"
                    }} onClick={() => this.Group(selectValue)}>
                        <Icon style={{
                            margin: '13px'
                        }} svg={message()} />
                    </div>
                </div>
                <Divider></Divider>

            </div>;
        } else if (selectValue.type === "user") {
            renderValue = <div style={{
                height: "115px",
                margin: 'auto',
                width: '500px',
                paddingTop: "50px"
            }}>
                <div style={{
                    marginBottom: '20px',
                    height: '75px'
                }}>

                </div>
                <Divider></Divider>
            </div>
        } else if (selectValue.type === "user-manager") {
            await this.renderUserManager()

            return
        }

        this.setState({
            renderValue
        })
    }

    friendHandle(id: string, state: number) {
        friendService.FriendHandle(id, state)
            .then(res => {
                if (res.code === "200") {
                    Notification.success({
                        content: "成功"
                    })

                    this.renderUserManager()

                } else {
                    Notification.error({
                        content: res.message
                    })
                }
            })
    }

    async renderUserManager() {
        const { page, pageSize } = this.state;
        const result = await friendService.List(page, pageSize);

        if (result.code === "200") {
            const requestIds = result.data.result.map((item: any) => item.requestId);

            const users = await GetUserInfos(requestIds);

            const items = result.data.result.length > 0 ? result.data.result.map((x: any) => {
                const user = users.filter(y => y.id === x.requestId)[0];
                return <div className='user-manager-item' style={{
                    height: "75px",
                    width: '600px',
                    margin: '8px',

                    backgroundColor: 'var(--setting-tong-theme-background-color)'
                }}>

                    <div style={{
                        padding: '13px'
                    }}>
                        <Avatar style={{
                            float: 'left',
                        }} src={user.avatar}></Avatar>
                        <div style={{
                            marginLeft: '5px',
                            float: 'left',
                            fontSize: '14px'
                        }}>
                            <span className='name'>
                                {user.name}
                            </span>
                            <span>请求加为好友</span>
                            <span style={{
                                marginLeft: '5px'
                            }}>{moment(x.applicationDate).format('YYYY-MM-DD HH:mm:ss')}</span>
                            <div className='description'>
                                留言：{x.description}
                            </div>
                        </div>
                    </div>

                    <div style={{
                        float: 'right',
                        marginRight: '10px',
                    }}>
                        {x.state === 0 ? <><Button style={{
                            margin: '5px'
                        }} theme='borderless' type='secondary' onClick={() => this.friendHandle(x.id, 1)}>同意</Button>
                            <Button style={{
                                margin: '5px'
                            }} theme='borderless' type='danger' onClick={() => this.friendHandle(x.id, 2)}>拒绝</Button>
                        </> : <div>{x.state === 1 ? "已同意" : "已拒绝"}</div>}
                    </div>
                </div>
            }) : <div style={{
                marginTop:'200px',
                textAlign: 'center',
            }}>
                <div>
                    <IconMailStroked1 size='extra-large' />
                </div>
                暂无通知
            </div>

            let renderValue = <>
                <div style={{
                    marginLeft: '25px',
                    marginTop: '35px'
                }}>
                    好友通知
                </div>
                <div style={{
                    height: "115px",
                    margin: 'auto',
                    width: '500px',
                    paddingTop: "50px"
                }}>

                    {items}
                </div>
            </>

            this.setState({
                renderValue
            })
        }
    }

    loadUserManager() {

    }

    selectUserManager() {
        this.setState({
            selectValue: {
                type: 'user-manager'
            }
        }, async () => {
            await this.renderValue()
        })
    }

    render() {
        const { middleWidth, slidingBlock, selectValue, data, renderValue } = this.state;

        const rightWidth = `calc(100% - 60px - ${middleWidth}px)`;
        return (
            <>
                <div className="middle" style={{ width: `${middleWidth}px` }}>
                    <div style={{
                        marginTop: '30px',
                        fontSize: '20px',
                        fontWeight: 'bold',
                        height: "100%",
                        textAlign: 'center',
                    }}>
                        <h2>好友管理</h2>
                        <div onClick={() => this.selectUserManager()} className='inform'>
                            <div className='name'>
                                好友通知
                            </div>
                        </div>
                        <div className='inform'>
                            <div className='name'>
                                群通知
                            </div>
                        </div>
                        <Divider></Divider>
                        <div style={{
                            border: '1px',
                            margin: '10px',
                            borderRadius: "8px",
                            backgroundColor: 'var(--user-tab-background-color)',
                            height: '35px'
                        }}>
                            <span onClick={() => this.selectUser()} style={{
                                width: '50%',
                                float: 'left',
                                textAlign: 'center',
                                lineHeight: '35px',
                                zIndex: 100,
                                position: 'relative',
                                fontSize: '14px',
                                cursor: 'pointer',
                            }}>好友</span>
                            <span onClick={() => this.selectGroup()} style={{
                                width: '50%',
                                float: 'left',
                                zIndex: 100,
                                textAlign: 'center',
                                position: 'relative',
                                lineHeight: '35px',
                                cursor: 'pointer',
                                fontSize: '14px',
                            }}>群聊</span>
                            <div className='sliding-block' style={{
                                left: slidingBlock + "%",
                            }}>
                            </div>
                        </div>

                        {data.map((item: any) => {
                            return (<div onClick={() => this.onClick(item)} className='manager'>
                                <Avatar style={{
                                    float: 'left',
                                    marginTop: '8px'
                                }} src={item.avatar}></Avatar>
                                <div style={{
                                    fontSize: '18px',
                                    textAlign: 'center',
                                    float: 'left',
                                    userSelect: 'none',
                                    marginLeft: '15px',
                                    marginTop: '18px'
                                }}>
                                    {item.name}
                                </div>
                            </div>)
                        })}

                    </div>
                </div>
                <div className="resizer" onMouseDown={this.handleMouseDown}></div>
                <div className="right" style={{ width: rightWidth }}>
                    {selectValue && renderValue}
                </div>
            </>
        );
    }
}

export default User;