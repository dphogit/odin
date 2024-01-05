import { Box, Grid, Sheet, Typography } from '@mui/joy';
import { useGetDeviceDetailsQuery } from '../api';
import { ApiTemperatureDto } from '../types';
import DeviceDisplayInfoCard from './DeviceDisplayInfoCard';
import DeviceTemperatureGraph, { DeviceTemperatureGraphDataPoint } from './DeviceTemperatureGraph';
import TimeRangeDropdown from './TimeRangeDropdown';

function transformToTemperatureGraphData(
    data: ApiTemperatureDto[]
): DeviceTemperatureGraphDataPoint[] {
    return data.map((item) => ({
        timestamp: new Date(item.timestamp),
        degreesCelsius: item.degreesCelsius,
    }));
}

export default function DeviceDetailsPage() {
    const { data: response } = useGetDeviceDetailsQuery();

    const deviceTemperatureGraphData = transformToTemperatureGraphData(
        response.device.temperatures ?? []
    );

    return (
        <Box px="24px">
            <Typography level="h2" component="h1" mb="24px">
                Device Details
            </Typography>
            <Grid container spacing="24px" alignItems="stretch">
                <Grid xs={12} xl={3}>
                    <DeviceDisplayInfoCard device={response.device} />
                </Grid>
                <Grid xs={12} xl={9}>
                    <Sheet variant="outlined" sx={{ borderRadius: '8px', p: '16px' }}>
                        <Box
                            mb="24px"
                            display="flex"
                            alignItems="center"
                            justifyContent="space-between"
                        >
                            <Typography level="title-lg">
                                Temperatures Recorded (Daily Average)
                            </Typography>
                            <TimeRangeDropdown defaultValue={response.days} />
                        </Box>
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
