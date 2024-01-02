import { Box, Typography } from '@mui/joy';
import { useLoaderData } from 'react-router-dom';
import { IDevice } from '../types';
import { DevicesTable } from '../components';

export default function DevicePage() {
    const devices = useLoaderData() as IDevice[];

    return (
        <div>
            <Typography level="h1">Device Page</Typography>
            <Box mt="24px">
                <DevicesTable devices={devices} />
            </Box>
        </div>
    );
}
