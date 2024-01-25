import { Typography } from '@mui/joy';

interface DataCellProps {
    children: React.ReactNode;
    numericFormatting?: boolean;
}

export default function DataCell({ children, numericFormatting }: DataCellProps) {
    return (
        <td>
            <Typography
                level="body-xs"
                sx={{
                    fontVariantNumeric: numericFormatting ? 'tabular-nums' : undefined,
                }}
            >
                {children}
            </Typography>
        </td>
    );
}
