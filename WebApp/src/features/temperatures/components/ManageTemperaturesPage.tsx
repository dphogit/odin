import { Box, Button, Typography } from '@mui/joy';
import FileDownloadIcon from '@mui/icons-material/FileDownload';
import TemperaturesTable from './TemperaturesTable';
import { useGetTemperaturesWithDeviceQuery } from '../api';

export default function ManageTemperaturesPage() {
    const { data: temperatures } = useGetTemperaturesWithDeviceQuery();

    return (
        <Box maxWidth="1536px" px="24px" mx="auto">
            <Box display="flex" justifyContent="space-between" alignItems="flex-start">
                <Typography level="h2" component="h1">
                    Manage Temperatures
                </Typography>
                <Button
                    variant="solid"
                    color="primary"
                    startDecorator={<FileDownloadIcon />}
                    disabled
                >
                    Download (Coming Soon)
                </Button>
            </Box>
            <Box my="24px">
                <TemperaturesTable
                    temperatures={temperatures.slice(0, 10)}
                    count={temperatures.length}
                />
            </Box>
        </Box>
    );
}
