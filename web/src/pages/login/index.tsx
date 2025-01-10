import { memo, useState, useEffect } from "react";
import { Form, Input, Button, message } from "antd";
import { Logo } from '@lobehub/ui';
import './index.css';
// import { getVerificationCode } from "@/apis/verification"; // 引入获取验证码的方法
// import { getToken } from "@/apis/authorize"; // 引入获取 token 的方法
import { useNavigate, useLocation} from "react-router-dom";
// import { useUserStore } from "@/store/user";
import { Card } from "antd";
import { Github } from 'lucide-react';
import Divider from "@lobehub/ui/es/Form/components/FormDivider";
import { GITHUB_CLIENT_ID,GITEE_CLIENT_ID } from "@/const/oauth";
import { GiteeAI } from '@lobehub/icons';

const Login = memo(() => {
    const navigate = useNavigate();
    const location = useLocation();
    // const [signIn] = useUserStore((s) => [s.signIn]);
    const [loading, setLoading] = useState(false);
    const [captcha, setCaptcha] = useState<{
        key: string;
        code: string;
    }>(); // 初始验证码为空

    // 获取重定向URL
    const searchParams = new URLSearchParams(location.search);
    const redirectUrl = searchParams.get('redirect') || null;

    const onFinish = async (values: any) => {

        try {
            setLoading(true);
            // const token = await getToken({
            //     username: values.username,
            //     password: values.password,
            //     captchaKey: captcha!.key, // 传递验证码 key
            //     captchaCode: values.captcha, // 传递用户输入的验证码
            // });
            // if (token.code === 200) {
            //     message.success('登录成功！');
            //     signIn(token.data);
            //     // 使用重定向URL
            //     if (redirectUrl) {
            //         window.location.href = redirectUrl + "?token=" + token.data;
            //     } else {
            //         navigate('/');
            //     }
            // } else {
            //     message.error(token.message);
            // }
        } catch (error) {
            message.error('登录失败，请检查您的信息是否正确！');
        } finally {
            setLoading(false);
        }
    };

    const refreshCaptcha = async () => {
        try {
            // const { data } = await getVerificationCode("login"); // 
            // setCaptcha(data); // 更新验证码
        } catch (error) {
            message.error("获取验证码失败，请稍后重试！"); // 错误提示
        }
    };

    useEffect(() => {
        refreshCaptcha();
        // 获取重定向参数
        const redirectUrl = localStorage.getItem('redirect');
        if (redirectUrl) {
            // set到路由
            const searchParams = new URLSearchParams(location.search);
            searchParams.set('redirect', redirectUrl);
            navigate(`?${searchParams.toString()}`);
            localStorage.removeItem('redirect');
        }
    }, []);

    function handlerGithub() {
        // 记录重定向参数到localStorage
        if (redirectUrl) {
            localStorage.setItem('redirect', redirectUrl);
        }
        // 跳转github登录
        const redirectUri = window.origin + "/oauth?type=github&" + location.search;
        const githubAuthUrl = `https://github.com/login/oauth/authorize?client_id=${GITHUB_CLIENT_ID}&redirect_uri=${redirectUri}&scope=user`;
        window.location.href = githubAuthUrl;
    }

    function handlerGitee(){
        // 记录重定向参数到localStorage
        if (redirectUrl) {
            localStorage.setItem('redirect', redirectUrl);
        }
        // 跳转gitee登录
        const redirectUri = window.origin + "/oauth?type=gitee&" + location.search;
        const giteeAuthUrl = `https://gitee.com/oauth/authorize?client_id=${GITEE_CLIENT_ID}&redirect_uri=${redirectUri}&response_type=code`;
        window.location.href = giteeAuthUrl;
    }

    return (
        <Card className="login-container">
            <Logo className="login-logo" />
            <h2 className="welcome-title">欢迎登录TokenAI</h2>
            <Form
                name="login"
                onFinish={onFinish}
                layout="vertical"
            >
                <Form.Item
                    label="账号"
                    name="username"
                    rules={[{ required: true, message: '请输入账号！' }]}
                >
                    <Input placeholder="账号" />
                </Form.Item>
                <Form.Item
                    label="密码"
                    name="password"
                    rules={[{ required: true, message: '请输入密码！' }]}
                >
                    <Input.Password placeholder="密码" />
                </Form.Item>
                <Form.Item
                    label="验证码"
                    name="captcha"
                    rules={[{ required: true, message: '请输入验证码！' }]}
                >
                    <Input
                        suffix={
                            <img
                                className="captcha-image"
                                src={captcha?.code}
                                alt="captcha"
                                onClick={refreshCaptcha} // 
                                style={{ cursor: "pointer" }}
                            />
                        }
                        placeholder="验证码" />
                </Form.Item>
                <Form.Item>
                    <Button 
                        loading={loading}
                        type='default' htmlType="submit" style={{ width: '100%' }}>
                        登录
                    </Button>
                </Form.Item>
            </Form>
            <div className="third-party-login">
                <Divider>
                    其他登录方式
                </Divider>
                <div className="login-buttons">
                    <Button 
                        loading={loading}
                        icon={<Github />} 
                        onClick={() => handlerGithub()}>
                    </Button>
                    <Button 
                        loading={loading}
                        icon={<GiteeAI />} 
                        onClick={()=>handlerGitee()}>

                    </Button>
                </div>
            </div>
        </Card>
    );
});

Login.displayName = "Login";

export default Login;