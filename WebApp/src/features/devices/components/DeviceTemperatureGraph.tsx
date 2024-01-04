import { Box } from '@mui/joy';
import dayjs from 'dayjs';
import {
    CartesianGrid,
    Line,
    LineChart,
    ResponsiveContainer,
    Tooltip,
    XAxis,
    YAxis,
} from 'recharts';

export interface DeviceTemperatureGraphDataPoint {
    timestamp: Date;
    degreesCelsius: number;
}

interface DeviceTemperatureGraphProps {
    data: DeviceTemperatureGraphDataPoint[];
    containerHeight: number;
}

function formatTimestamp(timestamp: Date): string {
    return dayjs(timestamp).format('DD-MM-YY HH:mm');
}

export default function DeviceTemperatureGraph({
    data,
    containerHeight,
}: DeviceTemperatureGraphProps) {
    return (
        <Box sx={{ width: '100%', height: containerHeight, pl: '4px', pr: '16px', pb: '8px' }}>
            <ResponsiveContainer>
                <LineChart data={data}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis
                        dataKey="timestamp"
                        tickFormatter={formatTimestamp}
                        height={50}
                        label={{
                            value: 'Time',
                            position: 'insideBottomRight',
                        }}
                    />
                    <YAxis
                        dataKey="degreesCelsius"
                        label={{
                            value: 'Degrees (°C)',
                            position: 'insideLeft',
                            angle: -90,
                            offset: 15,
                        }}
                        domain={[
                            (dataMin: number) => Math.min(0, Math.floor(dataMin - 2)),
                            (dataMax: number) => Math.ceil(dataMax + 2),
                        ]}
                    />
                    <Tooltip />
                    <Line type="monotone" dataKey="degreesCelsius" activeDot={{ r: 8 }} />
                </LineChart>
            </ResponsiveContainer>
        </Box>
    );
}
