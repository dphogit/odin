import { LoaderReturnType } from 'types';
import { GetTemperatureOptions, getTemperaturesWithDevice } from './getTemperatures';
import { getDevices } from 'features/devices';
import { LoaderFunctionArgs, useLoaderData } from 'react-router-dom';
import {
    DEFAULT_PAGE,
    DEFAULT_PAGE_LIMIT,
    SearchFilterQueries,
    getSearchFilterQueries,
    useGetSearchFilterQueries,
} from '../util';
import { QueryClient, useQuery } from '@tanstack/react-query';

interface GetManageTemperaturePageDataOptions {
    getTemperaturesOptions: GetTemperatureOptions;
}

async function getManageTemperaturesPageData(options: GetManageTemperaturePageDataOptions) {
    const [devices, temperatures] = await Promise.all([
        getDevices(),
        getTemperaturesWithDevice(options.getTemperaturesOptions),
    ]);

    return { devices, temperatures };
}

type GetManageTemperaturesPageLoaderReturnType = LoaderReturnType<
    typeof getManageTemperaturesPageDataLoader
>;

const getPageQuery = (options: GetManageTemperaturePageDataOptions) => {
    const {
        page = DEFAULT_PAGE,
        limit = DEFAULT_PAGE_LIMIT,
        timestampSort = 'desc',
        minValue,
        maxValue,
        deviceIds,
    } = options.getTemperaturesOptions;

    return {
        queryKey: [
            'temperatures',
            { withDevice: true, page, limit, sort: timestampSort, minValue, maxValue, deviceIds },
        ],
        queryFn: () => getManageTemperaturesPageData(options),
    };
};

export function getManageTemperaturesPageDataLoader(queryClient: QueryClient) {
    return async ({ request }: LoaderFunctionArgs) => {
        const urlSearchParams = new URL(request.url).searchParams;
        const searchFilters = getSearchFilterQueries(urlSearchParams);
        const query = getPageQuery({
            getTemperaturesOptions: transformSearchFiltersToRequestOptions(searchFilters),
        });
        return queryClient.ensureQueryData(query);
    };
}

export function useGetManageTemperaturesPageDataQuery() {
    const searchFilters = useGetSearchFilterQueries();
    const initialData = useLoaderData() as GetManageTemperaturesPageLoaderReturnType;

    return useQuery({
        ...getPageQuery({
            getTemperaturesOptions: transformSearchFiltersToRequestOptions(searchFilters),
        }),
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
    const { minTemperature, maxTemperature, ...filters } = searchFilters;

    return {
        ...filters,
        minValue: minTemperature ?? undefined,
        maxValue: maxTemperature ?? undefined,
    };
}
