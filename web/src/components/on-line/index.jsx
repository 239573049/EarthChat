import { Divider, Avatar, Badge } from 'antd'
import './index.css'
import { useEffect, useState } from 'react'
import {
    getOnlineUsers,
} from '../../services/chatService'
import PubSub from 'pubsub-js';
import { List } from 'react-virtualized';

import AutoSizer from "react-virtualized-auto-sizer";

const updateOnline = 'UpdateOnline';

const OnLine = () => {

    const [onlineUsers, setOnlineUsers] = useState([]);
    const [onlineSize, setOnlineSize] = useState(0);

    const loadOnlineUsers = () => {
        getOnlineUsers()
            .then(x => {
                PubSub.publish("userLists", x.data)
                setOnlineUsers(x.data);
            })
    }

    useEffect(() => {
        loadOnlineUsers();
        PubSub.subscribe(updateOnline, UpdateOnline);
        return () => {
            PubSub.unsubscribe(updateOnline);
        }
    }, []);

    // 监听OnLine消耗的事件

    const UpdateOnline = (name,i) => {
        loadOnlineUsers();
        setOnlineSize(i);
    }

    const rowRenderer = ({ index, style }) => {
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


    const ListComponent = () => {
        return (
            <>
                <div style={{ height: '100%' }}>
                    <AutoSizer>
                        {({ height, width }) => (
                            <List
                                width={width}
                                height={height}
                                rowCount={onlineUsers.length}
                                rowHeight={42}
                                rowRenderer={rowRenderer}
                            />
                        )}
                    </AutoSizer>
                </div >
            </>
        );
    };

    return (
        <div className="on-line">
            <div style={{
                textAlign: 'center',
                fontSize: '20px',
                fontWeight: 'bold',
            }}>在线人数({onlineSize})</div>
            <Divider></Divider>
            <div id='user-list' style={{
                height: 'calc(100vh - 200px)',
            }}>
                {ListComponent()}
            </div>
        </div>
    )
}

export default OnLine