import {
    createBrowserRouter,
    RouterProvider,
} from "react-router-dom";

import MainLayout from "./layout/main-layout";
import Login from "./pages/login";
import './App.scss'
import Home from "./pages/home";
import User from "./pages/user";

const router = createBrowserRouter([{
    path: "/",
    element: <MainLayout />,
    children: [{
        path: "",
        element: <Home />,
    }, {
        path: "/user",
        element: <User />,

    }],
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

