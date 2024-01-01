import { Box, Typography } from '@mui/joy';
import { isRouteErrorResponse, useRouteError } from 'react-router-dom';

function ErrorPage() {
    const error = useRouteError();
    console.error(error);

    let errorMessage: string;
    if (isRouteErrorResponse(error)) {
        errorMessage = error.data?.message || error.statusText;
    } else if (error instanceof Error) {
        errorMessage = error.message;
    } else if (typeof error === 'string') {
        errorMessage = error;
    } else {
        errorMessage = 'Unknown error';
    }

    return (
        <Box
            id="error-page"
            display="flex"
            height="100vh"
            flexDirection="column"
            justifyContent="center"
            alignItems="center"
            gap="24px"
        >
            <Typography level="h1">Oops!</Typography>
            <Typography>Sorry, an unexpected error has occured.</Typography>
            <Typography fontStyle="italic">{errorMessage}</Typography>
        </Box>
    );
}

export default ErrorPage;
