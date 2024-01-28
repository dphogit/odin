import { useSearchParams } from 'react-router-dom';

export const DEFAULT_PAGE = 1;
export const DEFAULT_PAGE_LIMIT = 25;
export const MAX_PAGE_LIMIT = 100;

export const DEFAULT_ROWS_PER_PAGE_OPTIONS = [10, 25, 50, 100];

export type TimestampSort = 'asc' | 'desc';
export type TemperatureRangeValues = [number, number];

export const GetTemperaturesSearchKeys = {
    PAGE: 'page',
    LIMIT: 'limit',
    TIMESTAMP_SORT: 'sort',
    MIN_TEMPERATURE: 'minValue',
    MAX_TEMPERATURE: 'maxValue',
    DEVICE_ID: 'deviceId',
} as const;

export type SearchFilterQueries = {
    page: number;
    limit: number;
    timestampSort: TimestampSort;
    minTemperature: number | null;
    maxTemperature: number | null;
    deviceIds: string[];
};

/**
 * Gets the search queries from the URLSearchParams object that can be used to filter the
 * temperatures results. If these keys are not found, then either the default values are used
 * or null.
 */
export function getSearchFilterQueries(urlSearchParams: URLSearchParams): SearchFilterQueries {
    const pageQueryValue = urlSearchParams.get(GetTemperaturesSearchKeys.PAGE);
    const limitQueryValue = urlSearchParams.get(GetTemperaturesSearchKeys.LIMIT);
    const timestampSortQueryValue = urlSearchParams.get(GetTemperaturesSearchKeys.TIMESTAMP_SORT);
    const minTemperatureQueryValue = urlSearchParams.get(GetTemperaturesSearchKeys.MIN_TEMPERATURE);
    const maxTemperatureQueryValue = urlSearchParams.get(GetTemperaturesSearchKeys.MAX_TEMPERATURE);
    const deviceIdQueryValues = urlSearchParams.getAll(GetTemperaturesSearchKeys.DEVICE_ID);

    const page = pageQueryValue ? parseInt(pageQueryValue) : DEFAULT_PAGE;
    const limit = limitQueryValue ? parseInt(limitQueryValue) : DEFAULT_PAGE_LIMIT;
    const timestampSort: TimestampSort =
        timestampSortQueryValue?.toLowerCase() === 'asc' ? 'asc' : 'desc';
    const minTemperature = minTemperatureQueryValue ? parseFloat(minTemperatureQueryValue) : null;
    const maxTemperature = maxTemperatureQueryValue ? parseFloat(maxTemperatureQueryValue) : null;

    return {
        page,
        limit,
        timestampSort,
        minTemperature,
        maxTemperature,
        deviceIds: deviceIdQueryValues,
    };
}

/**
 * Hook that wraps {@link getSearchFilterQueries} to returns the search queries from the current
 * URLSearchParams.
 */
export function useGetSearchFilterQueries() {
    const [searchParams] = useSearchParams();
    return getSearchFilterQueries(searchParams);
}
