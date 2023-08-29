import { Avatar, Button, Divider, Input, List } from '@douyinfe/semi-ui';
import React, { Component } from 'react';
import { IconSearch, IconChevronRight } from '@douyinfe/semi-icons';
import './index.scss'


interface IState {
    middleWidth: number;
    selectid: number;
    slidingBlock: number;
}


class User extends Component<any, IState> {

    state: Readonly<IState> = {
        middleWidth: 230,
        selectid: 0,
        slidingBlock: 0
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
        this.setState({
            slidingBlock: 0
        })
    }

    selectGroup() {
        this.setState({
            slidingBlock: 50
        })
    }

    render() {
        const { middleWidth, slidingBlock } = this.state;
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
                        <div className='inform'>
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
                            // 边框
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
                        
                    </div>
                </div>
                <div className="resizer" onMouseDown={this.handleMouseDown}></div>
                <div className="right" style={{ width: rightWidth }}>
                </div>
            </>
        );
    }
}

export default User;