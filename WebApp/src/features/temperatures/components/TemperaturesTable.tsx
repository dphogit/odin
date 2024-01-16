import { Box, Divider, Sheet, Table } from '@mui/joy';
import { ApiTemperatureWithDeviceDto } from '../api';
import dayjs from 'dayjs';
import DataCell from './DataCell';
import TemperaturesTableRowMenu from './TemperaturesTableRowMenu';
import TablePagination from './TablePagination';
import { useSearchParams } from 'react-router-dom';
import { DEFAULT_PAGE } from '..';

function formatTimestamp(timestamp: string) {
    return dayjs(timestamp).format('MMM DD, YYYY hh:mm:ss a');
}

interface TemperaturesTableProps {
    temperatures: ApiTemperatureWithDeviceDto[];
    total: number;
    page: number;
}

export default function TemperaturesTable({ temperatures, total, page }: TemperaturesTableProps) {
    const [, setSearchParams] = useSearchParams();

    const handleRowsPerPageChange = (_: React.SyntheticEvent | null, newValue: string) => {
        setSearchParams({ page: DEFAULT_PAGE.toString(), limit: newValue }, { replace: true });
    };

    return (
        <Sheet variant="outlined" sx={{ borderRadius: 'sm' }}>
            <Table aria-label="Temperatures Table" noWrap>
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
            <Divider />
            <TablePagination
                total={total}
                page={page}
                onRowsPerPageChange={handleRowsPerPageChange}
            />
        </Sheet>
    );
}
