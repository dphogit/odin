import App from 'App';
import ErrorPage from 'components/ErrorPage';
import {
    DeviceDetailsPage,
    DevicesPage,
    getDeviceDetailsLoader,
    getDevicesLoader,
    editDeviceAction,
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
                index: true,
                element: <Navigate to={PathNames.DEVICES} replace />,
            },
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
                        action: editDeviceAction,
                    },
                ],
            },
        ],
    },
    {
        path: '*',
        element: <Navigate to="/" replace />,
    },
]);
