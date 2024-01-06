import { ActionFunctionArgs } from 'react-router-dom';
import axiosInstance from 'lib/axios';
import { ApiCreateDeviceDto, apiCreateDeviceDtoSchema, apiDeviceDtoSchema } from './types';
import { reactQueryClient } from 'providers/ReactQueryClientProvider';

async function addDevice(createDeviceDto: ApiCreateDeviceDto) {
    const response = await axiosInstance.post('/devices', createDeviceDto);
    return apiDeviceDtoSchema.parse(response.data);
}

/**
 * Gets the required request body from the form data, trimming all values and converting empty
 * string values to null.
 */
function cleanFormData(formData: FormData): Record<string, string | null> {
    const fieldNames = ['name', 'description', 'location'];

    const requestBody: Record<string, string | null> = {};

    fieldNames.forEach((fieldName) => {
        const rawValue = formData.get(fieldName);
        const trimmedValue = typeof rawValue === 'string' ? rawValue.trim() : null;
        requestBody[fieldName] = trimmedValue || null;
    });

    return requestBody;
}

export async function addDeviceAction({ request }: ActionFunctionArgs) {
    const formData = await request.formData();
    const requestBody = cleanFormData(formData);
    const addedDevice = await addDevice(apiCreateDeviceDtoSchema.parse(requestBody));
    reactQueryClient.invalidateQueries({ queryKey: ['devices'] });
    return addedDevice;
}
