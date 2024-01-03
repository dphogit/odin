import App from 'App';
import ErrorPage from 'components/ErrorPage';
import {
    DeviceDetailsPage,
    DevicesPage,
    getDeviceDetailsLoader,
    getDevicesLoader,
} from 'features/devices';
import { reactQueryClient } from 'providers/ReactQueryClientProvider';
import { Navigate, createBrowserRouter } from 'react-router-dom';
import { PathNames } from './util';

export const browserRouter = createBrowserRouter([
    {
        path: '/',
        element: <App />,
        errorElement: <ErrorPage />,
        children: [
            {
                errorElement: <ErrorPage />,
                path: PathNames.DEVICES,
                children: [
                    {
                        index: true,
                        element: <DevicesPage />,
                        loader: getDevicesLoader(reactQueryClient),
                    },
                    {
                        path: PathNames.DEVICE_DETAILS,
                        element: <DeviceDetailsPage />,
                        loader: getDeviceDetailsLoader(reactQueryClient),
                    },
                ],
            },
            {
                errorElement: <ErrorPage />,
                path: PathNames.UNITS,
                element: <h1>Units</h1>,
            },
        ],
    },
    {
        path: '*',
        element: <Navigate to="/" replace />,
    },
]);
