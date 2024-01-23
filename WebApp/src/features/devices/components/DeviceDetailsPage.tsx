import { Box, Grid, Sheet, Typography } from '@mui/joy';
import { useNavigation } from 'react-router-dom';
import { useGetDeviceDetailsQuery } from '../api';
import { TIMERANGE_SEARCHPARAMS_KEY } from '../util';
import DeviceEditableInfoCard from './DeviceEditableInfoCard';
import DeviceOptionsMenu from './DeviceOptionsMenu';
import DeviceTemperatureGraph from './DeviceTemperatureGraph';
import TimeRangeDropdown from './TimeRangeDropdown';

/**
 * Determines if we are fetching temperatures for a new selected time range (not in cache)
 * from the dropdown. Can be used to show a loading indicator.
 */
function useIsFetchingTemperatures() {
    const navigation = useNavigation();
    return new URLSearchParams(navigation.location?.search).has(TIMERANGE_SEARCHPARAMS_KEY);
}

export default function DeviceDetailsPage() {
    const { data: response } = useGetDeviceDetailsQuery();

    // Show loading when changing time range options from dropdown.
    const isFetchingTemperatures = useIsFetchingTemperatures();

    return (
        <Box px="24px">
            <Box display="flex" justifyContent="space-between" alignItems="flex-start" mb="24px">
                <Typography level="h2" component="h1">
                    Device Details
                </Typography>
                <DeviceOptionsMenu />
            </Box>
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
                            <TimeRangeDropdown defaultValue={response.timeRange} />
                        </Box>
                        <DeviceTemperatureGraph
                            data={response.device.temperatures}
                            containerHeight={600}
                            isLoading={isFetchingTemperatures}
                            timeRange={response.timeRange}
                        />
                    </Sheet>
                </Grid>
            </Grid>
        </Box>
    );
}
