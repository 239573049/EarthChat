import './index.css';
import { FixedSizeList as List } from 'react-window';
import React, { useState, useEffect, useRef, useCallback } from 'react';
import { Button, Mentions, Avatar } from 'antd';
import {
    SmileOutlined,
    FileOutlined,
    FileImageOutlined
} from '@ant-design/icons';

import AutoSizer from "react-virtualized-auto-sizer";

let rowHeights = 0;

const Message = () => {
    const [message, setMessage] = useState([]);
    const [value, setValue] = useState('');
    const [listRef, setListRef] = useState(useRef());
    //   const rowHeights = useRef([]);

    useEffect(() => {
        // 监听消息
        window.connection.on('ReceiveMessage', (message) => {
            setMessage(oldMessage => [...oldMessage, message]);
            console.log('收到消息', message);
        });

        return () => {
            window.connection.off('ReceiveMessage');
        };
    }, []);

    const onScroll = (ListOnScrollProps) => {
        console.log(ListOnScrollProps);
    };

    const sendMessage = async () => {
        // if (value === '') {
        //     return;
        // }
        // console.log('发送消息');
        await window.connection.send('SendMessage', value, 0);

        setValue('');
    };

    const setRowHeight = useCallback((index, size) => {
        if (typeof listRef.current.resetAfterIndex === "function") {
            listRef.current.resetAfterIndex(index);
        }
        // rowHeights = { ...rowHeights, [index]: size };
        rowHeights += size;
    }, [listRef]);

    const renderRow = ({ index: i, style }) => {
        const item = message[i];
        const rowRef = useRef();

        useEffect(() => {
            if (rowRef.current) {
                setRowHeight(i, rowRef.current.getBoundingClientRect().height);
                if (i === 0) {
                    rowHeights = rowRef.current.getBoundingClientRect().height;
                    return;
                }
                rowRef.current.style.top = `${rowHeights - rowRef.current.getBoundingClientRect().height }px`;
                console.log(rowRef.current.style.top, `${rowHeights}px`);
            }
        }, [rowRef.current, setRowHeight]);

        return (
            <div ref={rowRef} key={item.id} style={{ ...style, height: "auto" }}>
                <Avatar size='large' style={{ float: 'left' }} src={<img src={item.User.Avatar} alt="avatar" />} />
                <div style={{ float: 'left', marginLeft: '10px', width: 'calc(100% - 50px)', display: "inline-block" }}>
                    {item.User.Name}
                </div>
                <div className='message-item-content message-item-content-user' style={{ display: 'inline-block', marginBottom: '20px', marginLeft: '10px' }}>
                    {item.Content}
                </div>
            </div>
        );
    };


    const ListComponent = () => {
        return (
            <div style={{ height: 'calc(100% - 240px)' }}>
                <AutoSizer>
                    {({ height, width }) => (
                        <List
                            ref={listRef}
                            height={height}
                            className="List"
                            itemCount={message.length}
                            itemSize={70}
                            width={width}
                            overscanCount={20}
                        >
                            {renderRow}
                        </List>
                    )}
                </AutoSizer>
            </div>
        );
    };

    return (
        <>
            <div className='message'>
                {ListComponent()}
                <div className='chat-send'>
                    <div className='chat-tools'>
                        <Button size='small' style={{ marginLeft: '10px', border: 'none' }} icon={<SmileOutlined />} />
                        <Button size='small' style={{ marginLeft: '10px', border: 'none' }} icon={<FileOutlined />} />
                        <Button size='small' style={{ marginLeft: '10px', border: 'none' }} icon={<FileImageOutlined />} />
                    </div>
                    <Mentions
                        autoSize
                        rows={5}
                        value={value}
                        onChange={(v) => setValue(v)}
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
                    <div style={{ float: "right", marginRight: '10px' }}>
                        <Button onClick={async () => await sendMessage()} type='primary'>发送</Button>
                    </div>
                </div>
            </div>
        </>
    );
};

export default Message;