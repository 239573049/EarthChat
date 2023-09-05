import { Divider, Icon, Modal, TabPane, Tabs, Image } from '@douyinfe/semi-ui'
import React, { Component } from 'react'

interface IProps {
    visible: boolean,
    onCancel: any,
}

const body = document.body;

const tong = () => {
    return <svg viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="5842" width="15" height="15"><path d="M324.93 410.8H166.71c-40.73 0-73.87-33.14-73.87-73.86V178.72c0-40.73 33.14-73.86 73.87-73.86h158.22c40.73 0 73.87 33.14 73.87 73.86v158.22c-0.01 40.72-33.14 73.86-73.87 73.86zM166.71 173.85c-2.68 0-4.87 2.18-4.87 4.86v158.22c0 2.68 2.18 4.86 4.87 4.86h158.22c2.68 0 4.87-2.18 4.87-4.86V178.72c0-2.68-2.18-4.86-4.87-4.86H166.71z" p-id="5843"></path><path d="M324.93 410.8H166.71c-40.79 0-73.86-33.07-73.86-73.86V178.72c0-40.79 33.07-73.86 73.86-73.86h158.22c40.79 0 73.87 33.07 73.87 73.87v158.22c-0.01 40.78-33.08 73.85-73.87 73.85zM191.61 173.85c-16.44 0-29.76 13.32-29.76 29.76v108.42c0 16.44 13.32 29.76 29.76 29.76h108.43c16.44 0 29.76-13.32 29.76-29.76V203.61c0-16.44-13.32-29.76-29.76-29.76H191.61zM903.4 192.26H487.35c-19.05 0-34.5-15.45-34.5-34.5s15.45-34.5 34.5-34.5H903.4c19.05 0 34.5 15.45 34.5 34.5s-15.45 34.5-34.5 34.5zM903.4 390.03H487.35c-19.05 0-34.5-15.45-34.5-34.5s15.45-34.5 34.5-34.5H903.4c19.05 0 34.5 15.45 34.5 34.5s-15.45 34.5-34.5 34.5zM903.4 619.72H487.35c-19.05 0-34.5-15.45-34.5-34.5s15.45-34.5 34.5-34.5H903.4c19.05 0 34.5 15.45 34.5 34.5s-15.45 34.5-34.5 34.5zM903.4 817.48H487.35c-19.05 0-34.5-15.45-34.5-34.5s15.45-34.5 34.5-34.5H903.4c19.05 0 34.5 15.45 34.5 34.5s-15.45 34.5-34.5 34.5zM324.93 819.53H166.71c-40.79 0-73.86-33.07-73.86-73.86V587.45c0-40.79 33.07-73.86 73.86-73.86h158.22c40.79 0 73.87 33.07 73.87 73.87v158.22c-0.01 40.78-33.08 73.85-73.87 73.85zM191.61 582.58c-16.44 0-29.76 13.32-29.76 29.76v108.42c0 16.44 13.32 29.76 29.76 29.76h108.43c16.44 0 29.76-13.32 29.76-29.76V612.35c0-16.44-13.32-29.76-29.76-29.76H191.61z" p-id="5844"></path></svg>
}

const theme = localStorage.getItem('theme-mode');
if (theme) {
    if (theme === "light") {
        body.removeAttribute('theme-mode');
    } else if (theme === "dark") {
        body.setAttribute('theme-mode', 'dark');
    }
}

export default class Setting extends Component<IProps> {

    constructor(props: IProps) {
        super(props);


    }

    changeTheme = (ev: any) => {

        var d = document as any;
        d.documentElement.style.setProperty('--x', ev.clientX + 'px')
        d.documentElement.style.setProperty('--y', ev.clientY + 'px')

        if (body.hasAttribute('theme-mode')) {
            d.startViewTransition(() => {
                body.removeAttribute('theme-mode');
                localStorage.setItem('theme-mode', 'light')
                this.forceUpdate();
            });
        } else {
            d.startViewTransition(() => {
                body.setAttribute('theme-mode', 'dark');
                localStorage.setItem('theme-mode', 'dark')
                this.forceUpdate();
            });
        }
    }


    render() {
        const dark = body.hasAttribute('theme-mode');

        const style = {
            border: '2px solid blue',
        }

        return (
            <Modal width={'70%'} height={600} visible={this.props.visible} onCancel={() => this.props.onCancel()}>
                <Tabs style={{
                    height:"100%"
                }} tabPosition="left" type='button'>
                    <TabPane
                        style={{
                            backgroundColor: 'var(--chat-menu-background-color)',
                            height:"100%"
                        }}
                        tab={
                            <span>
                                <Icon svg={tong()} />
                                通用
                            </span>
                        }
                        itemKey="1"
                    >
                        <div style={{ padding: '0 24px' }}>
                            <h3>通用</h3>
                            <Divider></Divider>
                            <div style={{
                                backgroundColor: "var(--setting-tong-theme-background-color)",
                                height: "120px",
                                margin: "10px"
                            }}>
                                <div style={{
                                    padding: '25px',
                                    float: 'left',
                                }}>
                                    <div onClick={(e) => this.changeTheme(e)} style={{
                                        borderRadius: '5px',
                                        height: '63px',
                                        ...(!dark ? style : '')
                                    }}>
                                        <Image src='/img/light.jpg' preview={false} />
                                    </div>
                                </div>
                                <div style={{
                                    padding: '25px',
                                    float: "left"
                                }}>
                                    <div onClick={(e) => this.changeTheme(e)} style={{
                                        borderRadius: '5px',
                                        height: '67px',
                                        ...(dark ? style : '')
                                    }}>
                                        <Image src='/img/dark.jpg' preview={false} />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </TabPane>
                </Tabs>
            </Modal>
        )
    }
}
