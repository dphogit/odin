import { QueryClient, useQuery } from '@tanstack/react-query';
import axiosInstance from 'lib/axios';
import { useLoaderData, useParams } from 'react-router-dom';
import { LoaderFnArgsTypedParams } from 'routes/util';
import { LoaderReturnType } from 'types';
import { apiDeviceDtoSchema } from '../types';

interface GetDeviceOptions {
    withTemperatures?: boolean;
}

async function getDeviceDetails(id: string, options?: GetDeviceOptions) {
    const endpoint = options?.withTemperatures
        ? `/devices/${id}?withTemperatures=true`
        : `/devices/${id}`;

    const response = await axiosInstance.get(endpoint);

    return options?.withTemperatures
        ? apiDeviceDtoSchema.required({ temperatures: true }).parse(response.data)
        : apiDeviceDtoSchema.parse(response.data);
}

const getDeviceDetailsQuery = (id: string, options?: GetDeviceOptions) => ({
    queryKey: ['devices', id],
    queryFn: async () => getDeviceDetails(id, options),
});

type DeviceDetailsLoaderArgs = LoaderFnArgsTypedParams<'DEVICE_DETAILS'>;

export function useGetDeviceDetailsQuery(options: GetDeviceOptions) {
    const { deviceId } = useParams<DeviceDetailsLoaderArgs['params']>();
    const initialData = useLoaderData() as GetDeviceDetailsLoaderReturnType;

    if (!deviceId) throw new Error('No device id provided');

    return useQuery({
        ...getDeviceDetailsQuery(deviceId, options),
        initialData,
    });
}

export function getDeviceDetailsLoader(queryClient: QueryClient) {
    return async ({ params, request }: DeviceDetailsLoaderArgs) => {
        if (!params.deviceId) throw new Error('No device id provided');

        const url = new URL(request.url);
        const withTemperatures = url.searchParams.get('withTemperatures')?.toLowerCase() === 'true';

        return queryClient.ensureQueryData(
            getDeviceDetailsQuery(params.deviceId, { withTemperatures })
        );
    };
}

type GetDeviceDetailsLoaderReturnType = LoaderReturnType<typeof getDeviceDetailsLoader>;
