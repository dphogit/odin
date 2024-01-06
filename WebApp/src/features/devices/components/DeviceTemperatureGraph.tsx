import { Box, CircularProgress, Stack, Typography } from '@mui/joy';
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

interface DataPoint {
    timestamp: string;
    degreesCelsius: number | null;
}

/**
 * Obtains the daily pairs of (day, average temperature) for the last specified number of days.
 * If there is no data for a day, the average temperature will be null.
 */
function getDailyAverages(data: DeviceTemperatureGraphDataPoint[], days: number): DataPoint[] {
    function calculateAverage(temperatures: number[]) {
        if (temperatures.length === 0) return null;
        return temperatures.reduce((total, temp) => total + temp, 0) / temperatures.length;
    }

    function roundToTenth(num: number) {
        return Math.round((num + Number.EPSILON) * 10) / 10;
    }

    const format = 'ddd DD MMMM YYYY';
    const startDate = dayjs().subtract(days, 'day').startOf('day');
    const endDate = dayjs().endOf('day');

    const groupedByDay: Record<string, number[]> = {};

    for (let date = startDate; date.isBefore(endDate); date = date.add(1, 'day')) {
        groupedByDay[date.format(format)] = [];
    }

    data.forEach((dataPoint) => {
        const day = dayjs(dataPoint.timestamp).format(format);
        if (groupedByDay[day]) {
            groupedByDay[day].push(dataPoint.degreesCelsius);
        }
    });

    const dailyAverages = Object.entries(groupedByDay).map(([day, temps]) => {
        const avgTemp = calculateAverage(temps);
        return {
            timestamp: day,
            degreesCelsius: avgTemp === null ? null : roundToTenth(avgTemp),
        };
    });

    dailyAverages.sort((a, b) => dayjs(a.timestamp).diff(dayjs(b.timestamp)));

    return dailyAverages;
}

interface DeviceTemperatureGraphProps {
    data: DeviceTemperatureGraphDataPoint[];
    containerHeight: number;
    days: number;
    isLoading?: boolean;
}

export default function DeviceTemperatureGraph({
    containerHeight,
    data,
    days,
    isLoading,
}: DeviceTemperatureGraphProps) {
    const dailyAverageData = getDailyAverages(data, days);

    return (
        <Box sx={{ width: '100%', height: containerHeight, pl: '4px', pb: '8px' }}>
            {isLoading ? (
                <Box
                    sx={{
                        height: '100%',
                        display: 'flex',
                        flexDirection: 'column',
                        justifyContent: 'center',
                        alignItems: 'center',
                    }}
                >
                    <Stack alignItems="center" spacing="16px">
                        <CircularProgress size="lg" />
                        <Typography>Loading...</Typography>
                    </Stack>
                </Box>
            ) : (
                <ResponsiveContainer>
                    <LineChart data={dailyAverageData} margin={{ right: 20 }}>
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis
                            dataKey="timestamp"
                            tickFormatter={(timestamp) => dayjs(timestamp).format('DD MMM')}
                            tick={{
                                dx: -5,
                                dy: 30,
                                fontSize: dailyAverageData.length <= 10 ? '14px' : '12px',
                            }}
                            angle={-90}
                            height={100}
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
                        />
                        <Tooltip formatter={(value) => [`${value} °C`, null]} />
                        <Line
                            connectNulls
                            type="linear"
                            dataKey="degreesCelsius"
                            dot={{ r: 6 }}
                            activeDot={{ r: 12 }}
                            strokeWidth={3}
                        />
                    </LineChart>
                </ResponsiveContainer>
            )}
        </Box>
    );
}
