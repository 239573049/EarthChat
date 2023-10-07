import { Avatar, Card } from '@douyinfe/semi-ui';
import Meta from '@douyinfe/semi-ui/lib/es/card/meta';
import React, { Component } from 'react'

interface IProps {
    content: string
}

interface IState {
    uriRender: any
}

export default class RenderText extends Component<IProps, IState> {

    state: Readonly<IState> = {
        uriRender: undefined
    }

    async findURIs() {
        const { content } = this.props;

        const uriRegex = /https?:\/\/[^\s]+/g;
        const uri = content.match(uriRegex) || [];

        const uriInfos = [] as {
            title: string | undefined,
            icon: string,
            uri: string,
            description:string|null
        }[]

        for (let i = 0; i < uri.length; i++) {
            const item = uri[i];
            const info = await this.getSiteInfo(item);
            uriInfos.push(info);
        }

        const uriRender = uriInfos.map((item, index) => {
            return <div onClick={() => window.open(item.uri)} style={{
                marginBottom: '5px'
            }}>
                <Card
                    key={index}
                    shadows='hover'
                    style={{ maxWidth: 320, width: 320 }}
                    bodyStyle={{
                        display: 'flex',
                        alignItems: 'center',
                        cursor: 'pointer',
                        userSelect: 'none',
                    }}
                >
                    <Avatar
                        size="default"
                        src={item.icon}
                    />
                    <div style={{
                        fontSize: '18px',
                        fontWeight: 'bold',
                        textAlign:'left',
                        marginLeft:'5px',
                        display:'flex'
                    }}>{item.title}</div>
                    <div style={{
                        fontSize:'12px',
                        marginLeft:'5px',
                        width: 190
                    }}>
                        {item.description}
                    </div>
                </Card>
            </div>
        })

        this.setState({
            uriRender: uriRender
        })

    }

    componentDidMount(): void {
        this.findURIs();
    }

    async getSiteInfo(uri: string) {
        // 获取网页内容
        const response = await fetch(uri);
        const text = await response.text();

        // 使用DOM解析器来解析HTML内容
        const parser = new DOMParser();
        const doc = parser.parseFromString(text, 'text/html');

        // 获取标题
        const title = doc?.querySelector('title')?.innerText;

        // 尝试获取图标
        let icon = doc.querySelector("link[rel='shortcut icon']") as any;
        if (!icon) {
            icon = doc.querySelector("link[rel='icon']");
        }
        const metaDescription = doc.querySelector('meta[name="description"]');
        const description = metaDescription ? metaDescription.getAttribute('content') : '';

        const iconHref = icon ? icon.href : null;

        return {
            title: title,
            icon: iconHref,
            uri: uri,
            description
        };
    }

    render() {
        const { content } = this.props;
        const { uriRender } = this.state;
        return (
            <>
                {content}
                {uriRender}
            </>
        )
    }
}
