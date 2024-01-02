import { Box, List, ListItem, ListItemButton, ListItemContent, Sheet, Typography } from '@mui/joy';
import DevicesIcon from '@mui/icons-material/Devices';
import StraightenIcon from '@mui/icons-material/Straighten';
import { Link, useMatch, useResolvedPath } from 'react-router-dom';

interface NavItemProps {
    to: string;
    icon: React.ReactNode;
    label: string;
}

function NavItem({ to, icon, label }: NavItemProps) {
    const resolved = useResolvedPath(to);
    const match = useMatch({ path: resolved.pathname, end: true });

    return (
        <ListItem>
            <ListItemButton
                selected={Boolean(match)}
                sx={{ gap: '16px', textDecoration: 'none' }}
                component={Link}
                to={to}
            >
                {icon}
                <ListItemContent>
                    <Typography level="title-sm" fontWeight={match ? 'bold' : 'normal'}>
                        {label}
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
            <NavItem to="/devices" icon={<DevicesIcon />} label="Devices" />
            <NavItem to="/units" icon={<StraightenIcon />} label="Units" />
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
            <Typography level="h3" component="h2">
                📱 Odin
            </Typography>
            <Box pt="24px">
                <NavItems />
            </Box>
        </Sheet>
    );
}

export default Sidebar;
