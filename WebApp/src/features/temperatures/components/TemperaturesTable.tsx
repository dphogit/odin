import { Box, Sheet, Table, Typography } from '@mui/joy';
import { ApiTemperatureWithDeviceDto } from '../api';
import dayjs from 'dayjs';
import TemperaturesTableRowMenu from './TemperaturesTableRowMenu';

function formatTimestamp(timestamp: string) {
    return dayjs(timestamp).format('MMM DD, YYYY hh:mm:ss a');
}

interface DataCellProps {
    children: React.ReactNode;
    numericFormatting?: boolean;
}

function DataCell({ children, numericFormatting }: DataCellProps) {
    return (
        <td>
            <Typography
                level="body-sm"
                sx={{
                    fontVariantNumeric: numericFormatting ? 'tabular-nums' : undefined,
                }}
            >
                {children}
            </Typography>
        </td>
    );
}

interface TemperaturesTableProps {
    temperatures: ApiTemperatureWithDeviceDto[];
}

export default function TemperaturesTable({ temperatures }: TemperaturesTableProps) {
    return (
        <Sheet variant="outlined" sx={{ borderRadius: 'sm' }}>
            <Table aria-label="Temperatures Table" noWrap size="lg">
                <thead>
                    <tr>
                        <th style={{ width: '300px' }}>Timestamp</th>
                        <th style={{ width: '180px' }}>Degrees&nbsp;(°C)</th>
                        <th>Device</th>
                        <th>Location</th>
                        <th style={{ width: '140px' }}></th>
                    </tr>
                </thead>
                <tbody>
                    {temperatures.map((t) => (
                        <tr key={t.id}>
                            <DataCell numericFormatting>{formatTimestamp(t.timestamp)}</DataCell>
                            <DataCell numericFormatting>{t.degreesCelsius.toFixed(2)}</DataCell>
                            <DataCell>{t.device.name}</DataCell>
                            <DataCell>{t.device.location}</DataCell>
                            <td>
                                <Box display="flex" justifyContent="flex-end" alignItems="center">
                                    <TemperaturesTableRowMenu temperatureId={t.id.toString()} />
                                </Box>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </Table>
        </Sheet>
    );
}
