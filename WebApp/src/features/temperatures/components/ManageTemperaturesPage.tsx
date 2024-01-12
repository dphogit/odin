import { Box, Button, Typography } from '@mui/joy';
import FileDownloadIcon from '@mui/icons-material/FileDownload';

export default function ManageTemperaturesPage() {
    return (
        <Box maxWidth="1536px" px="24px" mx="auto">
            <Box display="flex" justifyContent="space-between" alignItems="flex-start">
                <Typography level="h2" component="h1">
                    Manage Temperatures
                </Typography>
                <Button
                    variant="solid"
                    color="primary"
                    startDecorator={<FileDownloadIcon />}
                    disabled
                >
                    Download (Coming Soon)
                </Button>
            </Box>
        </Box>
    );
}
