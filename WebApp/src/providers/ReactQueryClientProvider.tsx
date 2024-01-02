// react-router provides data fetching conventions and react-query optimises these with caching etc.
// react-router is about "when", data caching libraries (react-query) are about "what".
// https://tkdodo.eu/blog/react-query-meets-react-router

import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

export const reactQueryClient = new QueryClient();

export interface ReactQueryClientProviderProps {
    children: React.ReactNode;
}

export default function ReactQueryClientProvider({ children }: ReactQueryClientProviderProps) {
    return <QueryClientProvider client={reactQueryClient}>{children}</QueryClientProvider>;
}
