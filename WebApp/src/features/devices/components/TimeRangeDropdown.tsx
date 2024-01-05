import { Option, Select } from '@mui/joy';
import { useSubmit } from 'react-router-dom';
import { DAYS_SEARCH_PARAMS_KEY, TimeRangeOptions, isDaysWithinDropdownOptions } from '../util';

interface TimeRangeDropdownProps {
    defaultValue?: number;
}

export default function TimeRangeDropdown({
    defaultValue = TimeRangeOptions.LAST_30_DAYS,
}: TimeRangeDropdownProps) {
    const submit = useSubmit();

    const handleChange = (_: React.SyntheticEvent | null, newValue: string | null) => {
        if (!newValue) return;
        const searchParams = new URLSearchParams();
        searchParams.set(DAYS_SEARCH_PARAMS_KEY, newValue);
        submit(searchParams);
    };

    if (!isDaysWithinDropdownOptions(defaultValue)) {
        defaultValue = TimeRangeOptions.LAST_30_DAYS;
    }

    return (
        <Select
            size="sm"
            defaultValue={defaultValue.toString()}
            name="days"
            onChange={handleChange}
        >
            <Option value={TimeRangeOptions.LAST_30_DAYS.toString()}>Last 30 Days</Option>
            <Option value={TimeRangeOptions.LAST_7_DAYS.toString()}>Last 7 Days</Option>
        </Select>
    );
}
