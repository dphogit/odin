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
    reactQueryClient.invalidateQueries({ queryKey: ['devices'] });

    return redirect(`/${PathNames.DEVICES}`);
}
