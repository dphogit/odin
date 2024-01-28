import { FormControl, FormHelperText, FormLabel, Slider } from '@mui/joy';
import { TemperatureRangeValues } from '../util';

interface TemperatureRangeFilterProps {
    values: TemperatureRangeValues;
    maxValue: number;
    step?: number;
    onChange?: (event: Event, newValues: TemperatureRangeValues, activeThumb: number) => void;
}

export default function TemperatureRangeFilter({
    values,
    onChange,
    maxValue,
    step,
}: TemperatureRangeFilterProps) {
    const formHelperText = `${values[0]} °C - ${values[1]} °C`;

    return (
        <FormControl sx={{ alignItems: 'center' }}>
            <FormLabel>Temperature Range</FormLabel>
            <Slider
                aria-label="Temperature Range"
                sx={{ pt: '12px', pb: '4px' }}
                value={values}
                max={maxValue}
                step={step}
                size="sm"
                onChange={(e, newValues, activeThumb) =>
                    onChange?.(e, newValues as TemperatureRangeValues, activeThumb)
                }
            />
            <FormHelperText>{formHelperText}</FormHelperText>
        </FormControl>
    );
}
