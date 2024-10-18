import { createBrowserRouter, RouteObject } from "react-router-dom";
import GlobalLayout from "../layouts/GlobalLayout";
import AuthPage from "../pages/auth";
import WelcomePage from "../pages/welcome";

const routes: RouteObject[] = [];

routes.push({
    path:'/',
    element:<WelcomePage />
});

// 添加全局组件
routes.push({
    path: '/auth',
    element: <AuthPage />,
});


const mainRoutes = createBrowserRouter([
    {
        element: <GlobalLayout />,
        children: routes
    }
]);


export default mainRoutes;