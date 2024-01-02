import axiosInstance from 'lib/axios';
import { z } from 'zod';

const getDevicesResponseSchema = z.array(
    z.object({
        id: z.number(),
        name: z.string(),
        description: z.string().optional(),
        location: z.string().optional(),
        createdAt: z.string().datetime({ offset: true }),
        updatedAt: z.string().datetime({ offset: true }),
    })
);

export async function getDevices() {
    const response = await axiosInstance.get('/devices');
    return getDevicesResponseSchema.parse(response.data);
}
