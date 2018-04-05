import 'whatwg-fetch';
import {
    GENERATE_KEYS,
    SUCCESSFULLY_GENERATED_KEYS,
    UNSUCCESSFULLY_GENERATED_KEYS,
    ENTER_KEYS,
    CHECK_KEYS,
    CHECKED_KEYS,
    CHECKED_KEYS_ERROR,
    CLEAR_KEYS
} from '../constants/actionTypes';
import { HttpFetchHeaders, Configuration } from '../common';

export function generateKeys() {
    return {
        type: GENERATE_KEYS
    }
}

export function successfullyGeneratedKeys(payload) {
    return {
        type: SUCCESSFULLY_GENERATED_KEYS,
        payload
    }
}

export function unsuccessfullyGeneretedKeys() {
    return {
        type: UNSUCCESSFULLY_GENERATED_KEYS
    }
}

export function checkKeys(payload) {
    return {
        type: CHECK_KEYS,
        payload
    }
}

export function checkedKeys(payload) {
    return {
        type: CHECKED_KEYS,
        payload
    }
}

export function enterKeys(payload) {
    return {
        type: ENTER_KEYS,
        payload
    }
}

export function checkedKeysError(payload) {
    return {
        type: CHECKED_KEYS_ERROR,
        payload
    }
}

export function clearKeys() {
    return {
        type: CLEAR_KEYS
    }
}

function fetchToGenerateKeysApi() {
    var url = Configuration.serverUrl + 'api/keys';
    return fetch(url, {
        method: 'GET',
        headers: new Headers(HttpFetchHeaders)
    });
}

function fetchToCheckKeysApi(data) {
    var url = Configuration.serverUrl + 'api/keys/check';
    return fetch(url, {
        method: 'POST',
        headers: new Headers(HttpFetchHeaders),
        body: JSON.stringify(data)
    });
}

export function fetchToGenerateKeys() {
    return (dispatch) => {
        dispatch(generateKeys());
        return fetchToGenerateKeysApi().then((response) => {
            if (response.ok) {
                return response.json().then(payload => {
                    dispatch(successfullyGeneratedKeys());
                    return {
                        statusText: response.statusText,
                        status: response.status,
                        ok: response.ok,
                        payload
                    };
                });
            }
            else {
                throw Error(response.status + " " + response.statusText);
            }
        }).catch((err) => {
            dispatch(unsuccessfullyGeneretedKeys());
            throw err;
        }).then((response) => {
            return response;
        });
    }
}

export function fetchToCheckKeys(data) {
    return (dispatch) => {
        dispatch(enterKeys(data));
        dispatch(checkKeys(data));
        return fetchToCheckKeysApi(data).then((response) => {
            if (response.ok) {
                return response.json().then(payload => {
                    dispatch(checkedKeys(payload));
                    return {
                        statusText: response.statusText,
                        status: response.status,
                        ok: response.ok,
                        payload
                    };
                });
            }
            else {
                var error = Error(response.status + " " + response.statusText);
                error.response = response;
                throw error;
            }
        }).catch((err) => {
            console.error(err);
            dispatch(checkedKeysError(err));
            throw err;
        });
    }
}