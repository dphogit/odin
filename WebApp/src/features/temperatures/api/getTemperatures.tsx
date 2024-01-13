import axiosInstance from 'lib/axios';
import { apiTemperatureWithDeviceDtoSchema } from './types';
import { z } from 'zod';
import { useLoaderData } from 'react-router-dom';
import { QueryClient, useQuery } from '@tanstack/react-query';
import { LoaderReturnType } from 'types';

const getTemperaturesResponseSchema = z.array(apiTemperatureWithDeviceDtoSchema);
type GetTemperaturesResponse = z.infer<typeof getTemperaturesResponseSchema>;

async function getTemperaturesWithDevice(): Promise<GetTemperaturesResponse> {
    const response = await axiosInstance.get('/temperatures?withDevice=true');
    return getTemperaturesResponseSchema.parse(response.data);
}

const getTemperatureWithDeviceQuery = {
    queryKey: ['temperatures', { withDevice: true }],
    queryFn: getTemperaturesWithDevice,
};

export function getTemperaturesWithDeviceLoader(queryClient: QueryClient) {
    return async () => {
        return queryClient.ensureQueryData(getTemperatureWithDeviceQuery);
    };
}

type GetTemperaturesLoaderReturnType = LoaderReturnType<typeof getTemperaturesWithDeviceLoader>;

export function useGetTemperaturesWithDeviceQuery() {
    const initialData = useLoaderData() as GetTemperaturesLoaderReturnType;
    return useQuery({
        ...getTemperatureWithDeviceQuery,
        initialData,
    });
}
