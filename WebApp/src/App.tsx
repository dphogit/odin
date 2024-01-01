import '@fontsource/inter';
import { CssBaseline, CssVarsProvider } from '@mui/joy';
import { Sidebar } from './components';

function App() {
    return (
        <CssVarsProvider>
            <CssBaseline />
            <Sidebar />
        </CssVarsProvider>
    );
}

export default App;
