import { useRef, useState } from 'react';
import {
    Box,
    Button,
    Card,
    CardActions,
    CardOverflow,
    Divider,
    FormControl,
    FormLabel,
    Input,
    Stack,
    Textarea,
    Typography,
} from '@mui/joy';
import { ApiDeviceDto } from '../api/types';
import { useFetcher } from 'react-router-dom';

const DEFAULT_UNDEFINED_FIELD_VALUE = '';

interface DeviceInfoCardProps {
    device: ApiDeviceDto;
}

type DisplayedFields = keyof Pick<ApiDeviceDto, 'name' | 'location' | 'description'>;
type IFormChanges = Record<DisplayedFields, boolean>;

const initialFormChanges: IFormChanges = {
    name: false,
    location: false,
    description: false,
};

export default function DeviceEditableInfoCard({ device }: DeviceInfoCardProps) {
    const nameRef = useRef<HTMLInputElement | null>(null);
    const locationRef = useRef<HTMLInputElement | null>(null);
    const descriptionRef = useRef<HTMLTextAreaElement | null>(null);

    const fetcher = useFetcher();

    const [formChanges, setFormChanges] = useState<IFormChanges>(initialFormChanges);

    const resetFormToHaveNoChanges = () => setFormChanges(initialFormChanges);

    // Detect whether changes have been made to the form from the original device data
    // which we can use for the UI. This saves re-rendering the entire form on every change
    // to one of the form fields.
    const handleFieldChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        let fieldName = e.target.name;

        if (fieldName !== 'name' && fieldName !== 'description' && fieldName !== 'location') {
            return;
        }

        fieldName = fieldName as DisplayedFields;
        const deviceValue = device[fieldName as keyof ApiDeviceDto] as string | undefined;

        const hasChangedFromSaved =
            typeof deviceValue === 'string'
                ? deviceValue.trim() !== e.target.value.trim()
                : e.target.value.trim() !== DEFAULT_UNDEFINED_FIELD_VALUE;

        const needsChanging = formChanges[fieldName as DisplayedFields] !== hasChangedFromSaved;

        if (needsChanging) {
            setFormChanges((prevFormChanges) => ({
                ...prevFormChanges,
                [fieldName]: hasChangedFromSaved,
            }));
        }
    };

    const restoreForm = () => {
        if (nameRef.current) {
            nameRef.current.value = device.name;
        }
        if (locationRef.current) {
            locationRef.current.value = device.location ?? DEFAULT_UNDEFINED_FIELD_VALUE;
        }
        if (descriptionRef.current) {
            descriptionRef.current.value = device.description ?? DEFAULT_UNDEFINED_FIELD_VALUE;
        }
        resetFormToHaveNoChanges();
    };

    const determineFieldColor = (fieldName: DisplayedFields) => {
        const hasChanged = formChanges[fieldName];
        return hasChanged ? 'primary' : 'neutral';
    };

    const formHasNoChanges = Object.values(formChanges).every((hasChanged) => !hasChanged);

    return (
        <fetcher.Form method="PUT" onSubmit={resetFormToHaveNoChanges}>
            <Card variant="outlined">
                <Box>
                    <Typography level="title-lg" mb="4px">
                        Display Info
                    </Typography>
                    <Typography level="body-sm">
                        Edit device information here. Unsaved values are highlighted in blue.
                    </Typography>
                </Box>
                <Divider />
                <Stack spacing="24px" mt="8px" mb="20px">
                    <FormControl>
                        <FormLabel>Name</FormLabel>
                        <Input
                            name="name"
                            slotProps={{ input: { ref: nameRef } }}
                            required
                            defaultValue={device.name}
                            placeholder="Enter device name"
                            onChange={handleFieldChange}
                            color={determineFieldColor('name')}
                        />
                    </FormControl>
                    <FormControl>
                        <FormLabel>Location</FormLabel>
                        <Input
                            name="location"
                            slotProps={{ input: { ref: locationRef } }}
                            defaultValue={device.location ?? DEFAULT_UNDEFINED_FIELD_VALUE}
                            placeholder="Enter device location"
                            onChange={handleFieldChange}
                            color={determineFieldColor('location')}
                        />
                    </FormControl>
                    <FormControl>
                        <FormLabel>Description</FormLabel>
                        <Textarea
                            name="description"
                            slotProps={{ textarea: { ref: descriptionRef } }}
                            defaultValue={device.description ?? DEFAULT_UNDEFINED_FIELD_VALUE}
                            placeholder="Enter device description"
                            minRows={4}
                            maxRows={4}
                            onChange={handleFieldChange}
                            color={determineFieldColor('description')}
                        />
                    </FormControl>
                </Stack>
                <CardOverflow sx={{ borderTop: '1px solid', borderColor: 'divider' }}>
                    <CardActions sx={{ alignSelf: 'flex-end' }}>
                        <Button
                            variant="outlined"
                            color="neutral"
                            type="button"
                            onClick={restoreForm}
                            disabled={formHasNoChanges}
                        >
                            Restore
                        </Button>
                        <Button variant="solid" type="submit" disabled={formHasNoChanges}>
                            Save
                        </Button>
                    </CardActions>
                </CardOverflow>
            </Card>
        </fetcher.Form>
    );
}
