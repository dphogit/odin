import { Box, Grid, Typography } from '@mui/joy';
import { AddDevice } from '.';
import { useGetDevicesQuery } from '../api';
import DeviceCard from './DeviceCard';

export default function DevicesPage() {
    const { data: devices } = useGetDevicesQuery();

    return (
        <Box maxWidth="1536px" mx="auto" px="24px">
            <Box display="flex" justifyContent="space-between" alignItems="stretch">
                <Typography level="h2" component="h1">
                    Your Devices
                </Typography>
                <AddDevice />
            </Box>
            <Grid mt="24px" container spacing={1} alignItems="stretch" gap="24px">
                {devices.map((device) => (
                    <Grid key={device.id} xs={12} md={4}>
                        <DeviceCard device={device} />
                    </Grid>
                ))}
            </Grid>
        </Box>
    );
}
