import { apiDeviceDtoSchema } from 'features/devices/api/types';
import { createPaginatedResponseSchema } from 'types';
import { z } from 'zod';

export const apiTemperatureDtoSchema = z.object({
    id: z.number(),
    deviceId: z.number(),
    degreesCelsius: z.number(),
    timestamp: z.string().datetime({ offset: true }),
});

export const apiTemperatureWithDeviceDtoSchema = apiTemperatureDtoSchema.extend({
    device: apiDeviceDtoSchema,
});

export const getTemperaturesResponseSchema = createPaginatedResponseSchema(
    apiTemperatureWithDeviceDtoSchema
);

export type ApiTemperatureDto = z.infer<typeof apiTemperatureDtoSchema>;
export type ApiTemperatureWithDeviceDto = z.infer<typeof apiTemperatureWithDeviceDtoSchema>;
export type GetTemperaturesResponse = z.infer<typeof getTemperaturesResponseSchema>;
