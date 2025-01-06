import { ThemeProvider } from "@lobehub/ui";
import { memo } from "react";
import { Outlet } from "react-router-dom";

const Layout = memo(() => {
        return <ThemeProvider>
            <Outlet />
    </ThemeProvider>;
})

export default Layout;
