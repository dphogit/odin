import React, { useState } from 'react';
import ClearIcon from '@mui/icons-material/Clear';
import FilterAltIcon from '@mui/icons-material/FilterAlt';
import { Box, Button, Stack } from '@mui/joy';
import TemperatureRangeFilter from './TemperatureRangeFilter';
import { useSearchParams } from 'react-router-dom';
import {
    DEFAULT_PAGE,
    GetTemperaturesSearchKeys,
    TemperatureRangeValues,
    getSearchFilterQueries,
} from '../util';

const DEFAULT_TEMPERATURE_RANGE_VALUES: TemperatureRangeValues = [0, 50];
const DEFAULT_TEMPERATURE_RANGE_MIN_VALUE = DEFAULT_TEMPERATURE_RANGE_VALUES[0];
const DEFAULT_TEMPERATURE_RANGE_MAX_VALUE = DEFAULT_TEMPERATURE_RANGE_VALUES[1];

export default function TemperatureFilterOptions() {
    const [searchParams, setSearchParams] = useSearchParams();
    const { minTemperature, maxTemperature } = getSearchFilterQueries(searchParams);

    const [temperatureRangeValues, setTemperatureRangeValues] = useState<TemperatureRangeValues>([
        minTemperature ?? DEFAULT_TEMPERATURE_RANGE_MIN_VALUE,
        maxTemperature ?? DEFAULT_TEMPERATURE_RANGE_MAX_VALUE,
    ]);

    const handleTemperatureRangeFilterChange = (_: Event, newValues: TemperatureRangeValues) => {
        setTemperatureRangeValues(newValues);
    };

    const handleClear = () => {
        searchParams.set(GetTemperaturesSearchKeys.PAGE, DEFAULT_PAGE.toString());
        searchParams.delete(GetTemperaturesSearchKeys.MIN_TEMPERATURE);
        searchParams.delete(GetTemperaturesSearchKeys.MAX_TEMPERATURE);
        setTemperatureRangeValues(DEFAULT_TEMPERATURE_RANGE_VALUES);
        setSearchParams(searchParams, { replace: true });
    };

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        const [minValue, maxValue] = temperatureRangeValues;
        searchParams.set(GetTemperaturesSearchKeys.PAGE, DEFAULT_PAGE.toString());
        searchParams.set(GetTemperaturesSearchKeys.MIN_TEMPERATURE, minValue.toString());
        searchParams.set(GetTemperaturesSearchKeys.MAX_TEMPERATURE, maxValue.toString());
        setSearchParams(searchParams, { replace: true });
    };

    return (
        <form onSubmit={handleSubmit}>
            <Stack spacing="32px">
                <TemperatureRangeFilter
                    values={temperatureRangeValues}
                    maxValue={DEFAULT_TEMPERATURE_RANGE_MAX_VALUE}
                    onChange={handleTemperatureRangeFilterChange}
                />
                <Box mt="24px" display="flex" alignItems="center" columnGap="12px">
                    <Button
                        variant="outlined"
                        color="neutral"
                        startDecorator={<ClearIcon />}
                        sx={{ flex: 1 }}
                        onClick={handleClear}
                    >
                        Clear
                    </Button>
                    <Button
                        variant="solid"
                        color="primary"
                        startDecorator={<FilterAltIcon />}
                        sx={{ flex: 1 }}
                        type="submit"
                    >
                        Apply
                    </Button>
                </Box>
            </Stack>
        </form>
    );
}
