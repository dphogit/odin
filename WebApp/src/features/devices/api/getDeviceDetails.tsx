import { QueryClient, useQuery } from '@tanstack/react-query';
import axiosInstance from 'lib/axios';
import { useLoaderData, useParams } from 'react-router-dom';
import { LoaderFnArgsTypedParams } from 'routes/util';
import { LoaderReturnType } from 'types';
import { IDevice } from '../types';

async function getDeviceDetails(id: string): Promise<IDevice> {
    const response = await axiosInstance.get(`/devices/${id}`);
    return response.data; // TODO Validate response with zod
}

const getDeviceDetailsQuery = (id: string) => ({
    queryKey: ['devices', id],
    queryFn: async () => getDeviceDetails(id),
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
