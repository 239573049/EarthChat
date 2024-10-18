import './App.css'
import { RouterProvider } from 'react-router-dom'
import mainRoutes from './routes'
import { memo } from 'react'

const App = memo(() => {
  return (
    <RouterProvider router={mainRoutes}>

    </RouterProvider>
  )
})

export default App