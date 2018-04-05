import {
    WEBSOCKET_CLOSE_CONNECTION,
    SEND_WEBSOCKET_CONFIG_MESSAGE,
    WEBSOCKET_ERROR_CONNECTION,
    GET_WEBSOCKET_MESSAGE,
    SEND_WEBSOCKET_MESSAGE,
    WEBSOCKET_OPEN_CONNECTION,
    CLEAR_MESSAGES,
    GET_WEBSOCKET_CONFIRM_MESSAGE,
    GET_WEBSOCKET_MESSAGE_ERROR,
    WEBSOCKET_INIT_CONNECTION,
    SEND_WEBSOCKET_SYNCHRONIZATION_MESSAGE,
    CURRENT_MESSAGE,
    CLEAR_CURRENT_MESSAGE,
    GET_WEBSOCKET_ERROR,
    GET_END_OF_SYNCHRONIZATION
} from '../constants/actionTypes';
import {
    MESSAGE_STATUS_NOTCONFIRMED,
    MESSAGE_STATUS_OK
} from '../constants/messageStatuses'

import { HttpFetchHeaders, Configuration } from '../common';

function getCurrentTimestamp() {
    return Math.floor(Date.now() / 1000);
}

export function closeConnection() {
    return {
        type: WEBSOCKET_CLOSE_CONNECTION
    };
}

export function initConnection(publicKey,privateKey) {
    return {
        type: WEBSOCKET_INIT_CONNECTION,
        publicKey,
        privateKey
    };
}

export function openConnection() {
    return {
        type: WEBSOCKET_OPEN_CONNECTION
    };
}

export function errorConnection(payload) {
    return {
        type: WEBSOCKET_ERROR_CONNECTION,
        payload
    };
}

export function sendConfigMessage(publicKey,privateKey) {
    return {
        type: SEND_WEBSOCKET_CONFIG_MESSAGE,
        publicKey,
        privateKey
    };
}

export function receiveMessage(id, to, from, title, content, timestamp, datetime) {
    return {
        type: GET_WEBSOCKET_MESSAGE,
        id,
        to,
        from,
        title,
        content,
        timestamp,
        datetime,
        correlationId: null,
        status: MESSAGE_STATUS_OK
    };
}

export function sendMessage(to, from, title, content) {
    return {
        type: SEND_WEBSOCKET_MESSAGE,
        id: null,
        to,
        from,
        title,
        content,
        timestamp: getCurrentTimestamp(),
        datetime: null,
        correlationId: getCorrelationId(),
        status: MESSAGE_STATUS_NOTCONFIRMED
    };
}

export function receiveMessageError(correlationId, description, datetime) {
    return {
        type: GET_WEBSOCKET_MESSAGE_ERROR,
        correlationId,
        description,
        datetime
    };
}

export function receiveError(description, exception) {
    return {
        type: GET_WEBSOCKET_ERROR,
        description,
        exception
    }
}

export function clearMessages() {
    return {
        type: CLEAR_MESSAGES
    };
}

export function receiveMessageConfirmation(correlationId, id, timestamp, datetime) {
    return {
        type: GET_WEBSOCKET_CONFIRM_MESSAGE,
        correlationId,
        id,
        timestamp,
        datetime
    };
}


export function sendSynchronizationMessage(timestampFrom, timestampTo) {
    var timestamp = getCurrentTimestamp();
    if (timestampFrom === undefined || timestampTo === undefined) {
        return {
            type: SEND_WEBSOCKET_SYNCHRONIZATION_MESSAGE,
            timestampFrom: getCurrentTimestamp() - (7 * ONE_DAY_IN_SECONDS),
            timestampTo: getCurrentTimestamp()
        }
    }
    else {
        return {
            type: SEND_WEBSOCKET_SYNCHRONIZATION_MESSAGE,
            timestampFrom: timestampFrom - (7 * ONE_DAY_IN_SECONDS),
            timestampTo: timestampFrom
        }
    }
}

export function currentMessage(msg) {
    return {
        type: CURRENT_MESSAGE,
        message: msg
    }
}

export function clearCurrentMessage() {
    return {
        type: CLEAR_CURRENT_MESSAGE
    }
}

export function receiveEndOfSynchronization(data) {
    return {
        type: GET_END_OF_SYNCHRONIZATION,
        data
    }
}

function getCorrelationId() {
    return Math.floor(Math.random() * Math.pow(10, 8));
}

export const ONE_DAY_IN_SECONDS = 24 * 60 * 60;