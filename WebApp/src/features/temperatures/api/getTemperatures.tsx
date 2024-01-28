import axiosInstance from 'lib/axios';
import {
    DEFAULT_PAGE,
    DEFAULT_PAGE_LIMIT,
    GetTemperaturesSearchKeys,
    TimestampSort,
} from '../util';
import { GetTemperaturesResponse, getTemperaturesResponseSchema } from './types';

export interface GetTemperatureOptions {
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

    /**
     * Device ids to filter temperatures that are only measured by these specified devices.
     */
    deviceIds: string[];
}

export async function getTemperaturesWithDevice({
    page = DEFAULT_PAGE,
    limit = DEFAULT_PAGE_LIMIT,
    timestampSort = 'desc',
    minValue,
    maxValue,
    deviceIds,
}: GetTemperatureOptions): Promise<GetTemperaturesResponse> {
    const urlSearchParams = new URLSearchParams();

    urlSearchParams.set('withDevice', 'true');
    urlSearchParams.set(GetTemperaturesSearchKeys.PAGE, page.toString());
    urlSearchParams.set(GetTemperaturesSearchKeys.LIMIT, limit.toString());
    urlSearchParams.set(GetTemperaturesSearchKeys.TIMESTAMP_SORT, timestampSort);

    deviceIds.forEach((deviceId) => {
        urlSearchParams.append(GetTemperaturesSearchKeys.DEVICE_ID, deviceId);
    });

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
