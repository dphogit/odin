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
    totalRecords: number;
    page: number;
    rowsPerPage: number;
}

export default function TemperaturesTable({
    temperatures,
    totalRecords,
    page,
    rowsPerPage,
}: TemperaturesTableProps) {
    const [, setSearchParams] = useSearchParams();

    const lastPage = Math.ceil(totalRecords / temperatures.length);

    const handleRowsPerPageChange = (_: React.SyntheticEvent | null, newValue: string) => {
        setSearchParams({ page: DEFAULT_PAGE.toString(), limit: newValue }, { replace: true });
    };

    const changePage = (newPage: number) => {
        setSearchParams(
            { page: newPage.toString(), limit: rowsPerPage.toString() },
            { replace: true }
        );
    };

    const handleFirstPageClick = () => {
        changePage(1);
    };

    const handlePrevPageClick = () => {
        if (page <= 1) return;
        changePage(page - 1);
    };

    const handleNextPageClick = () => {
        if (page >= lastPage) return;
        changePage(page + 1);
    };

    const handleLastPageClick = () => {
        changePage(lastPage);
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
                totalRecords={totalRecords}
                page={page}
                firstPageButtonOptions={{ onClick: handleFirstPageClick, show: true }}
                prevPageButtonOptions={{ onClick: handlePrevPageClick }}
                nextPageButtonOptions={{ onClick: handleNextPageClick }}
                lastPageButtonOptions={{ onClick: handleLastPageClick, show: true }}
                onRowsPerPageChange={handleRowsPerPageChange}
            />
        </Sheet>
    );
}
