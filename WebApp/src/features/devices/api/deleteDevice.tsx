import axiosInstance from 'lib/axios';
import { reactQueryClient } from 'providers/ReactQueryClientProvider';
import { ActionFunctionArgs, redirect } from 'react-router-dom';
import { PathNames } from 'routes/util';

async function deleteDevice(id: string) {
    return axiosInstance.delete(`/devices/${id}`);
}

export async function deleteDeviceAction({ params }: ActionFunctionArgs) {
    const deviceId = params.deviceId;

    if (!deviceId) {
        throw new Error('No device ID found');
    }

    await deleteDevice(deviceId);

    // Make sure we EXACTLY invalidate the "devices" query, otherwise we will prompt a refetch
    // of the device details page, which will cause an error since the device no longer exists.
    reactQueryClient.invalidateQueries({ queryKey: ['devices'], exact: true });

    return redirect(`/${PathNames.DEVICES}`);
}
