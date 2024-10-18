import { ConfigProvider } from "antd"
import zhCN from 'antd/locale/zh_CN';
import 'dayjs/locale/zh-cn';
import { Outlet } from "react-router-dom";

interface GlobalLayoutProps {

}

const GlobalLayout = ({  }: GlobalLayoutProps) => {

    return (
        <ConfigProvider
            locale={zhCN}
            theme={{

            }}
        >
            <Outlet />
        </ConfigProvider>
    )
}

export default GlobalLayout