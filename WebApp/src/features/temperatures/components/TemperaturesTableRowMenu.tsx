import MoreHorizRoundedIcon from '@mui/icons-material/MoreHorizRounded';
import { Dropdown, IconButton, Menu, MenuButton, MenuItem } from '@mui/joy';
import { useSubmit } from 'react-router-dom';
import { PathNames } from 'routes/util';

interface TemperaturesTableRowMenuProps {
    temperatureId: string;
}

export default function TemperaturesTableRowMenu({ temperatureId }: TemperaturesTableRowMenuProps) {
    const deleteActionRoute = `${temperatureId}/${PathNames.TEMPERATURE_DELETE}`;

    const submit = useSubmit();

    const handleDelete = () => {
        submit(null, { method: 'DELETE', action: deleteActionRoute });
    };

    return (
        <Dropdown>
            <MenuButton
                slots={{ root: IconButton }}
                slotProps={{ root: { variant: 'plain', color: 'neutral', size: 'sm' } }}
            >
                <MoreHorizRoundedIcon />
            </MenuButton>
            <Menu size="sm" placement="bottom-end" sx={{ minWidth: 120 }}>
                <MenuItem color="danger" onClick={handleDelete}>
                    Delete
                </MenuItem>
            </Menu>
        </Dropdown>
    );
}
