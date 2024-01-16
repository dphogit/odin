import axiosInstance from 'lib/axios';
import { LoaderReturnType } from 'types';
import { GetTemperaturesResponse, getTemperaturesResponseSchema } from './types';
import { LoaderFunctionArgs, useLoaderData, useSearchParams } from 'react-router-dom';
import { QueryClient, useQuery } from '@tanstack/react-query';
import { DEFAULT_PAGE, DEFAULT_PAGE_LIMIT, getPageAndLimitFromUrlSearchParams } from '../util';

async function getTemperaturesWithDevice(
    page = DEFAULT_PAGE,
    limit = DEFAULT_PAGE_LIMIT
): Promise<GetTemperaturesResponse> {
    const endpoint = `/temperatures?withDevice=true&page=${page}&limit=${limit}`;
    const response = await axiosInstance.get(endpoint);
    return getTemperaturesResponseSchema.parse(response.data);
}

const getTemperatureWithDeviceQuery = (page = DEFAULT_PAGE, limit = DEFAULT_PAGE_LIMIT) => ({
    queryKey: ['temperatures', { withDevice: true, page, limit }],
    queryFn: () => getTemperaturesWithDevice(page, limit),
});

export function getTemperaturesWithDeviceLoader(queryClient: QueryClient) {
    return async ({ request }: LoaderFunctionArgs) => {
        const urlSearchParams = new URL(request.url).searchParams;
        const { page, limit } = getPageAndLimitFromUrlSearchParams(urlSearchParams);
        const temperatureWithDeviceQuery = getTemperatureWithDeviceQuery(page, limit);
        return queryClient.ensureQueryData(temperatureWithDeviceQuery);
    };
}

type GetTemperaturesLoaderReturnType = LoaderReturnType<typeof getTemperaturesWithDeviceLoader>;

export function useGetTemperaturesWithDeviceQuery() {
    const [searchParams] = useSearchParams();
    const initialData = useLoaderData() as GetTemperaturesLoaderReturnType;

    const { page, limit } = getPageAndLimitFromUrlSearchParams(searchParams);

    return useQuery({
        ...getTemperatureWithDeviceQuery(page, limit),
        initialData,
    });
}
