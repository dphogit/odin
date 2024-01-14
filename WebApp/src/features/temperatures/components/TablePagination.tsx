import { useState } from 'react';
import { Box, IconButton, Option, Select, Typography, iconButtonClasses } from '@mui/joy';
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import FirstPageIcon from '@mui/icons-material/FirstPage';
import LastPageIcon from '@mui/icons-material/LastPage';

const DEFAULT_ROWS_PER_PAGE_OPTIONS = [10, 25, 50, 100];

interface TablePaginationProps {
    count: number;
    page: number;
    onRowsPerPageChange?: (e: React.SyntheticEvent | null, newValue: string | null) => void;
}

export default function TablePagination({
    count,
    page,
    onRowsPerPageChange,
}: TablePaginationProps) {
    const [rowsPerPage, setRowsPerPage] = useState(DEFAULT_ROWS_PER_PAGE_OPTIONS[0]);

    const handleRowsPerPageChange = (e: React.SyntheticEvent | null, newValue: string | null) => {
        if (!newValue) return;

        const newRowsPerPage = parseInt(newValue);

        if (Number.isNaN(newRowsPerPage) || newRowsPerPage == rowsPerPage) {
            console.warn(`Invalid "rowsPerPage": ${newValue}, ignored and no update will occur.`);
            return;
        }

        setRowsPerPage(newRowsPerPage);
        onRowsPerPageChange?.(e, newValue);
    };

    const start = (page - 1) * rowsPerPage + 1;
    const end = Math.min(page * rowsPerPage, count);
    const isFirstPage = start === 1;
    const isLastPage = end === count;
    const currentRange = `${start}-${end} of ${count}`;

    return (
        <Box
            display="flex"
            justifyContent="flex-end"
            alignItems="center"
            px="12px"
            py="8px"
            gap="24px"
        >
            <Box display="flex" alignItems="center" gap="12px" mr="12px">
                <Typography level="body-sm">Rows per page:</Typography>

                <Select
                    defaultValue={DEFAULT_ROWS_PER_PAGE_OPTIONS[0].toString()}
                    size="sm"
                    variant="outlined"
                    onChange={handleRowsPerPageChange}
                >
                    {DEFAULT_ROWS_PER_PAGE_OPTIONS.map((option) => (
                        <Option key={option} value={option.toString()}>
                            {option}
                        </Option>
                    ))}
                </Select>
            </Box>
            <Typography level="body-sm">{currentRange}</Typography>
            <Box display="flex" sx={{ [`& .${iconButtonClasses.root}`]: { borderRadius: '50%' } }}>
                <IconButton aria-label="First Page" disabled={isFirstPage}>
                    <FirstPageIcon />
                </IconButton>
                <IconButton aria-label="Previous Page" disabled={isFirstPage}>
                    <ChevronLeftIcon />
                </IconButton>
                <IconButton aria-label="Next Page" disabled={isLastPage}>
                    <ChevronRightIcon />
                </IconButton>
                <IconButton aria-label="Last Page" disabled={isLastPage}>
                    <LastPageIcon />
                </IconButton>
            </Box>
        </Box>
    );
}
