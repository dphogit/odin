import axiosInstance from 'lib/axios';
import { LoaderReturnType } from 'types';
import { GetTemperaturesResponse, getTemperaturesResponseSchema } from './types';
import { LoaderFunctionArgs, useLoaderData } from 'react-router-dom';
import { QueryClient, useQuery } from '@tanstack/react-query';
import {
    DEFAULT_PAGE,
    DEFAULT_PAGE_LIMIT,
    GetTemperaturesSearchKeys,
    SearchFilterQueries,
    TimestampSort,
    getSearchFilterQueries,
    useGetSearchFilterQueries,
} from '../util';

interface GetTemperatureOptions {
    page?: number;

    /**
     * Number of records to return per page.
     */
    limit?: number;

    /**
     * Sort order of temperature results by their recorded timestamps.
     */
    timestampSort?: TimestampSort;

    /**
     * Minimum temperature (inclusive) value to filter by.
     */
    minValue?: number;

    /**
     * Maximum temperature (inclusive) value to filter by.
     */
    maxValue?: number;
}

async function getTemperaturesWithDevice({
    page = DEFAULT_PAGE,
    limit = DEFAULT_PAGE_LIMIT,
    timestampSort = 'desc',
    minValue,
    maxValue,
}: GetTemperatureOptions): Promise<GetTemperaturesResponse> {
    const urlSearchParams = new URLSearchParams();

    urlSearchParams.set('withDevice', 'true');
    urlSearchParams.set(GetTemperaturesSearchKeys.PAGE, page.toString());
    urlSearchParams.set(GetTemperaturesSearchKeys.LIMIT, limit.toString());
    urlSearchParams.set(GetTemperaturesSearchKeys.TIMESTAMP_SORT, timestampSort);

    if (minValue != undefined) {
        urlSearchParams.set(GetTemperaturesSearchKeys.MIN_TEMPERATURE, minValue.toString());
    }

    if (maxValue != undefined) {
        urlSearchParams.set(GetTemperaturesSearchKeys.MAX_TEMPERATURE, maxValue.toString());
    }

    const endpoint = `/temperatures?${urlSearchParams.toString()}`;
    const response = await axiosInstance.get(endpoint);
    return getTemperaturesResponseSchema.parse(response.data);
}

const getTemperatureWithDeviceQuery = (options: GetTemperatureOptions) => {
    const {
        page = DEFAULT_PAGE,
        limit = DEFAULT_PAGE_LIMIT,
        timestampSort = 'desc',
        minValue,
        maxValue,
    } = options;

    return {
        queryKey: [
            'temperatures',
            { withDevice: true, page, limit, sort: timestampSort, minValue, maxValue },
        ],
        queryFn: () => getTemperaturesWithDevice(options),
    };
};

export function getTemperaturesWithDeviceLoader(queryClient: QueryClient) {
    return async ({ request }: LoaderFunctionArgs) => {
        const urlSearchParams = new URL(request.url).searchParams;
        const searchFilters = getSearchFilterQueries(urlSearchParams);

        const temperatureWithDeviceQuery = getTemperatureWithDeviceQuery(
            transformSearchFiltersToRequestOptions(searchFilters)
        );

        return queryClient.ensureQueryData(temperatureWithDeviceQuery);
    };
}

type GetTemperaturesLoaderReturnType = LoaderReturnType<typeof getTemperaturesWithDeviceLoader>;

export function useGetTemperaturesWithDeviceQuery() {
    const searchFilters = useGetSearchFilterQueries();
    const initialData = useLoaderData() as GetTemperaturesLoaderReturnType;
    return useQuery({
        ...getTemperatureWithDeviceQuery(transformSearchFiltersToRequestOptions(searchFilters)),
        initialData,
    });
}

/**
 * Helper function to transform the client side query parameters to the equivalent query parameters
 * that the API expects in the request URL.
 */
function transformSearchFiltersToRequestOptions(
    searchFilters: SearchFilterQueries
): GetTemperatureOptions {
    const { page, limit, timestampSort, minTemperature, maxTemperature } = searchFilters;

    return {
        page,
        limit,
        timestampSort,
        minValue: minTemperature ?? undefined,
        maxValue: maxTemperature ?? undefined,
    };
}
