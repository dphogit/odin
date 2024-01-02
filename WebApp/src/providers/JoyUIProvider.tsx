import { CssVarsProvider, CssBaseline, GlobalStyles } from '@mui/joy';

export interface JoyUIProviderProps {
    children: React.ReactNode;
}

export default function JoyUIProvider({ children }: JoyUIProviderProps) {
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
            {children}
        </CssVarsProvider>
    );
}
