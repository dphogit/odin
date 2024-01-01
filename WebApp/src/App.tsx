import '@fontsource/inter';
import { Box, CssBaseline, CssVarsProvider, GlobalStyles, Stack } from '@mui/joy';
import { Outlet } from 'react-router-dom';
import { Sidebar } from 'components';

export default function App() {
    return (
        <CssVarsProvider>
            <CssBaseline />
            <GlobalStyles
                styles={{
                    ':root': {
                        '--Sidebar-width': '260px',
                        '--Page-padding-top': '36px',
                    },
                }}
            />
            <Stack spacing={0} direction="row" alignItems="flex-start">
                <Sidebar />
                <Box component="main" flex={1} px="48px" pt="var(--Page-padding-top)">
                    <Outlet />
                </Box>
            </Stack>
        </CssVarsProvider>
    );
}
