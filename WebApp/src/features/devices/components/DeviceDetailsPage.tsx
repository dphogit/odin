import { Box, Grid, Sheet, Typography } from '@mui/joy';
import { useGetDeviceDetailsQuery } from '../api';
import DeviceDisplayInfoCard from './DeviceDisplayInfoCard';
import DeviceTemperatureGraph, { DeviceTemperatureGraphDataPoint } from './DeviceTemperatureGraph';
import { ApiTemperatureDto } from '../types';

function transformToTemperatureGraphData(
    data: ApiTemperatureDto[]
): DeviceTemperatureGraphDataPoint[] {
    return data.map((item) => ({
        timestamp: new Date(item.timestamp),
        degreesCelsius: item.degreesCelsius,
    }));
}

export default function DeviceDetailsPage() {
    const { data: device } = useGetDeviceDetailsQuery();

    const deviceTemperatureGraphData = transformToTemperatureGraphData(device.temperatures ?? []);

    return (
        <Box px="24px">
            <Typography level="h2" component="h1" mb="24px">
                Device Details
            </Typography>
            <Grid container spacing="24px" alignItems="stretch">
                <Grid xs={12} xl={3}>
                    <DeviceDisplayInfoCard device={device} />
                </Grid>
                <Grid xs={12} xl={9}>
                    <Sheet variant="outlined" sx={{ borderRadius: '8px', p: '16px' }}>
                        <Typography level="title-lg" mb="16px" textAlign="center">
                            Temperatures Recorded
                        </Typography>
                        <DeviceTemperatureGraph
                            data={deviceTemperatureGraphData}
                            containerHeight={600}
                        />
                    </Sheet>
                </Grid>
            </Grid>
        </Box>
    );
}
