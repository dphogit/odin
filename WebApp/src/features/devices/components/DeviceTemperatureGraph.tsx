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
import { ApiTimeSeriesDataPointDto } from 'types';
import { TimeRangeOptionType } from '..';
import { roundToTenth } from 'util';

function getDateFormatFromTimeRange(timeRange: TimeRangeOptionType): string {
    let dateFormat: string;

    switch (timeRange) {
        case 'month':
        case 'week':
            dateFormat = 'MMM DD';
            break;
        case 'year':
            dateFormat = 'MMM YY';
            break;
        default:
            throw new Error(`Time range "${timeRange}" invalid.`);
    }

    return dateFormat;
}

function prettifyData(
    data: ApiTimeSeriesDataPointDto[],
    dateFormat: string
): ApiTimeSeriesDataPointDto[] {
    return data.map(({ timestamp, value }) => ({
        timestamp: dayjs(timestamp).format(dateFormat),
        value: value ? roundToTenth(value) : null,
    }));
}

interface DeviceTemperatureGraphProps {
    data: ApiTimeSeriesDataPointDto[];
    containerHeight: number;
    isLoading?: boolean;
    timeRange: TimeRangeOptionType;
}

export default function DeviceTemperatureGraph({
    containerHeight,
    data,
    isLoading,
    timeRange,
}: DeviceTemperatureGraphProps) {
    const dateFormat = getDateFormatFromTimeRange(timeRange);
    data = prettifyData(data, dateFormat);

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
                    <LineChart data={data} margin={{ right: 20 }}>
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis
                            dataKey="timestamp"
                            tick={{
                                dx: -5,
                                dy: 30,
                                fontSize: data.length <= 10 ? '14px' : '12px',
                            }}
                            angle={-90}
                            height={100}
                            label={{
                                value: 'time',
                                position: 'insideBottom',
                            }}
                        />
                        <YAxis
                            dataKey="value"
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
                            dataKey="value"
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
