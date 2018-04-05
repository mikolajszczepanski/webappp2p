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
} from '../constants/actionTypes'
import {
    MESSAGE_STATUS_CONFIRMED,
    MESSAGE_STATUS_ERROR
} from '../constants/messageStatuses'
import {
    MESSAGE_WEBSOCKET_CONNECTION_CLOSED,
    MESSAGE_WEBSOCKET_CONNECTION_OPENED,
    MESSAGE_WEBSOCKET_CONNECTION_INIT
} from '../constants/messageWebSocketStatuses'

export function messagesReducer(state = [], action) {
    switch (action.type) {
        case WEBSOCKET_OPEN_CONNECTION:
            return Object.assign({}, state, {
                websocket: {
                    status: MESSAGE_WEBSOCKET_CONNECTION_OPENED,
                    url: action.url,
                    error: state.websocket.error
                }
            });
        case WEBSOCKET_CLOSE_CONNECTION:
            return Object.assign({}, state, {
                websocket: {
                    status: MESSAGE_WEBSOCKET_CONNECTION_CLOSED,
                    url: state.websocket.url,
                    error: state.websocket.error
                }
            });
        case WEBSOCKET_ERROR_CONNECTION:
            return Object.assign({}, state, {
                websocket: {
                    status: MESSAGE_WEBSOCKET_CONNECTION_CLOSED,
                    url: state.websocket.url,
                    error: action.payload
                }
            });
        case WEBSOCKET_INIT_CONNECTION:
            return Object.assign({}, state, {
                websocket: {
                    status: MESSAGE_WEBSOCKET_CONNECTION_INIT,
                    url: state.websocket.url
                }
            });
        case SEND_WEBSOCKET_CONFIG_MESSAGE: {
            return state;
        }
        case SEND_WEBSOCKET_MESSAGE:
            var newArray = [...state.messages];
            var { type, ...msg } = action;
            newArray.push(msg);
            return Object.assign({}, state, {
                messages: newArray
            });
        case GET_WEBSOCKET_MESSAGE:
            if (state.messages.map(msg => msg.id).filter(id => id === action.id).length > 0) {
                return state;
            }
            var newArray = [...state.messages];
            var { type, ...msg } = action; 
            newArray.push(msg);
            return Object.assign({}, state, {
                messages: newArray
            });
        case GET_WEBSOCKET_CONFIRM_MESSAGE:
            var copyArray = [...state.messages];
            var confirmed = copyArray.filter(m => m.correlationId === action.correlationId).map(m => {
                m.status = MESSAGE_STATUS_CONFIRMED;
                m.datetime = action.datetime;
                m.id = action.id;
                return m;
            })
            newArray = [...copyArray.filter(m => m.correlationId === null || m.correlationId !== action.correlationId), ...confirmed];
            return Object.assign({}, state, {
                messages: newArray
            });
        case CLEAR_MESSAGES:
            return Object.assign({}, state, {
                messages: [],
                current: undefined,
                synchronization: {
                    synchronized: false,
                    timestampFrom: undefined,
                    timestampTo: undefined
                }
            });
        case GET_WEBSOCKET_MESSAGE_ERROR:
            var copyArray = [...state.messages];
            var msgError = copyArray.filter(m => m.correlationId === action.correlationId).map(m => {
                m.status = MESSAGE_STATUS_ERROR;
                m.error = action.description;
                return m;
            })
            newArray = [...copyArray.filter(m => m.correlationId === null || m.correlationId !== action.correlationId), ...msgError];
            return Object.assign({}, state, {
                messages: newArray
            });
        case SEND_WEBSOCKET_SYNCHRONIZATION_MESSAGE:
            return Object.assign({}, state, {
                synchronization: {
                    synchronized: false,
                    last: state.synchronization.last,
                    timestampFrom: action.timestampFrom,
                    timestampTo: action.timestampTo
                }
            });
        case GET_END_OF_SYNCHRONIZATION:
            return Object.assign({}, state, {
                synchronization: {
                    synchronized: true,
                    last: action.data,
                    timestampFrom: state.synchronization.timestampFrom,
                    timestampTo: state.synchronization.timestampTo,
                }
            });
        case CURRENT_MESSAGE:
            return Object.assign({}, state, {
                current: action.message
            });
        case CLEAR_CURRENT_MESSAGE:
            return Object.assign({}, state, {
                current: undefined
            });
        case GET_WEBSOCKET_ERROR:
            return Object.assign({}, state, {
                error: {
                    description: action.description,
                    exception: action.exception,
                    showed: false
                }
            });
        default:
            return state;
    }
}