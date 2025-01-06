import Layout from "@/_layout";
import SkeletonLoading from "@/components/Loading";
import { lazy, Suspense } from "react";
import { createBrowserRouter } from "react-router-dom";

const Login = lazy(() => import("@/pages/login"));
const Oauth = lazy(() => import("@/pages/oauth"));

const routes = [
    {
        element: <Layout />,
        children: [
            {
                path: '/login',
                element: <Suspense fallback={<SkeletonLoading />}>
                    <Login />
                </Suspense>
            },
            {
                path: '/oauth',
                element: <Suspense fallback={<SkeletonLoading />}>
                    <Oauth />
                </Suspense>
            }
        ]
    }
] 

export default createBrowserRouter(routes);
