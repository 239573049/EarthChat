import { Component } from 'react'
import AuthService from '../../services/authService'
import { Toast } from '@douyinfe/semi-ui';

export default class OAuth extends Component {


    constructor(props: any) {
        super(props)

        // 获取url的参数
        const queryString = window.location.search;

        // 创建URLSearchParams对象
        const params = new URLSearchParams(queryString);

        if (location.pathname === "/oauth/gitee") {
            var code = params.get('code');
            AuthService.giteeAuth(code!)
                .then(res => {
                    if (res.code === '200') {
                        localStorage.setItem('token', res.data);

                        window.location.href = '/';

                    } else {
                        Toast.error(res.message);
                    }
                })
        } else if (location.pathname === "/oauth/github") {

            var code = params.get('code');
            AuthService.gitHubAuth(code!)
                .then(res => {
                    if (res.code === '200') {
                        localStorage.setItem('token', res.data);

                        window.location.href = '/';

                    } else {
                        Toast.error(res.message);
                    }
                })
        }
    }

    render() {
        return (
            <div>OAuth</div>
        )
    }
}
