import { Divider, Avatar, Badge } from 'antd'
import './index.css'
import { FixedSizeList } from 'react-window';
import { Component } from 'react'
import {
    getOnlineUsers,
} from '../../services/chatService'

class OnLine extends Component {

    state = {
        height: 200,
        onlineUsers: []
    }

    loadOnlineUsers() {
        getOnlineUsers()
            .then(x => {
                this.setState({
                    onlineUsers: x.data
                })
            })
    }

    componentDidMount() {
        this.loadOnlineUsers()
        window.addEventListener('resize', () => {
            const { height } = this.state
            const messageid = document.getElementById('user-list');
            if (messageid && messageid.clientHeight !== height) {
                this.setState({
                    height: messageid.clientHeight
                })
            }
        });

        // 等待dom渲染完成
        setTimeout(() => {
            const messageid = document.getElementById('user-list');

            this.setState({
                height: messageid.clientHeight
            })
        }, 50);
    }

    render() {
        const { height, onlineUsers } = this.state

        const Row = ({ index, style }) => {
            var item = onlineUsers[index];

            return <div>
                <div className='user-list-item' style={{
                    marginRight: '10px'
                }}>
                    <Badge dot={item.onLine} style={{
                        margin: '5px'
                    }}>
                        <Avatar shape="square" style={{
                        }} src={<img src={item.avatar} alt="avatar" />} />
                    </Badge>
                    <span style={{
                        fontSize: '15px',
                        marginLeft: '8px'
                    }}>{item.name}</span>

                </div>
            </div>
        };

        return (
            <div className="on-line">
                <div style={{
                    textAlign: 'center',
                    fontSize: '20px',
                    fontWeight: 'bold',
                }}>人员列表</div>
                <Divider></Divider>
                <div id='user-list' style={{
                    height: 'calc(100vh - 200px)',
                }}>
                    <FixedSizeList
                        height={height} // 列表可视区域的高度
                        itemCount={onlineUsers.length} // 列表数据长度
                        itemSize={35} // 列表行高
                        width={'100%'} //列表可视区域的宽度
                        layout='vertical'
                    >
                        {Row}
                    </FixedSizeList>
                </div>
            </div>
        )
    }
}


export default OnLine