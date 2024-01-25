import { z } from 'zod';

export const apiDeviceDtoSchema = z.object({
    id: z.number(),
    name: z.string(),
    description: z.string().nullish(),
    location: z.string().nullish(),
    createdAt: z.string().datetime({ offset: true }),
    updatedAt: z.string().datetime({ offset: true }),
});

export const apiCreateDeviceDtoSchema = z.object({
    name: z.string(),
    description: z.string().nullish(),
    location: z.string().nullish(),
});

export const apiUpdateDeviceDtoSchema = z.object({
    name: z.string().optional(),
    description: z.string().optional(),
    location: z.string().optional(),
});

export type ApiDeviceDto = z.infer<typeof apiDeviceDtoSchema>;
export type ApiCreateDeviceDto = z.infer<typeof apiCreateDeviceDtoSchema>;
export type ApiUpdateDeviceDto = z.infer<typeof apiUpdateDeviceDtoSchema>;
