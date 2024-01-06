import { z } from 'zod';

export const apiTemperatureDtoSchema = z.object({
    id: z.number(),
    deviceId: z.number(),
    degreesCelsius: z.number(),
    timestamp: z.string().datetime({ offset: true }),
});

export const apiDeviceDtoSchema = z.object({
    id: z.number(),
    name: z.string(),
    description: z.string().optional(),
    location: z.string().optional(),
    createdAt: z.string().datetime({ offset: true }),
    updatedAt: z.string().datetime({ offset: true }),
});

export const apiUpdateDeviceDtoSchema = z.object({
    name: z.string().optional(),
    description: z.string().optional(),
    location: z.string().optional(),
});

export type ApiDeviceDto = z.infer<typeof apiDeviceDtoSchema>;
export type ApiTemperatureDto = z.infer<typeof apiTemperatureDtoSchema>;
export type ApiUpdateDeviceDto = z.infer<typeof apiUpdateDeviceDtoSchema>;
