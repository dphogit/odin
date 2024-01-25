import { Option, Select } from '@mui/joy';
import { useSubmit } from 'react-router-dom';
import {
    DEFAULT_TIMERANGE_OPTION,
    TIMERANGE_SEARCHPARAMS_KEY,
    TimeRangeOptionType,
    TimeRangeOptions,
    isTimeRangeWithinDropdownOptions,
} from '../util';

interface TimeRangeDropdownProps {
    defaultValue?: TimeRangeOptionType;
}

export default function TimeRangeDropdown({
    defaultValue = DEFAULT_TIMERANGE_OPTION,
}: TimeRangeDropdownProps) {
    const submit = useSubmit();

    const handleChange = (_: React.SyntheticEvent | null, newValue: string | null) => {
        if (!newValue) return;
        const searchParams = new URLSearchParams();
        searchParams.set(TIMERANGE_SEARCHPARAMS_KEY, newValue);
        submit(searchParams, { replace: true });
    };

    if (!isTimeRangeWithinDropdownOptions(defaultValue)) {
        defaultValue = DEFAULT_TIMERANGE_OPTION;
    }

    return (
        <Select size="sm" defaultValue={defaultValue} name="timerange" onChange={handleChange}>
            <Option value={TimeRangeOptions.WEEK}>Last 7 Days</Option>
            <Option value={TimeRangeOptions.MONTH}>Last 30 Days</Option>
            <Option value={TimeRangeOptions.YEAR}>Last Year</Option>
        </Select>
    );
}
