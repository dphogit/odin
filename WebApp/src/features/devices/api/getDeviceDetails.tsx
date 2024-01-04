import { QueryClient, useQuery } from '@tanstack/react-query';
import axiosInstance from 'lib/axios';
import { useLoaderData, useParams } from 'react-router-dom';
import { LoaderFnArgsTypedParams } from 'routes/util';
import { LoaderReturnType } from 'types';
import {
    ApiDeviceDto,
    ApiTemperatureDto,
    apiDeviceDtoSchema,
    apiTemperatureDtoSchema,
} from '../types';
import { z } from 'zod';

async function getDeviceDetails(id: string): Promise<ApiDeviceDto> {
    const response = await axiosInstance.get(`devices/${id}`);
    return apiDeviceDtoSchema.parse(response.data);
}

async function getDeviceTemperatures(id: string): Promise<ApiTemperatureDto[]> {
    const response = await axiosInstance.get(`/devices/${id}/temperatures`);
    return z.array(apiTemperatureDtoSchema).parse(response.data);
}

type GetDeviceDetailsResponse = ApiDeviceDto & { temperatures: ApiTemperatureDto[] };

async function getDeviceDetailsWithTemperatures(id: string): Promise<GetDeviceDetailsResponse> {
    const [device, temperatures] = await Promise.all([
        getDeviceDetails(id),
        getDeviceTemperatures(id),
    ]);
    return { ...device, temperatures };
}

const getDeviceDetailsQuery = (id: string) => ({
    queryKey: ['devices', id],
    queryFn: async () => getDeviceDetailsWithTemperatures(id),
});

type DeviceDetailsLoaderArgs = LoaderFnArgsTypedParams<'DEVICE_DETAILS'>;

export function useGetDeviceDetailsQuery() {
    const { deviceId } = useParams<DeviceDetailsLoaderArgs['params']>();
    const initialData = useLoaderData() as GetDeviceDetailsLoaderReturnType;

    if (!deviceId) throw new Error('No device id provided');

    return useQuery({
        ...getDeviceDetailsQuery(deviceId),
        initialData,
    });
}

export function getDeviceDetailsLoader(queryClient: QueryClient) {
    return async ({ params }: DeviceDetailsLoaderArgs) => {
        if (!params.deviceId) throw new Error('No device id provided');
        return queryClient.ensureQueryData(getDeviceDetailsQuery(params.deviceId));
    };
}

type GetDeviceDetailsLoaderReturnType = LoaderReturnType<typeof getDeviceDetailsLoader>;
