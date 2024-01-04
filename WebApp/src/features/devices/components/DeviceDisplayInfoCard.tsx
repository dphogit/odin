import {
    Box,
    Button,
    Card,
    CardActions,
    CardOverflow,
    Divider,
    FormControl,
    FormLabel,
    Input,
    Stack,
    Textarea,
    Typography,
} from '@mui/joy';
import { ApiDeviceDto } from '../types';

interface DeviceInfoCardProps {
    device: ApiDeviceDto;
}

export default function DeviceDisplayInfoCard({ device }: DeviceInfoCardProps) {
    return (
        <Card variant="outlined">
            <Box>
                <Typography level="title-lg" mb="4px">
                    Display Info
                </Typography>
                <Typography level="body-sm">
                    Customise your device's display information here.
                </Typography>
            </Box>
            <Divider />
            <Stack spacing="24px" mt="8px" mb="20px">
                <FormControl>
                    <FormLabel>Name</FormLabel>
                    <Input required value={device.name} placeholder="Enter device name" />
                </FormControl>
                <FormControl>
                    <FormLabel>Location</FormLabel>
                    <Input value={device.location} placeholder="Enter device location" />
                </FormControl>
                <FormControl>
                    <FormLabel>Description</FormLabel>
                    <Textarea
                        value={device.description}
                        placeholder="Enter device description"
                        minRows={4}
                        maxRows={4}
                    />
                </FormControl>
            </Stack>
            <CardOverflow sx={{ borderTop: '1px solid', borderColor: 'divider' }}>
                <CardActions sx={{ alignSelf: 'flex-end' }}>
                    <Button variant="outlined" color="neutral">
                        Reset
                    </Button>
                    <Button variant="solid">Save</Button>
                </CardActions>
            </CardOverflow>
        </Card>
    );
}
