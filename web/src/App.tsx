import {
  createBrowserRouter,
  RouterProvider,
} from "react-router-dom";

import MainLayout from "./layout/main-layout";
import Login from "./pages/login";

const router = createBrowserRouter([{
  path: "/",
  element: <MainLayout />,
  children: [],
},{
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
