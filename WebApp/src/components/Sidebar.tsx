import DevicesIcon from '@mui/icons-material/Devices';
import { Box, List, ListItem, ListItemButton, ListItemContent, Sheet, Typography } from '@mui/joy';

// TODO Add navigational features when required

function NavItem({ isSelected }: { isSelected?: boolean }) {
    return (
        <ListItem>
            <ListItemButton selected={isSelected} sx={{ gap: '16px' }}>
                <DevicesIcon />
                <ListItemContent>
                    <Typography level="title-sm" fontWeight={isSelected ? 'bold' : 'normal'}>
                        Devices
                    </Typography>
                </ListItemContent>
            </ListItemButton>
        </ListItem>
    );
}

function NavItems() {
    return (
        <List
            sx={{
                '--ListItem-radius': (theme) => theme.vars.radius.sm,
                gap: 1,
            }}
        >
            <NavItem isSelected />
        </List>
    );
}

function Sidebar() {
    return (
        <Sheet
            sx={{
                position: 'sticky',
                width: 'var(--Sidebar-width)',
                height: '100vh',
                top: 0,
                pt: 'var(--Page-padding-top)',
                px: '24px',
                borderRight: '1px solid',
                borderColor: 'divider',
            }}
        >
            <Typography level="h3" component="h1">
                📱 Odin
            </Typography>
            <Box pt="24px">
                <NavItems />
            </Box>
        </Sheet>
    );
}

export default Sidebar;
