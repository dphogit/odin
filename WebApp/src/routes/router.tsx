import { Navigate, createBrowserRouter } from 'react-router-dom';
import App from 'App';
import ErrorPage from 'components/ErrorPage';
import { DevicePage } from 'features/devices';

export const browserRouter = createBrowserRouter([
    {
        path: '/',
        element: <App />,
        errorElement: <ErrorPage />,
        children: [
            {
                errorElement: <ErrorPage />,
                children: [
                    {
                        index: true,
                        element: <Navigate to="/devices" replace />,
                    },
                    {
                        path: 'devices',
                        element: <DevicePage />,
                    },
                    {
                        path: 'units',
                        element: <h1>Units</h1>,
                    },
                    {
                        path: '*',
                        element: <Navigate to="/" replace />,
                    },
                ],
            },
        ],
    },
]);
