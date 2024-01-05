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

const DAILY_DEGREE_BUFFER = 4;

export interface DeviceTemperatureGraphDataPoint {
    timestamp: Date;
    degreesCelsius: number;
}

interface DataPoint {
    timestamp: string;
    degreesCelsius: number;
}

interface DeviceTemperatureGraphProps {
    data: DeviceTemperatureGraphDataPoint[];
    containerHeight: number;
}

/**
 * Transforms the raw props data into ascending sorted daily averages for the graph to consume.
 * Specifically, the function groups datapoints by day, which it then averages for each day.
 * @returns An array of pairs in the form of `[day (DD MMM YY), average temp 1dp (°C)]`
 */
function getDailyAverages(data: DeviceTemperatureGraphDataPoint[]): DataPoint[] {
    const groupedByDay = data.reduce((accum, dataPoint) => {
        const day = dayjs(dataPoint.timestamp).format('DD MMM YY');
        if (!accum[day]) {
            accum[day] = [];
        }
        accum[day].push(dataPoint.degreesCelsius);
        return accum;
    }, {} as Record<string, number[]>);

    const dailyAverages = Object.entries(groupedByDay).map(([day, temps]) => {
        const avgTemp = temps.reduce((total, temp) => total + temp, 0) / temps.length;
        return {
            timestamp: day,
            degreesCelsius: Math.round((avgTemp + Number.EPSILON) * 10) / 10,
        };
    });

    dailyAverages.sort((a, b) => dayjs(a.timestamp).diff(dayjs(b.timestamp)));

    return dailyAverages;
}

export default function DeviceTemperatureGraph({
    data,
    containerHeight,
}: DeviceTemperatureGraphProps) {
    const dailyAverageData = getDailyAverages(data);

    return (
        <Box sx={{ width: '100%', height: containerHeight, pl: '4px', pb: '8px' }}>
            <ResponsiveContainer>
                <LineChart data={dailyAverageData} margin={{ right: 20 }}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis
                        dataKey="timestamp"
                        tickFormatter={(timestamp) => dayjs(timestamp).format('DD MMM')}
                        tick={{ dy: 12.5, fontSize: 14 }}
                        height={65}
                        label={{
                            value: 'Day',
                            position: 'insideBottom',
                        }}
                    />
                    <YAxis
                        dataKey="degreesCelsius"
                        width={80}
                        tick={{ dx: -10, fontSize: 14 }}
                        label={{
                            value: 'Degrees (°C)',
                            position: 'insideLeft',
                            angle: -90,
                            offset: 15,
                        }}
                        domain={[
                            // Add buffer spacing around line as we use a relative range
                            (dataMin: number) => Math.floor(dataMin - DAILY_DEGREE_BUFFER),
                            (dataMax: number) => Math.ceil(dataMax + DAILY_DEGREE_BUFFER),
                        ]}
                    />
                    <Tooltip formatter={(value) => [`${value} °C`, null]} />
                    <Line
                        type="linear"
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
