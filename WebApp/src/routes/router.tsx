import App from 'App';
import ErrorPage from 'components/ErrorPage';
import {
    DeviceDetailsPage,
    DevicesPage,
    getDeviceDetailsLoader,
    getDevicesLoader,
    addDeviceAction,
    deleteDeviceAction,
    editDeviceAction,
} from 'features/devices';
import { reactQueryClient } from 'providers/ReactQueryClientProvider';
import { Navigate, createBrowserRouter } from 'react-router-dom';
import { PathNames } from './util';
import { ManageTemperaturesPage, getTemperaturesWithDeviceLoader } from 'features/temperatures';

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
                        action: addDeviceAction,
                    },
                    {
                        // TODO - combine the edit and delete actions into one action for the route
                        path: PathNames.DEVICE_DETAILS,
                        element: <DeviceDetailsPage />,
                        loader: getDeviceDetailsLoader(reactQueryClient),
                        action: editDeviceAction,
                    },
                    {
                        path: `${PathNames.DEVICE_DETAILS}/${PathNames.DEVICE_DELETE}`,
                        action: deleteDeviceAction,
                    },
                ],
            },
            {
                errorElement: <ErrorPage />,
                path: PathNames.TEMPERATURES,
                element: <ManageTemperaturesPage />,
                loader: getTemperaturesWithDeviceLoader(reactQueryClient),
            },
        ],
    },
    {
        path: '*',
        element: <Navigate to="/" replace />,
    },
]);