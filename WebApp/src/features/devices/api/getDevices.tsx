import { QueryClient, useQuery } from '@tanstack/react-query';
import axiosInstance from 'lib/axios';
import { useLoaderData } from 'react-router-dom';
import { LoaderReturnType } from 'types';
import { z } from 'zod';
import { apiDeviceDtoSchema } from './types';

const getDevicesResponseSchema = z.array(apiDeviceDtoSchema);
type GetDevicesResponse = z.infer<typeof getDevicesResponseSchema>;

export async function getDevices(): Promise<GetDevicesResponse> {
    const response = await axiosInstance.get('/devices');
    return getDevicesResponseSchema.parse(response.data);
}

const getDevicesQuery = { queryKey: ['devices'], queryFn: getDevices };

export function useGetDevicesQuery() {
    const initialData = useLoaderData() as GetDevicesLoaderReturnType;

    return useQuery({
        ...getDevicesQuery,
        initialData,
    });
}

export function getDevicesLoader(queryClient: QueryClient) {
    return async () => {
        return queryClient.ensureQueryData(getDevicesQuery);
    };
}

type GetDevicesLoaderReturnType = LoaderReturnType<typeof getDevicesLoader>;
