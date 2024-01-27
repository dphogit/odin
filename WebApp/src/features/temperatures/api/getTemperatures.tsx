import axiosInstance from 'lib/axios';
import { LoaderReturnType } from 'types';
import { GetTemperaturesResponse, getTemperaturesResponseSchema } from './types';
import { LoaderFunctionArgs, useLoaderData, useSearchParams } from 'react-router-dom';
import { QueryClient, useQuery } from '@tanstack/react-query';
import { DEFAULT_PAGE, DEFAULT_PAGE_LIMIT, TimestampSort, getSearchFilterQueries } from '../util';

interface GetTemperatureOptions {
    page?: number;
    limit?: number;

    /**
     * Sort order of temperature results by their recorded timestamps.
     */
    timestampSort?: TimestampSort;
}

async function getTemperaturesWithDevice({
    page = DEFAULT_PAGE,
    limit = DEFAULT_PAGE_LIMIT,
    timestampSort = 'desc',
}: GetTemperatureOptions): Promise<GetTemperaturesResponse> {
    const urlSearchParams = new URLSearchParams();
    urlSearchParams.set('withDevice', 'true');
    urlSearchParams.set('page', page.toString());
    urlSearchParams.set('limit', limit.toString());
    urlSearchParams.set('sort', timestampSort);

    const endpoint = `/temperatures?${urlSearchParams.toString()}`;
    const response = await axiosInstance.get(endpoint);
    return getTemperaturesResponseSchema.parse(response.data);
}

const getTemperatureWithDeviceQuery = (options: GetTemperatureOptions) => {
    const { page = DEFAULT_PAGE, limit = DEFAULT_PAGE_LIMIT, timestampSort = 'desc' } = options;
    return {
        queryKey: ['temperatures', { withDevice: true, page, limit, sort: timestampSort }],
        queryFn: () => getTemperaturesWithDevice(options),
    };
};

export function getTemperaturesWithDeviceLoader(queryClient: QueryClient) {
    return async ({ request }: LoaderFunctionArgs) => {
        const urlSearchParams = new URL(request.url).searchParams;
        const { page, limit, timestampSort } = getSearchFilterQueries(urlSearchParams);
        const temperatureWithDeviceQuery = getTemperatureWithDeviceQuery({
            page,
            limit,
            timestampSort,
        });
        return queryClient.ensureQueryData(temperatureWithDeviceQuery);
    };
}

type GetTemperaturesLoaderReturnType = LoaderReturnType<typeof getTemperaturesWithDeviceLoader>;

export function useGetTemperaturesWithDeviceQuery() {
    const [searchParams] = useSearchParams();
    const initialData = useLoaderData() as GetTemperaturesLoaderReturnType;

    const searchFilters = getSearchFilterQueries(searchParams);

    return useQuery({
        ...getTemperatureWithDeviceQuery(searchFilters),
        initialData,
    });
}
