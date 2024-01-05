import { QueryClient, useQuery } from '@tanstack/react-query';
import axiosInstance from 'lib/axios';
import { useLoaderData, useParams, useSearchParams } from 'react-router-dom';
import { LoaderFnArgsTypedParams } from 'routes/util';
import { LoaderReturnType } from 'types';
import { z } from 'zod';
import {
    ApiDeviceDto,
    ApiTemperatureDto,
    apiDeviceDtoSchema,
    apiTemperatureDtoSchema,
} from '../types';
import { TimeRangeOptions, isDaysWithinDropdownOptions, getDaysFromUrlSearchParams } from '../util';

async function getDeviceDetails(id: string): Promise<ApiDeviceDto> {
    const response = await axiosInstance.get(`devices/${id}`);
    return apiDeviceDtoSchema.parse(response.data);
}

async function getDeviceTemperatures(
    id: string,
    days: number = TimeRangeOptions.LAST_30_DAYS
): Promise<ApiTemperatureDto[]> {
    const endpoint = `/devices/${id}/temperatures?days=${days}`;
    const response = await axiosInstance.get(endpoint);
    return z.array(apiTemperatureDtoSchema).parse(response.data);
}

interface GetDeviceDetailsResponse {
    device: ApiDeviceDto & { temperatures: ApiTemperatureDto[] };
    days: number;
}

async function getDeviceDetailsWithTemperatures(
    id: string,
    days: number = TimeRangeOptions.LAST_30_DAYS
): Promise<GetDeviceDetailsResponse> {
    const [device, temperatures] = await Promise.all([
        getDeviceDetails(id),
        getDeviceTemperatures(id, days),
    ]);

    // Make sure the days value is within our valid options, otherwise we set it to the default.
    const daysResponseValue = isDaysWithinDropdownOptions(days)
        ? days
        : TimeRangeOptions.LAST_30_DAYS;

    return {
        device: { ...device, temperatures },
        days: daysResponseValue,
    };
}

const getDeviceDetailsQuery = (id: string, days: number = TimeRangeOptions.LAST_30_DAYS) => ({
    queryKey: ['devices', id, 'days', days],
    queryFn: async () => getDeviceDetailsWithTemperatures(id, days),
});

type DeviceDetailsLoaderArgs = LoaderFnArgsTypedParams<'DEVICE_DETAILS'>;

export function useGetDeviceDetailsQuery() {
    const [searchParams] = useSearchParams();
    const { deviceId } = useParams<DeviceDetailsLoaderArgs['params']>();
    const initialData = useLoaderData() as GetDeviceDetailsLoaderReturnType;

    if (!deviceId) throw new Error('No device id provided');

    const days = getDaysFromUrlSearchParams(searchParams);

    return useQuery({
        ...getDeviceDetailsQuery(deviceId, days),
        initialData,
    });
}

export function getDeviceDetailsLoader(queryClient: QueryClient) {
    return async ({ params, request }: DeviceDetailsLoaderArgs) => {
        if (!params.deviceId) throw new Error('No device id provided');

        const urlSearchParams = new URL(request.url).searchParams;
        const days = getDaysFromUrlSearchParams(urlSearchParams);

        return queryClient.ensureQueryData(getDeviceDetailsQuery(params.deviceId, days));
    };
}

type GetDeviceDetailsLoaderReturnType = LoaderReturnType<typeof getDeviceDetailsLoader>;
