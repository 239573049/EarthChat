import { Divider, Avatar, Badge } from 'antd'
import './index.css'
import { useEffect, useState } from 'react'
import {
    getOnlineUsers,
} from '../../services/chatService'

import { List } from 'react-virtualized';

import AutoSizer from "react-virtualized-auto-sizer";

const OnLine = () => {

    const [onlineUsers, setOnlineUsers] = useState([]);

    const loadOnlineUsers = () => {
        getOnlineUsers()
            .then(x => {
                setOnlineUsers(x.data);
            })
    }

    useEffect(() => {
        loadOnlineUsers();
    }, []);

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
                                rowHeight={50}
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
            }}>人员列表</div>
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