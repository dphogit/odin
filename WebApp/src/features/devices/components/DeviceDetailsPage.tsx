import { Box, Grid, Sheet, Typography } from '@mui/joy';
import { useGetDeviceDetailsQuery } from '../api';
import { ApiTemperatureDto } from '../api/types';
import DeviceEditableInfoCard from './DeviceEditableInfoCard';
import DeviceTemperatureGraph, { DeviceTemperatureGraphDataPoint } from './DeviceTemperatureGraph';
import TimeRangeDropdown from './TimeRangeDropdown';
import { useNavigation } from 'react-router-dom';
import { DAYS_SEARCH_PARAMS_KEY } from '../util';

function transformToTemperatureGraphData(
    data: ApiTemperatureDto[]
): DeviceTemperatureGraphDataPoint[] {
    return data.map((item) => ({
        timestamp: new Date(item.timestamp),
        degreesCelsius: item.degreesCelsius,
    }));
}

/**
 * Determines if we are fetching temperatures for a new selected time range (not in cache)
 * from the dropdown. Can be used to show a loading indicator.
 */
function useIsFetchingTemperatures() {
    const navigation = useNavigation();
    return new URLSearchParams(navigation.location?.search).has(DAYS_SEARCH_PARAMS_KEY);
}

export default function DeviceDetailsPage() {
    const { data: response } = useGetDeviceDetailsQuery();

    const deviceTemperatureGraphData = transformToTemperatureGraphData(
        response.device.temperatures ?? []
    );

    const isFetchingTemperatures = useIsFetchingTemperatures();

    return (
        <Box px="24px">
            <Typography level="h2" component="h1" mb="24px">
                Device Details
            </Typography>
            <Grid container spacing="24px" alignItems="stretch">
                <Grid xs={12} xl={3}>
                    <DeviceEditableInfoCard device={response.device} />
                </Grid>
                <Grid xs={12} xl={9}>
                    <Sheet variant="outlined" sx={{ borderRadius: '8px', p: '16px' }}>
                        <Box
                            mb="32px"
                            display="flex"
                            alignItems="flex-start"
                            justifyContent="space-between"
                        >
                            <Box>
                                <Typography level="title-lg" mb="4px">
                                    Recorded Temperatures
                                </Typography>
                                <Typography level="body-sm">
                                    Daily average temperatures recorded within the selected time
                                    range.
                                </Typography>
                            </Box>
                            <TimeRangeDropdown defaultValue={response.days} />
                        </Box>
                        <DeviceTemperatureGraph
                            data={deviceTemperatureGraphData}
                            containerHeight={600}
                            isLoading={isFetchingTemperatures}
                            days={response.days}
                        />
                    </Sheet>
                </Grid>
            </Grid>
        </Box>
    );
}
