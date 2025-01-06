import { useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { message, Spin, Card, Result } from 'antd';
import { GithubOutlined, LoadingOutlined } from '@ant-design/icons';
import Icon from '@ant-design/icons';
import { oauth } from '@/apis/authorize';
import { GiteeAI } from '@lobehub/icons';
import { bindOauth } from '@/apis/user';

const OAuthCallback = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();

  useEffect(() => {
    const handleOAuth = async () => {
      try {
        const type = searchParams.get('type');
        const code = searchParams.get('code');
        const bind = localStorage.getItem('bind');

        if (bind) {
          const response = await bindOauth({
            type: type || '',
            code: code || '',
            state: searchParams.get('state') || '',
            redirectUri: window.origin + "/oauth?type=" + type
          });

          if (response.code === 200) {
            message.success('绑定成功');
            navigate('/profile/oauth');
          } else {
            message.error('绑定失败');
            navigate('/profile/oauth');
          }

          return;
        }

        const redirect = localStorage.getItem('redirect');

        if (!type || !code) {
          message.error('授权参数不完整');
          navigate('/login');
          return;
        }

        const response = await oauth(type, code, "", window.origin + "/oauth?type=" + type);
        if (response.code === 200) {
          message.success('登录成功');
          localStorage.setItem('token', response.data);
          if (redirect) {
            // 使用重定向URL
            localStorage.removeItem('redirect');
            const searchParams = new URLSearchParams();
            searchParams.set('token', response.data);
            location.href = redirect + "?" + searchParams.toString();
          } else {
            navigate('/');
          }
        } else {
          throw new Error(response.message || '授权失败');
        }
      } catch (error) {
        message.error('授权失败：' + (error as Error).message);
        navigate('/login');
      }
    };

    handleOAuth();
  }, [searchParams, navigate]);

  // 自定义 Loading 图标
  const antIcon = <LoadingOutlined style={{ fontSize: 24 }} spin />;

  return (
    <div style={{
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      minHeight: '100vh',
    }} className="flex items-center justify-center min-h-screen bg-gray-50">
      <Card
        style={{
          width: 400,
          boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.1)',
          borderRadius: '8px'
        }}
        className="flex flex-col items-center" // 添加居中样式
      >
        <Result
          className="flex flex-col items-center" // 添加居中样式
          icon={
            <div className="flex flex-col items-center justify-center"> {/* 修改为flex布局 */}
              {searchParams.get('type') === 'github' && (
                <GithubOutlined
                  style={{
                    fontSize: 48,
                    marginBottom: 24,
                    display: 'block' // 确保图标块级显示
                  }}
                />
              )}
              {
                searchParams.get('type') === 'gitee' && (
                  <Icon style={{
                    fontSize: 48,
                    marginBottom: 24,
                    display: 'block' // 确保图标块级显示
                  }} component={GiteeAI} />
                )
              }
              <Spin indicator={antIcon} />
            </div>
          }
          title={
            <div className="text-center text-xl font-medium">
              正在处理授权
            </div>
          }
          subTitle={
            <div className="text-center space-y-2">
              <p className="text-gray-500">请稍候，我们正在验证您的身份</p>
              <p className="text-gray-400 text-sm">
                授权方式：{searchParams.get('type')?.toUpperCase() || '未知'}
              </p>
            </div>
          }
        />
      </Card>
    </div>
  );
};

export default OAuthCallback;