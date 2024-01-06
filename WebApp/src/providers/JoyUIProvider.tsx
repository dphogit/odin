import { CssVarsProvider, CssBaseline, GlobalStyles, extendTheme } from '@mui/joy';

const theme = extendTheme({
    components: {
        JoyFormLabel: {
            styleOverrides: {
                root: {
                    fontWeight: 700,
                },
            },
        },
    },
});

export interface JoyUIProviderProps {
    children: React.ReactNode;
}

export default function JoyUIProvider({ children }: JoyUIProviderProps) {
    return (
        <CssVarsProvider theme={theme}>
            <CssBaseline />
            <GlobalStyles
                styles={{
                    ':root': {
                        '--Sidebar-width': '260px',
                        '--Page-padding-top': '48px',
                    },
                }}
            />
            {children}
        </CssVarsProvider>
    );
}
