import { Box, Button, Grid, Typography } from '@mui/joy';
import AddIcon from '@mui/icons-material/Add';
import DeviceCard from './DeviceCard';
import { useGetDevicesQuery } from '../api';

export default function DevicesPage() {
    const { data: devices } = useGetDevicesQuery();

    return (
        <Box maxWidth="1536px" mx="auto" px="24px">
            <Box display="flex" justifyContent="space-between" alignItems="stretch">
                <Typography level="h2" component="h1">
                    Your Devices
                </Typography>
                <Button startDecorator={<AddIcon />}>Add Device</Button>
            </Box>
            <Grid mt="24px" container spacing={1}>
                {devices.map((device) => (
                    <Grid key={device.id} xs={12} md={4}>
                        <DeviceCard device={device} />
                    </Grid>
                ))}
            </Grid>
        </Box>
    );
}
