import { ActionFunctionArgs } from 'react-router-dom';
import { ApiUpdateDeviceDto, apiUpdateDeviceDtoSchema } from './types';
import axiosInstance from 'lib/axios';
import { reactQueryClient } from 'providers/ReactQueryClientProvider';

async function editDevice(id: string, body: ApiUpdateDeviceDto) {
    return axiosInstance.patch(`/devices/${id}`, body);
}

export async function editDeviceAction({ request, params }: ActionFunctionArgs) {
    const deviceId = params.deviceId;

    if (!deviceId) {
        throw new Error('No device ID found');
    }

    const formData = await request.formData();
    const requestBody = {
        name: formData.get('name') ?? undefined,
        description: formData.get('description') ?? undefined,
        location: formData.get('location') ?? undefined,
    };

    await editDevice(deviceId, apiUpdateDeviceDtoSchema.parse(requestBody));
    reactQueryClient.invalidateQueries({ queryKey: ['devices'] });

    return null;
}
