import { QueryClient, useQuery } from '@tanstack/react-query';
import axiosInstance from 'lib/axios';
import { z } from 'zod';
import { IDevice } from '../types';
import { useLoaderData } from 'react-router-dom';
import { LoaderReturnType } from 'types';

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

export async function getDevices(): Promise<IDevice[]> {
    const response = await axiosInstance.get('/devices');
    return getDevicesResponseSchema.parse(response.data);
}

export const getDevicesQuery = { queryKey: ['devices'], queryFn: getDevices };

export function useGetDevicesQuery() {
    const initialData = useLoaderData() as GetDevicesLoaderReturnType;

    return useQuery({
        ...getDevicesQuery,
        initialData,
    });
}

export type GetDevicesLoaderReturnType = LoaderReturnType<typeof getDevicesLoader>;

export function getDevicesLoader(queryClient: QueryClient) {
    return async () => {
        return queryClient.ensureQueryData(getDevicesQuery);
    };
}
