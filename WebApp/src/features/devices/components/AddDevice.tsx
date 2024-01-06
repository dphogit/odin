import AddIcon from '@mui/icons-material/Add';
import {
    Button,
    DialogContent,
    DialogTitle,
    FormControl,
    FormLabel,
    Input,
    Modal,
    ModalClose,
    ModalDialog,
    Stack,
    Textarea,
} from '@mui/joy';
import { useState } from 'react';
import { useFetcher } from 'react-router-dom';

export default function AddDevice() {
    const fetcher = useFetcher();

    const [isOpen, setIsOpen] = useState(false);

    const closeModal = () => setIsOpen(false);
    const openModal = () => setIsOpen(true);

    return (
        <>
            <Button startDecorator={<AddIcon />} onClick={openModal}>
                Add Device
            </Button>
            <Modal
                aria-labelledby="add-device-modal-title"
                open={isOpen}
                onClose={closeModal}
                sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center' }}
            >
                <ModalDialog minWidth="450px">
                    <ModalClose onClick={closeModal} />
                    <DialogTitle>Add New Device</DialogTitle>
                    <DialogContent>Enter the details for the device you want to add.</DialogContent>
                    <fetcher.Form method="POST" onSubmit={closeModal}>
                        <Stack spacing="20px" mt="4px" mb="32px">
                            <FormControl>
                                <FormLabel>Name</FormLabel>
                                <Input autoFocus required name="name" />
                            </FormControl>
                            <FormControl>
                                <FormLabel>Location (Optional)</FormLabel>
                                <Input name="location" />
                            </FormControl>
                            <FormControl>
                                <FormLabel>Description (Optional)</FormLabel>
                                <Textarea name="description" minRows={4} maxRows={4} />
                            </FormControl>
                        </Stack>
                        <Button type="submit" size="lg" sx={{ width: '100%' }}>
                            Submit
                        </Button>
                    </fetcher.Form>
                </ModalDialog>
            </Modal>
        </>
    );
}
