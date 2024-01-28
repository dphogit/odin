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
import {
    deleteTemperatureAction,
    ManageTemperaturesPage,
    getManageTemperaturesPageDataLoader,
} from 'features/temperatures';

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
                children: [
                    {
                        index: true,
                        element: <ManageTemperaturesPage />,
                        loader: getManageTemperaturesPageDataLoader(reactQueryClient),
                    },
                    {
                        path: `${PathNames.TEMPERATURE_DETAILS}/${PathNames.TEMPERATURE_DELETE}`,
                        action: deleteTemperatureAction,
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
