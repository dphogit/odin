import { Box, Grid, Sheet, Typography } from '@mui/joy';
import { useGetDeviceDetailsQuery } from '../api';
import DeviceDisplayInfoCard from './DeviceDisplayInfoCard';

export default function DeviceDetailsPage() {
    const { data: device } = useGetDeviceDetailsQuery();

    return (
        <Box maxWidth="1536px" mx="auto" px="24px">
            <Typography level="h2" component="h1" mb="24px">
                Device Details
            </Typography>
            <Grid container spacing="24px" alignItems="stretch">
                <Grid xs={12} xl={4}>
                    <DeviceDisplayInfoCard device={device} />
                </Grid>
                <Grid xs={12} xl={8}>
                    <Sheet variant="outlined" sx={{ borderRadius: '8px' }}>
                        Temperatures Graph
                    </Sheet>
                </Grid>
            </Grid>
        </Box>
    );
}
