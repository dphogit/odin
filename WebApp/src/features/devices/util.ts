export const TIMERANGE_SEARCHPARAMS_KEY = 'days';

export const TimeRangeOptions = {
    WEEK: 'week',
    MONTH: 'month',
    YEAR: 'year',
} as const;

export type TimeRangeOptionKey = keyof typeof TimeRangeOptions;
export type TimeRangeOptionType = (typeof TimeRangeOptions)[TimeRangeOptionKey];

export const DEFAULT_TIMERANGE_OPTION = TimeRangeOptions.MONTH;

export function isTimeRangeWithinDropdownOptions(candidateTimeRange: string) {
    const validOptions: string[] = Object.values(TimeRangeOptions);
    return validOptions.includes(candidateTimeRange);
}

export function getTimeRangeFromUrlSearchParams(urlSearchParams: URLSearchParams): string {
    const timeRangeQueryValue = urlSearchParams.get(TIMERANGE_SEARCHPARAMS_KEY);

    if (!timeRangeQueryValue) {
        return DEFAULT_TIMERANGE_OPTION;
    }

    if (!isTimeRangeWithinDropdownOptions(timeRangeQueryValue)) {
        return DEFAULT_TIMERANGE_OPTION;
    }

    return timeRangeQueryValue;
}
