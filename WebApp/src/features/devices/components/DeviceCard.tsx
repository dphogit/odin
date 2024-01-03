import { Button, Card, CardActions, CardContent, Link, Typography } from '@mui/joy';
import { Link as RouterLink } from 'react-router-dom';
import { PathNames } from 'routes/util';
import { IDevice } from '../types';

interface DeviceCardProps {
    device: IDevice;
}

export default function DeviceCard({ device }: DeviceCardProps) {
    return (
        <Card sx={{ p: '20px' }}>
            <CardContent>
                <Typography level="h4" component="h3">
                    <Link
                        component={RouterLink}
                        to={`/${PathNames.DEVICES}/${device.id}`}
                        overlay
                        underline="none"
                        sx={{
                            color: 'text.primary',
                            display: '-webkit-box',
                            WebkitLineClamp: 1,
                            WebkitBoxOrient: 'vertical',
                            overflow: 'hidden',
                        }}
                    >
                        {device.name}
                    </Link>
                </Typography>
                <Typography level="body-xs" fontWeight={700}>
                    {device.location ?? <i>No Location Specified</i>}
                </Typography>
                <Typography
                    level="body-sm"
                    title={device.description ?? undefined}
                    sx={{
                        mt: '12px',
                        display: '-webkit-box',
                        WebkitLineClamp: 2,
                        WebkitBoxOrient: 'vertical',
                        overflow: 'hidden',
                    }}
                >
                    {device.description ?? <i>No Description</i>}
                </Typography>
                <CardActions sx={{ alignSelf: 'flex-end' }}>
                    <Button variant="plain" size="sm">
                        Edit
                    </Button>
                </CardActions>
            </CardContent>
        </Card>
    );
}
