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

// function transformToAverageDaily(data: DeviceTemperatureGraphDataPoint[]): DeviceTemperatureGraphDataPoint[] {
//     return [];
// }

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
                        tickFormatter={(timestamp) => dayjs(timestamp).format('DD-MM-YY')}
                        tick={{ dy: 12.5 }}
                        height={65}
                        label={{
                            value: 'Time',
                            position: 'insideBottomRight',
                        }}
                    />
                    <YAxis
                        dataKey="degreesCelsius"
                        width={80}
                        tick={{ dx: -5 }}
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
                    <Tooltip formatter={(value) => [`${value} °C`, null]} />
                    <Line
                        type="monotone"
                        dataKey="degreesCelsius"
                        dot={{ r: 6 }}
                        activeDot={{ r: 12 }}
                        strokeWidth={3}
                    />
                </LineChart>
            </ResponsiveContainer>
        </Box>
    );
}
