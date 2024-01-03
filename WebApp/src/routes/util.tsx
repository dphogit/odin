import { ActionFunctionArgs, ParamParseKey, Params } from 'react-router-dom';

export const PathNames = {
    DEVICES: 'devices',
    DEVICE_DETAILS: ':deviceId',
    UNITS: 'units',
} as const;

export interface LoaderFnArgsTypedParams<T extends keyof typeof PathNames>
    extends ActionFunctionArgs {
    params: Params<ParamParseKey<(typeof PathNames)[T]>>;
}
