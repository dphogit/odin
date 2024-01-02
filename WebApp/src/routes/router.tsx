import { Navigate, createBrowserRouter } from 'react-router-dom';
import App from 'App';
import ErrorPage from 'components/ErrorPage';
import { DevicePage, getDevicesLoader } from 'features/devices';
import { reactQueryClient } from 'providers/ReactQueryClientProvider';

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
                        loader: getDevicesLoader(reactQueryClient),
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
