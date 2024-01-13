import { Sheet, Table } from '@mui/joy';
import { ApiTemperatureWithDeviceDto } from '../api';
import dayjs from 'dayjs';

function formatTimestamp(timestamp: string) {
    return dayjs(timestamp).format('MMM DD, YYYY hh:mm:ss a');
}

interface TemperaturesTableProps {
    temperatures: ApiTemperatureWithDeviceDto[];
}

export default function TemperaturesTable({ temperatures }: TemperaturesTableProps) {
    return (
        <Sheet variant="outlined" sx={{ borderRadius: 'sm' }}>
            <Table aria-label="Temperatures Table" noWrap>
                <thead>
                    <tr>
                        <th>Timestamp</th>
                        <th style={{ width: '150px' }}>Degrees&nbsp;(°C)</th>
                        <th>Device</th>
                        <th>Location</th>
                        <th style={{ width: '100px' }}>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {temperatures.map((t) => (
                        <tr key={t.id}>
                            <td>{formatTimestamp(t.timestamp)}</td>
                            <td>{t.degreesCelsius.toFixed(2)}</td>
                            <td>{t.device.name}</td>
                            <td>{t.device.location}</td>
                            <td>...</td>
                        </tr>
                    ))}
                </tbody>
            </Table>
        </Sheet>
    );
}
