import {
    createBrowserRouter,
    RouterProvider,
    useNavigate
} from "react-router-dom";
import MainLayout from "./layout/main-layout";
import Login from "./pages/login";
import './App.scss'
import Home from "./pages/home";
import User from "./pages/user";
import InvitationGroup from "./pages/invitation-group";
import Register from "./pages/register";
import OAuth from "./pages/o-auth";
import Content from "./pages/home/content";

const router = createBrowserRouter([{
    path: "/",
    element: <MainLayout />,
    children: [{
        path: "/",
        element: <Home />,
        children:[
            {
                path:'',
                element:<Content />
            }
        ]
    }, {
        path: "/user",
        element: <User />,
    }],
}, {
    path: "/invitation-group",
    element: <InvitationGroup />,
    children: [],
}, {
    path: "/register",
    element: <Register />,
    children: [],
}, {
    path: "/oauth/gitee",
    element: <OAuth />,
    children: [],
}, {
    path: "/oauth/github",
    element: <OAuth />,
    children: [],
}, {
    path: "/login",
    element: <Login />,
    children: [],
}]);

function App() {
    return (
        <RouterProvider router={router} />
    )
}

export default App

