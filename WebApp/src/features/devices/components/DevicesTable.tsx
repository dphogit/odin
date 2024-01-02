import { Sheet, Table } from '@mui/joy';
import { IDevice } from '../types';

interface DevicesTableProps {
    devices: IDevice[];
}

export default function DevicesTable({ devices }: DevicesTableProps) {
    return (
        <Sheet sx={{ borderRadius: 'sm' }}>
            <Table aria-label="devices table" size="lg">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Name</th>
                        <th>Location</th>
                    </tr>
                </thead>
                <tbody>
                    {devices.map((device) => (
                        <tr key={device.id}>
                            <td>{device.id}</td>
                            <td>{device.name}</td>
                            <td>{device.location ?? '-'}</td>
                        </tr>
                    ))}
                </tbody>
            </Table>
        </Sheet>
    );
}
