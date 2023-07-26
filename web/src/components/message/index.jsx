import './index.css';
import { List, CellMeasurerCache, CellMeasurer } from 'react-virtualized';
import React, { useState, useEffect } from 'react';
import { Button, Mentions, Avatar, Spin, message, FloatButton, Image, Upload, Card } from 'antd';
import {
    SmileOutlined,
    FileOutlined,
    MessageOutlined,
    FileImageOutlined
} from '@ant-design/icons';
import config from '../../config';

import { GetList } from '../../services/chatService';

import AutoSizer from "react-virtualized-auto-sizer";
import { useRef } from 'react';

const cache = new CellMeasurerCache({
    defaultHeight: 100,
    fixedWidth: true,
});

let user = {

};

const Message = () => {
    const [chatMessage, setChatMessage] = useState([]);
    const [value, setValue] = useState('');
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(10);
    const [loading, setLoading] = useState(false);
    const [unread, setUnread] = useState(0);
    const listRef = useRef([]);


    useEffect(() => {

        getlist();

        // 监听消息
        window.connection.on('ReceiveMessage', (message) => {
            // 设置未读消息
            var v = JSON.parse(message);
            console.log('更新未读', v);
            if (v.userId !== window?.user?.id) {
                setUnread(oldUnread => oldUnread + 1);
            }

            setChatMessage(oldMessage => [...oldMessage, v]);
        });

        return () => {
            window.connection.off('ReceiveMessage');
        };
    }, []);


    const onScroll = (ListOnScrollProps) => {
        if (ListOnScrollProps.scrollTop === 0) {

            setPage(oldPage => oldPage + 1);
            getlist();
        }
    };

    const getlist = () => {
        setLoading(true);
        GetList(page, pageSize)
            .then(res => {
                if (res.code === '200') {
                    // 将res.data.result的数据加入到message前面,并且根据id去重
                    setChatMessage(oldMessage => [...new Map([...res.data.result, ...oldMessage].map(item => [item.id, item])).values()]);

                    // 如果返回的数据长度为0，说明没有更多数据了，将page-1
                    if (res.data.result.length === 0) {
                        setPage(oldPage => oldPage - 1);
                    }
                    setLoading(false);
                }
            }).catch(() => {
                setLoading(false);
            })

    }

    const scrollToRow = () => {
        var messageList = document.getElementById('message-list');
        if (messageList) {
            messageList.scrollTop = messageList.scrollHeight + 1000;
        }
        setUnread(0);
    };


    const sendMessage = async () => {
        if (value === '') {
            return;
        }
        await send('SendMessage', value, 0);

        setTimeout(() => {
            var messageList = document.getElementById('message-list');
            if (messageList) {
                messageList.scrollTop = messageList.scrollHeight + 1000;
            }
        }, 100);

        setValue('');
    };


    const send = async (value, type) => {
        if (value === '') {
            return;
        }
        await window.connection.send('SendMessage', value, type);
    }

    let userJson = localStorage.getItem('user');
    if (userJson) {
        try {
            user = JSON.parse(userJson);
        } catch (e) {

        }
    }

    const download = (url) => {
        // 打开新标签下载
        var a = document.createElement('a');
        a.href = url;
        a.target = '_blank';
        a.click();

    }

    const rendetContent = (item) => {
        const className = user?.id === item.user.id ? ' message-item-content-user' : '';
        if (item.type === 0) {
            return (
                <div className={'message-item-content' + className} style={{ display: 'inline-block', marginBottom: '20px', marginLeft: '10px' }}>
                    {item.content}
                </div>
            )
        } else if (item.type === 1) {
            return (
                <div className={'message-item-content ' + className} style={{ display: 'inline-block', marginBottom: '20px', marginLeft: '10px' }}>
                    <Image
                        width={200}
                        src={item.content}
                    />
                </div>
            )
        } else if (item.type === 2) {
            var name = item.content.split('/')[item.content.split('/').length - 1];
            return (
                <Card
                    hoverable
                    className='message-item-content '
                    title={name}
                    style={{ width: 300, display: 'inline-block', marginBottom: '20px', marginLeft: '10px' }}
                >
                    <Avatar>
                        <FileOutlined />
                    </Avatar>
                    <Button onClick={() => download(item.content)} style={{
                        float: 'right'
                    }}>下载</Button>
                </Card>
            )
        }
    }

    function rowRenderer({
        key, // Unique key within array of rows
        index, // Index of row within collection
        isScrolling, // The List is currently being scrolled
        isVisible, // This row is visible within the List (eg it is not an overscanned row)
        style, // Style object to be applied to row (to position it)
        parent
    }) {
        const item = chatMessage[index];
        return (
            <CellMeasurer
                cache={cache}
                columnIndex={0}
                key={key}
                parent={parent}
                rowIndex={index}
            >
                {({ measure }) => (
                    <div key={item.id} onLoad={measure} style={{ ...style }}>
                        <Avatar size='large' style={{ float: 'left' }} src={<img src={item.user.avatar} alt="avatar" />} />
                        <div style={{ float: 'left', marginLeft: '10px', width: 'calc(100% - 50px)', display: "inline-block" }}>
                            {item.user.name}
                        </div>
                        {rendetContent(item)}
                    </div>
                )}
            </CellMeasurer>
        );
    }

    const ListComponent = () => {
        return (
            <>
                <Spin style={{
                    position: 'absolute',
                    width: '50%'
                }} spinning={loading}>

                </Spin>
                <div style={{ height: 'calc(100% - 240px)' }}>
                    <AutoSizer>
                        {({ height, width }) => (
                            <List
                                id='message-list'
                                ref={listRef}
                                onFocus={() => setUnread(0)}
                                width={width}
                                onScroll={onScroll}
                                height={height}
                                rowCount={chatMessage.length}
                                rowHeight={cache.rowHeight}
                                rowRenderer={rowRenderer}
                            />
                        )}
                    </AutoSizer>
                </div >
            </>
        );
    };

    const onKeyDown = (e) => {
        // 按下enter键发送消息
        if (e.keyCode === 13) {
            if (value === '') {
                return;
            }
            sendMessage();
        }

    }

    const onImage = async ({
        file
    }) => {
        if (file.response) {
            if (file.response.code === '200') {
                await send(file.response.data, 1);
            } else {
                message.error(file.response.message);
            }
        }
    }


    const onFile = async ({
        file
    }) => {
        if (file.response) {
            if (file.response.code === '200') {
                await window.connection.send('SendMessage', file.response.data, 2);
            } else {
                message.error(file.response.message);
            }
        }
    }

    return (
        <>
            <div className='message'>
                {unread > 0 ? <FloatButton onClick={scrollToRow} icon={<MessageOutlined />} badge={{ count: unread, overflowCount: 999 }} /> : ''}
                {ListComponent()}
                <div className='chat-send'>
                    <div className='chat-tools'>
                        <Button size='small' style={{ marginLeft: '10px', border: 'none' }} icon={<SmileOutlined />} />
                        <Upload
                            listType='image'
                            headers={{ Authorization: 'Bearer ' + localStorage.getItem('token') }}
                            onChange={onFile}
                            action={config.API_URL + '/api/v1/Files/Upload'}
                            showUploadList={false}>
                            <Button size='small' style={{ marginLeft: '10px', border: 'none' }} icon={<FileOutlined />} />
                        </Upload>
                        <Upload
                            listType='image'
                            accept='image/*'
                            headers={{ Authorization: 'Bearer ' + localStorage.getItem('token') }}
                            onChange={onImage}
                            action={config.API_URL + '/api/v1/Files/Upload'}
                            showUploadList={false}>
                            <Button size='small' style={{ marginLeft: '10px', border: 'none' }} icon={<FileImageOutlined />} />
                        </Upload>
                    </div>
                    <Mentions
                        autoSize
                        onFocus={() => setUnread(0)}
                        rows={5}
                        value={value}
                        maxLength={2000}
                        onChange={(v) => setValue(v)}
                        className='chat-input'
                        onKeyDown={onKeyDown}
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