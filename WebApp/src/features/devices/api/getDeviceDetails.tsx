import { QueryClient, useQuery } from '@tanstack/react-query';
import axiosInstance from 'lib/axios';
import { useLoaderData, useParams, useSearchParams } from 'react-router-dom';
import { LoaderFnArgsTypedParams } from 'routes/util';
import {
    ApiTimeSeriesDataPointDto,
    ApiTimeSeriesDataPointResponse,
    LoaderReturnType,
    apiTimeSeriesDataPointResponseSchema,
} from 'types';
import {
    DEFAULT_TIMERANGE_OPTION,
    TimeRangeOptionType,
    getTimeRangeFromUrlSearchParams,
    isTimeRangeWithinDropdownOptions,
} from '../util';
import { ApiDeviceDto, apiDeviceDtoSchema } from './types';

async function getDeviceDetails(id: string): Promise<ApiDeviceDto> {
    const response = await axiosInstance.get(`devices/${id}`);
    return apiDeviceDtoSchema.parse(response.data);
}

async function getDeviceTemperatureTimeSeriesData(
    id: string,
    timeRange: string
): Promise<ApiTimeSeriesDataPointResponse> {
    const endpoint = `devices/${id}/temperatures/time-series?timeRange=${timeRange}`;
    const response = await axiosInstance.get(endpoint, {
        headers: {
            'X-Timezone-Offset': -new Date().getTimezoneOffset(),
        },
    });
    return apiTimeSeriesDataPointResponseSchema.parse(response.data);
}

interface GetDeviceDetailsResponse {
    device: ApiDeviceDto & { temperatures: ApiTimeSeriesDataPointDto[] };
    timeRange: TimeRangeOptionType;
}

async function getDeviceDetailsWithTemperatureTimeSeriesData(
    id: string,
    option: string
): Promise<GetDeviceDetailsResponse> {
    const [device, temperatures] = await Promise.all([
        getDeviceDetails(id),
        getDeviceTemperatureTimeSeriesData(id, option),
    ]);

    // Make sure the days value is within our valid options, otherwise we set it to the default.
    const timeRange = isTimeRangeWithinDropdownOptions(option)
        ? (option as TimeRangeOptionType)
        : DEFAULT_TIMERANGE_OPTION;

    return {
        device: { ...device, temperatures },
        timeRange,
    };
}

const getDeviceDetailsQuery = (id: string, timeRange: string) => ({
    queryKey: ['devices', id, { timeRange }],
    queryFn: async () => getDeviceDetailsWithTemperatureTimeSeriesData(id, timeRange),
});

type DeviceDetailsLoaderArgs = LoaderFnArgsTypedParams<'DEVICE_DETAILS'>;

export function useGetDeviceDetailsQuery() {
    const [searchParams] = useSearchParams();
    const { deviceId } = useParams<DeviceDetailsLoaderArgs['params']>();
    const initialData = useLoaderData() as GetDeviceDetailsLoaderReturnType;

    if (!deviceId) throw new Error('No device id provided');

    const timeRange = getTimeRangeFromUrlSearchParams(searchParams);

    return useQuery({
        ...getDeviceDetailsQuery(deviceId, timeRange),
        initialData,
    });
}

export function getDeviceDetailsLoader(queryClient: QueryClient) {
    return async ({ params, request }: DeviceDetailsLoaderArgs) => {
        if (!params.deviceId) throw new Error('No device id provided');

        const urlSearchParams = new URL(request.url).searchParams;
        const timeRange = getTimeRangeFromUrlSearchParams(urlSearchParams);

        return queryClient.ensureQueryData(getDeviceDetailsQuery(params.deviceId, timeRange));
    };
}

type GetDeviceDetailsLoaderReturnType = LoaderReturnType<typeof getDeviceDetailsLoader>;
