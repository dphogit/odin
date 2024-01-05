export const DAYS_SEARCH_PARAMS_KEY = 'days';

export const TimeRangeOptions = {
    LAST_7_DAYS: 7,
    LAST_30_DAYS: 30,
} as const;

export function isDaysWithinDropdownOptions(candidateDays: number) {
    const validValues: number[] = Object.values(TimeRangeOptions);
    return validValues.includes(candidateDays);
}

export function getDaysFromUrlSearchParams(urlSearchParams: URLSearchParams): number {
    const daysQueryValue = urlSearchParams.get(DAYS_SEARCH_PARAMS_KEY);

    if (!daysQueryValue) {
        return TimeRangeOptions.LAST_30_DAYS;
    }

    const days = parseInt(daysQueryValue);

    if (isNaN(days) || !isDaysWithinDropdownOptions(days)) {
        return TimeRangeOptions.LAST_30_DAYS;
    }

    return days;
}
