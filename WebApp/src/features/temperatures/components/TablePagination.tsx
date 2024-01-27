import { useState } from 'react';
import { Box, IconButton, Option, Select, Typography, iconButtonClasses } from '@mui/joy';
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import FirstPageIcon from '@mui/icons-material/FirstPage';
import LastPageIcon from '@mui/icons-material/LastPage';
import { DEFAULT_PAGE_LIMIT, DEFAULT_ROWS_PER_PAGE_OPTIONS } from '../util';

export interface PageButtonOptions {
    onClick?: (e: React.MouseEvent<HTMLAnchorElement>) => void;
}

export interface SkipPageButtonOptions extends PageButtonOptions {
    show?: boolean;
}

interface TablePaginationProps {
    totalRecords: number;
    page: number;
    firstPageButtonOptions?: SkipPageButtonOptions;
    prevPageButtonOptions?: PageButtonOptions;
    nextPageButtonOptions?: PageButtonOptions;
    lastPageButtonOptions?: SkipPageButtonOptions;
    onRowsPerPageChange?: (e: React.SyntheticEvent | null, newValue: string) => void;
}

export default function TablePagination({
    totalRecords,
    page,
    firstPageButtonOptions,
    prevPageButtonOptions,
    nextPageButtonOptions,
    lastPageButtonOptions,
    onRowsPerPageChange,
}: TablePaginationProps) {
    const [rowsPerPage, setRowsPerPage] = useState(DEFAULT_PAGE_LIMIT);

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

    const start = totalRecords === 0 ? 0 : (page - 1) * rowsPerPage + 1;
    const end = Math.min(page * rowsPerPage, totalRecords);
    const isFirstPage = start === 1;
    const isLastPage = end === totalRecords;
    const currentRange = `${start}-${end} of ${totalRecords}`;

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
                    defaultValue={DEFAULT_PAGE_LIMIT.toString()}
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
                {firstPageButtonOptions?.show && (
                    <IconButton
                        aria-label="First Page"
                        disabled={isFirstPage}
                        onClick={firstPageButtonOptions?.onClick}
                    >
                        <FirstPageIcon />
                    </IconButton>
                )}
                <IconButton
                    aria-label="Previous Page"
                    disabled={isFirstPage}
                    onClick={prevPageButtonOptions?.onClick}
                >
                    <ChevronLeftIcon />
                </IconButton>
                <IconButton
                    aria-label="Next Page"
                    disabled={isLastPage}
                    onClick={nextPageButtonOptions?.onClick}
                >
                    <ChevronRightIcon />
                </IconButton>
                {lastPageButtonOptions?.show && (
                    <IconButton
                        aria-label="Last Page"
                        disabled={isLastPage}
                        onClick={lastPageButtonOptions?.onClick}
                    >
                        <LastPageIcon />
                    </IconButton>
                )}
            </Box>
        </Box>
    );
}
