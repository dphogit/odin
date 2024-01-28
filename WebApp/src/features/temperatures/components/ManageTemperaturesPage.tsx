import { Box, Button, Grid, Typography } from '@mui/joy';
import FileDownloadIcon from '@mui/icons-material/FileDownload';
import { useGetManageTemperaturesPageDataQuery } from '../api';
import TemperaturesTable from './TemperaturesTable';
import TemperatureFilterOptions from './TemperatureFilterOptions';

export default function ManageTemperaturesPage() {
    const response = useGetManageTemperaturesPageDataQuery();

    const { devices, temperatures } = response.data;

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
                    <TemperatureFilterOptions devices={devices} />
                </Grid>
                <Grid xs={10}>
                    <TemperaturesTable
                        temperatures={temperatures.data}
                        totalRecords={temperatures._meta.total}
                        page={temperatures._meta.page}
                        rowsPerPage={temperatures._meta.limit}
                    />
                </Grid>
            </Grid>
        </Box>
    );
}
