import './index.css'
import { FixedSizeList } from 'react-window';
import React from 'react'
import { Button, Mentions, Avatar } from 'antd';
import {
    SmileOutlined,
    FileOutlined,
    FileImageOutlined
} from '@ant-design/icons';

class Message extends React.Component {

    state = {
        message: [],
        value: '',
        height: 200
    }

    constructor(props) {
        super(props)
    }

    componentDidMount() {

        // 监听消息
        window.connection.on('ReceiveMessage', (message) => {
            const { message: oldMessage } = this.state
            oldMessage.push(message)
            console.log('收到消息', message)
            this.setState({
                message: [...oldMessage]
            })
        });

        window.addEventListener('resize', () => {
            const { height } = this.state;
            console.log('resize');
            const messageid = document.getElementById('message');
            if (messageid && messageid.clientHeight !== height) {
                this.setState({
                    height: messageid.clientHeight
                })
            }
        });

        // 等待dom渲染完成
        setTimeout(() => {
            const messageid = document.getElementById('message');

            this.setState({
                height: messageid.clientHeight
            })
        }, 50);
    }

    componentWillUnmount() {
        window.connection.off('ReceiveMessage')
    }

    onScroll(ListOnScrollProps) {
        console.log(ListOnScrollProps)
    }

    async sendMessage() {
        const { value } = this.state;
        if (value === '') {
            return;
        }
        console.log('发送消息');
        await window.connection.send('SendMessage', value, 0)

        this.setState({
            value: ''
        })

    }

    render() {

        const { message, height, value } = this.state


        const renderItem = ({ index }) => {
            const item = message[index];
            return <div key={item.id} style={{
                width: '100%'
            }}>
                <Avatar size='large' style={{
                    float: 'left',
                }} src={<img src={item.User.Avatar} alt="avatar" />} />
                <div style={{
                    float: 'left',
                    marginLeft: '10px',
                    width: 'calc(100% - 50px)',
                    display: "inline-block"
                }}>{item.User.Name}
                </div>
                <div className='message-item-content message-item-content-user' style={{
                    display: 'inline-block',
                    marginBottom: '20px',
                    marginLeft: '10px',
                }}>
                    {item.Content}
                </div>
            </div>
        };

        return (
            <>
                <div className='message'>
                    <div style={{
                        height: 'calc(100% - 240px)',
                    }} id='message'>
                        <FixedSizeList
                            height={height - 20} // 列表可视区域的高度
                            itemCount={message.length} // 列表数据长度
                            itemSize={35} // 列表行高
                            width={'100%'} //列表可视区域的宽度
                            initialScrollOffset={999999}
                            style={{
                                margin:'10px'
                            }}
                            overscanCount={50}
                            layout='vertical'
                        >
                            {renderItem}
                        </FixedSizeList>
                    </div>
                    <div className='chat-send'>
                        <div className='chat-tools'>
                            <Button size='small' style={{
                                marginLeft: '10px',
                                border: 'none',
                            }} icon={<SmileOutlined />} />

                            <Button size='small' style={{
                                marginLeft: '10px',
                                border: 'none',
                            }} icon={<FileOutlined />} />
                            <Button size='small' style={{
                                marginLeft: '10px',
                                border: 'none',
                            }} icon={<FileImageOutlined />} />

                        </div>
                        <Mentions
                            autoSize
                            rows={5}
                            value={value}
                            onChange={(v) => this.setState({
                                value: v
                            })}
                            className='chat-input'
                            placeholder="开始聊天吧"
                            options={[
                                {
                                    value: 'afc163',
                                    label: 'afc163',
                                },
                                {
                                    value: 'zombieJ',
                                    label: 'zombieJ',
                                },
                                {
                                    value: 'yesmeck',
                                    label: 'yesmeck',
                                },
                            ]}
                        />
                        <div style={{
                            float: "right",
                            marginRight: '10px',
                        }}>
                            <Button onClick={async () => await this.sendMessage()} type='primary' >发送</Button>
                        </div>
                    </div>
                </div>
            </>
        )
    }
}

export default Message