import { Box, Button, Grid, Typography } from '@mui/joy';
import FileDownloadIcon from '@mui/icons-material/FileDownload';
import { useGetTemperaturesWithDeviceQuery } from '../api';
import TemperaturesTable from './TemperaturesTable';
import TemperatureFilterOptions from './TemperatureFilterOptions';

export default function ManageTemperaturesPage() {
    const { data: response } = useGetTemperaturesWithDeviceQuery();

    return (
        <Box maxWidth="1920px" px="24px" mx="auto">
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
            <Grid container my="36px" columnSpacing="36px">
                <Grid xs={2}>
                    <TemperatureFilterOptions />
                </Grid>
                <Grid xs={10}>
                    <TemperaturesTable
                        temperatures={response.data}
                        totalRecords={response._meta.total}
                        page={response._meta.page}
                        rowsPerPage={response._meta.limit}
                    />
                </Grid>
            </Grid>
        </Box>
    );
}
