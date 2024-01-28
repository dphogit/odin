import { Select, Option, Box, Chip, FormControl, FormLabel } from '@mui/joy';
import { ApiDeviceDto } from 'features/devices';

export type DeviceSelectOnChange = (
    event:
        | React.MouseEvent<Element, MouseEvent>
        | React.KeyboardEvent<Element>
        | React.FocusEvent<Element, Element>
        | null,
    newSelectedDeviceIds: string[]
) => void;

interface DeviceSelectProps {
    devices: ApiDeviceDto[];
    selectedDeviceIds: string[];
    onChange?: DeviceSelectOnChange;
}

export default function DeviceSelect({ devices, selectedDeviceIds, onChange }: DeviceSelectProps) {
    return (
        <FormControl>
            <FormLabel>My Devices</FormLabel>
            <Select
                value={selectedDeviceIds}
                onChange={onChange}
                placeholder="Select Devices"
                multiple
                slotProps={{
                    listbox: { placement: 'bottom-start' },
                }}
                renderValue={(selected) => (
                    <Box sx={{ display: 'flex', gap: '0.25rem' }}>
                        {selected.map((selectedOption) => (
                            <Chip variant="soft" color="primary" key={selectedOption.id}>
                                {selectedOption.label}
                            </Chip>
                        ))}
                    </Box>
                )}
            >
                {devices.map((device) => (
                    <Option key={device.id.toString()} value={device.id.toString()}>
                        {device.name}
                    </Option>
                ))}
            </Select>
        </FormControl>
    );
}
