// General types used across the application

import { useLoaderData } from 'react-router-dom';
import { z } from 'zod';

/** Unwrap the return type from type assertion of the {@link useLoaderData} react-router-dom hook */
export type LoaderReturnType<T extends (...args: any[]) => any> = Awaited<
    ReturnType<ReturnType<T>>
>;

export const collectionResponseMetaSchema = z.object({
    /**
     * The current page number for the collection request.
     */
    page: z.number(),

    /**
     * The maximum number of records per page.
     */
    limit: z.number(),

    /**
     * The total number of records which meet the search criteria regardless of page or limit.
     */
    total: z.number(),

    /**
     * The number of records in the current page.
     */
    count: z.number(),
});

export type CollectionResponseMeta = z.infer<typeof collectionResponseMetaSchema>;

export function createPaginatedResponseSchema<T extends z.ZodTypeAny>(itemSchema: T) {
    return z.object({
        data: z.array(itemSchema),
        _meta: collectionResponseMetaSchema,
    });
}

export const apiTimeSeriesDataPointDtoSchema = z.object({
    timestamp: z.string(),
    value: z.number().nullable(),
});

export const apiTimeSeriesDataPointResponseSchema = apiTimeSeriesDataPointDtoSchema.array();

export type ApiTimeSeriesDataPointDto = z.infer<typeof apiTimeSeriesDataPointDtoSchema>;
export type ApiTimeSeriesDataPointResponse = z.infer<typeof apiTimeSeriesDataPointResponseSchema>;
