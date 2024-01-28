import { Box, Divider, Link, Sheet, Table } from '@mui/joy';
import ArrowDropDownIcon from '@mui/icons-material/ArrowDropDown';
import ArrowDropUpIcon from '@mui/icons-material/ArrowDropUp';
import { ApiTemperatureWithDeviceDto } from '../api';
import dayjs from 'dayjs';
import DataCell from './DataCell';
import TemperaturesTableRowMenu from './TemperaturesTableRowMenu';
import TablePagination from './TablePagination';
import { useSearchParams } from 'react-router-dom';
import { DEFAULT_PAGE, GetTemperaturesSearchKeys } from '../util';

function formatTimestamp(timestamp: string) {
    return dayjs(timestamp).format('MMM DD, YYYY hh:mm:ss a');
}

interface TimestampHeaderCellProps {
    onClick?: (e: React.MouseEvent) => void;

    /**
     * Direction for the arrow icon to indicate the order of the results,
     * intention is up is ascending, down is descending.
     */
    arrowIconDirection?: 'up' | 'down';
}

function TimestampHeaderCell({ onClick, arrowIconDirection }: TimestampHeaderCellProps) {
    return (
        <th style={{ width: '300px' }}>
            <Link
                underline="none"
                color="primary"
                component="button"
                onClick={onClick}
                fontWeight="lg"
                endDecorator={
                    arrowIconDirection === 'up' ? <ArrowDropUpIcon /> : <ArrowDropDownIcon />
                }
            >
                Timestamp
            </Link>
        </th>
    );
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
    const [searchParams, setSearchParams] = useSearchParams();

    const timestampSortOrder = searchParams.get('sort')?.toLowerCase() === 'asc' ? 'asc' : 'desc';

    const lastPage = Math.ceil(totalRecords / temperatures.length);

    const handleTimestampHeaderClick = () => {
        const newSortOrder = timestampSortOrder === 'asc' ? 'desc' : 'asc';
        searchParams.set(GetTemperaturesSearchKeys.TIMESTAMP_SORT, newSortOrder);
        searchParams.set(GetTemperaturesSearchKeys.PAGE, DEFAULT_PAGE.toString());
        setSearchParams(searchParams, { replace: true });
    };

    const handleRowsPerPageChange = (_: React.SyntheticEvent | null, newValue: string) => {
        searchParams.set(GetTemperaturesSearchKeys.PAGE, DEFAULT_PAGE.toString());
        searchParams.set(GetTemperaturesSearchKeys.LIMIT, newValue);
        setSearchParams(searchParams, { replace: true });
    };

    const changePage = (newPage: number) => {
        searchParams.set(GetTemperaturesSearchKeys.PAGE, newPage.toString());
        searchParams.set(GetTemperaturesSearchKeys.LIMIT, rowsPerPage.toString());
        setSearchParams(searchParams, { replace: true });
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

    const timestampHeaderCellArrowDirection = timestampSortOrder === 'asc' ? 'up' : 'down';

    return (
        <Sheet variant="outlined" sx={{ borderRadius: 'sm' }}>
            <Table aria-label="Temperatures Table" noWrap stickyHeader>
                <thead>
                    <tr>
                        <TimestampHeaderCell
                            onClick={handleTimestampHeaderClick}
                            arrowIconDirection={timestampHeaderCellArrowDirection}
                        />
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
