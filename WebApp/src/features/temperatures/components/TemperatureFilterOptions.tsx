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
import { DeviceSelect } from '.';
import { ApiDeviceDto } from 'features/devices';
import { DeviceSelectOnChange } from './DeviceSelect';

const DEFAULT_TEMPERATURE_RANGE_VALUES: TemperatureRangeValues = [0, 50];
const DEFAULT_TEMPERATURE_RANGE_MIN_VALUE = DEFAULT_TEMPERATURE_RANGE_VALUES[0];
const DEFAULT_TEMPERATURE_RANGE_MAX_VALUE = DEFAULT_TEMPERATURE_RANGE_VALUES[1];

interface TemperatureFilterOptionsProps {
    devices: ApiDeviceDto[];
}

/**
 * Filters out device ids that do not exist in the list of devices. This makes sure only ids with
 * a corresponding valid device can be selected.
 */
function initializeSelectedDeviceIds(deviceIds: string[], devices: ApiDeviceDto[]): string[] {
    const deviceExistsWithId = (id: string) => devices.some((d) => d.id.toString() === id);
    return deviceIds.filter(deviceExistsWithId);
}

export default function TemperatureFilterOptions({ devices }: TemperatureFilterOptionsProps) {
    const [searchParams, setSearchParams] = useSearchParams();
    const { minTemperature, maxTemperature, deviceIds } = getSearchFilterQueries(searchParams);

    const [temperatureRangeValues, setTemperatureRangeValues] = useState<TemperatureRangeValues>([
        minTemperature ?? DEFAULT_TEMPERATURE_RANGE_MIN_VALUE,
        maxTemperature ?? DEFAULT_TEMPERATURE_RANGE_MAX_VALUE,
    ]);

    const [selectedDeviceIds, setSelectedDeviceIds] = useState<string[]>(() =>
        initializeSelectedDeviceIds(deviceIds, devices)
    );

    const handleTemperatureRangeFilterChange = (_: Event, newValues: TemperatureRangeValues) => {
        setTemperatureRangeValues(newValues);
    };

    const handleSelectedDevicesChange: DeviceSelectOnChange = (_, newValues) => {
        setSelectedDeviceIds(newValues);
    };

    const handleClear = () => {
        searchParams.set(GetTemperaturesSearchKeys.PAGE, DEFAULT_PAGE.toString());
        searchParams.delete(GetTemperaturesSearchKeys.MIN_TEMPERATURE);
        searchParams.delete(GetTemperaturesSearchKeys.MAX_TEMPERATURE);
        searchParams.delete(GetTemperaturesSearchKeys.DEVICE_ID);

        setTemperatureRangeValues(DEFAULT_TEMPERATURE_RANGE_VALUES);
        setSelectedDeviceIds([]);

        setSearchParams(searchParams, { replace: true });
    };

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        // Selected devices
        selectedDeviceIds.forEach((deviceId) => {
            searchParams.append(GetTemperaturesSearchKeys.DEVICE_ID, deviceId);
        });

        // Temperature range values
        const [minValue, maxValue] = temperatureRangeValues;
        searchParams.set(GetTemperaturesSearchKeys.PAGE, DEFAULT_PAGE.toString());
        searchParams.set(GetTemperaturesSearchKeys.MIN_TEMPERATURE, minValue.toString());
        searchParams.set(GetTemperaturesSearchKeys.MAX_TEMPERATURE, maxValue.toString());

        // Perform the search by replacing current search params with submitted ones
        setSearchParams(searchParams, { replace: true });
    };

    return (
        <form onSubmit={handleSubmit}>
            <Stack spacing="32px">
                <DeviceSelect
                    devices={devices}
                    selectedDeviceIds={selectedDeviceIds}
                    onChange={handleSelectedDevicesChange}
                />
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
                        onClick={handleClear}
                        sx={{ flex: 1 }}
                    >
                        Clear
                    </Button>
                    <Button
                        variant="solid"
                        color="primary"
                        startDecorator={<FilterAltIcon />}
                        type="submit"
                        sx={{ flex: 1 }}
                    >
                        Apply
                    </Button>
                </Box>
            </Stack>
        </form>
    );
}
