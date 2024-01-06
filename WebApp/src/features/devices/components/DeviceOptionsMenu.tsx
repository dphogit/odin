import DeleteForeverIcon from '@mui/icons-material/DeleteForever';
import MoreHorizIcon from '@mui/icons-material/MoreHoriz';
import WarningRoundedIcon from '@mui/icons-material/WarningRounded';
import {
    Button,
    DialogActions,
    DialogContent,
    DialogTitle,
    Divider,
    Dropdown,
    ListItemDecorator,
    Menu,
    MenuButton,
    MenuItem,
    Modal,
    ModalDialog,
} from '@mui/joy';
import { useState } from 'react';
import { Form } from 'react-router-dom';
import { PathNames } from 'routes/util';

export default function DeviceOptionsMenu() {
    const [isDeleteOpen, setIsDeleteOpen] = useState(false);

    const openDeleteModal = () => setIsDeleteOpen(true);
    const closeDeleteModal = () => setIsDeleteOpen(false);

    return (
        <>
            <Dropdown>
                <MenuButton startDecorator={<MoreHorizIcon />}>Options</MenuButton>
                <Menu placement="bottom-end">
                    <MenuItem onClick={openDeleteModal}>
                        <ListItemDecorator>
                            <DeleteForeverIcon />
                        </ListItemDecorator>
                        Delete Device
                    </MenuItem>
                </Menu>
            </Dropdown>
            <Modal open={isDeleteOpen} onClose={closeDeleteModal}>
                <ModalDialog variant="outlined" role="alertdialog" maxWidth="450px">
                    <DialogTitle>
                        <WarningRoundedIcon />
                        Confirm Deletion
                    </DialogTitle>
                    <Divider />
                    <DialogContent>
                        Are you sure you want to delete this device? All associated measurements
                        will also be removed. This action is irreversible.
                    </DialogContent>
                    <DialogActions>
                        <Form method="DELETE" action={PathNames.DEVICE_DELETE}>
                            <Button variant="solid" color="danger" type="submit">
                                Confirm Deletion
                            </Button>
                        </Form>
                        <Button
                            variant="plain"
                            color="neutral"
                            onClick={closeDeleteModal}
                            type="button"
                        >
                            Cancel
                        </Button>
                    </DialogActions>
                </ModalDialog>
            </Modal>
        </>
    );
}
