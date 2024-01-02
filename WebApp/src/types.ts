// General types used across the application

import { useLoaderData } from 'react-router-dom';

/** Unwrap the return type from type assertion of the {@link useLoaderData} react-router-dom hook */
export type LoaderReturnType<T extends (...args: any[]) => any> = Awaited<
    ReturnType<ReturnType<T>>
>;
