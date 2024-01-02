import { Button, Card, CardActions, CardContent, Typography } from '@mui/joy';
import { IDevice } from '../types';

interface DeviceCardProps {
    device: IDevice;
}

export default function DeviceCard({ device }: DeviceCardProps) {
    return (
        <Card sx={{ p: '20px' }}>
            <CardContent>
                <Typography
                    level="h4"
                    component="h3"
                    sx={{
                        display: '-webkit-box',
                        WebkitLineClamp: 1,
                        WebkitBoxOrient: 'vertical',
                        overflow: 'hidden',
                    }}
                >
                    {device.name}
                </Typography>
                <Typography level="body-xs" fontWeight={700}>
                    {device.location ?? 'No Location Specified'}
                </Typography>
                <Typography
                    level="body-sm"
                    title={device.description}
                    sx={{
                        mt: '12px',
                        display: '-webkit-box',
                        WebkitLineClamp: 2,
                        WebkitBoxOrient: 'vertical',
                        overflow: 'hidden',
                    }}
                >
                    {device.description}
                </Typography>
            </CardContent>
            <CardActions sx={{ pt: '8px', justifyContent: 'flex-end' }}>
                <div>
                    <Button variant="plain">Edit</Button>
                </div>
            </CardActions>
        </Card>
    );
}
