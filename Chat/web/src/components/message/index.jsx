import './index.css'
import { FixedSizeList } from 'react-window';
import AutoSizer from "react-virtualized-auto-sizer";
import React from 'react'
import { Button, Mentions } from 'antd';
import Icon, {
    SmileOutlined,
    FileOutlined,
    FileImageOutlined
} from '@ant-design/icons';

class Message extends React.Component {

    state = {
        message: [{
            key: 1,
            content: '你好',
            creationTime: new Date(),
            user: {
                avatar: '',
                name: '张三',
            }
        }],
        height: 200
    }

    componentDidMount() {

        window.addEventListener('resize', () => {
            const { height } = this.state
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



    render() {
        const Row = ({ index, style }) => (
            <div style={style}>Row {index}</div>
        );
        const { message, height } = this.state


        
        return (
            <>
                <div className='message'>
                    <div style={{
                        height: 'calc(100% - 240px)',
                    }} id='message'>
                        <FixedSizeList
                            height={height} // 列表可视区域的高度
                            itemCount={'100'} // 列表数据长度
                            itemSize={35} // 列表行高
                            width={'100%'} //列表可视区域的宽度
                            layout='vertical'
                        >
                            {Row}
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
                            <Button>发送</Button>
                        </div>
                    </div>
                </div>
            </>
        )
    }
}

export default Message