import { Box, Typography } from '@mui/joy';
import { useGetDeviceDetailsQuery } from '../api';

export default function DeviceDetailsPage() {
    const { data: device } = useGetDeviceDetailsQuery();

    return (
        <Box maxWidth="1280px" mx="auto">
            <Typography level="h2" component="h1">
                Device Id: {device.id}
            </Typography>
            <ul>
                <li>Name: {device.name}</li>
                <li>Location: {device.location}</li>
                <li>Description: {device.description}</li>
                <li>Created At: {device.createdAt}</li>
                <li>Updated At: {device.updatedAt}</li>
            </ul>
        </Box>
    );
}
