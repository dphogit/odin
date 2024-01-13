import { ActionFunctionArgs, ParamParseKey, Params } from 'react-router-dom';

export const PathNames = {
    DEVICES: 'devices',
    DEVICE_NEW: 'new',
    DEVICE_DETAILS: ':deviceId',
    DEVICE_DELETE: 'delete',
    TEMPERATURES: 'temperatures',
    TEMPERATURE_DETAILS: ':temperatureId',
    TEMPERATURE_DELETE: 'delete',
} as const;

export interface LoaderFnArgsTypedParams<T extends keyof typeof PathNames>
    extends ActionFunctionArgs {
    params: Params<ParamParseKey<(typeof PathNames)[T]>>;
}
