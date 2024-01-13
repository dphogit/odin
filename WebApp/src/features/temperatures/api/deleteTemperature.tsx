import axiosInstance from 'lib/axios';
import { reactQueryClient } from 'providers/ReactQueryClientProvider';
import { ActionFunctionArgs, redirect } from 'react-router-dom';
import { PathNames } from 'routes/util';

async function deleteTemperature(id: string) {
    axiosInstance.delete(`/temperatures/${id}`);
}

export async function deleteTemperatureAction({ params }: ActionFunctionArgs) {
    const temperatureId = params.temperatureId;

    if (!temperatureId) {
        throw new Error('No temperature ID found');
    }

    await deleteTemperature(temperatureId);

    reactQueryClient.invalidateQueries({ queryKey: ['temperatures'] });

    return redirect(`/${PathNames.TEMPERATURES}`);
}
