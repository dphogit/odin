import '@fontsource/inter';
import { Box, Stack } from '@mui/joy';
import { Sidebar } from 'components';
import JoyUIProvider from 'providers/JoyUIProvider';
import ReactQueryClientProvider from 'providers/ReactQueryClientProvider';
import { Outlet } from 'react-router-dom';

function AppLayout() {
    return (
        <Stack spacing={0} direction="row" alignItems="flex-start">
            <Sidebar />
            <Box component="main" flex={1} pt="var(--Page-padding-top)">
                <Outlet />
            </Box>
        </Stack>
    );
}

export default function App() {
    return (
        <JoyUIProvider>
            <ReactQueryClientProvider>
                <AppLayout />
            </ReactQueryClientProvider>
        </JoyUIProvider>
    );
}
