import { useEffect } from "react"
import { get }
    from '../../services/userService'
import { useState } from 'react';
// 引用antd组件
import { Avatar } from 'antd';

import PubSub from 'pubsub-js';

const token = localStorage.getItem('token')

const UserInfo = () => {

    const [user, setUser] = useState({
        avatar: '',
    })

    useEffect(() => {
        if (token) {
            get()
                .then(res => {
                    if (res.code === '200') {
                        localStorage.setItem('user', JSON.stringify(res.data))
                        PubSub.publish('user', res.data)
                        setUser(res.data)
                    }
                })
        }
    }, [])

    const renderUser = () => {
        return (
            <>
                <Avatar shape="square" size={{ xs: 24, sm: 32, md: 40, lg: 64, xl: 80, xxl: 100 }} src={<img src={user.avatar} alt="avatar" />} />
                <div style={{
                    fontSize: '15px',
                    marginTop: '8px'
                }}>
                    昵称：{user.name}
                </div>
                <div  style={{
                    fontSize: '15px',
                    marginTop: '25px'
                }}>
                    账号：{user.account}
                </div>
            </>
        )
    }

    return (
        <div style={{
            color: '#fff',
            fontSize: '20px',
        }}>
            {/* 使用if渲染 */}

            {user?.avatar ? renderUser() : null}
        </div>
    )
}


export default UserInfo