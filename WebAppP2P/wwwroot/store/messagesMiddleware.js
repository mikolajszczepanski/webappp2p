import { Configuration } from '../common';

import {
    GET_WEBSOCKET_MESSAGE,
    SEND_WEBSOCKET_MESSAGE,
    WEBSOCKET_OPEN_CONNECTION,
    WEBSOCKET_CLOSE_CONNECTION,
    WEBSOCKET_ERROR_CONNECTION,
    WEBSOCKET_INIT_CONNECTION,
    SEND_WEBSOCKET_CONFIG_MESSAGE,
    GET_WEBSOCKET_MESSAGE_ERROR,
    SEND_WEBSOCKET_SYNCHRONIZATION_MESSAGE
} from '../constants/actionTypes'

import {
    sendConfigMessage,
    receiveMessage,
    sendMessage,
    closeConnection,
    errorConnection,
    openConnection,
    receiveMessageConfirmation,
    receiveMessageError,
    receiveError,
    receiveEndOfSynchronization,
    sendSynchronizationMessage
} from '../actions/messages'

function createWebSocket(url, store, action)
{
    var ws = new WebSocket(url);
    ws.addEventListener('open', function (event) {
        store.dispatch(openConnection())
        store.dispatch(sendConfigMessage(
            action.publicKey,
            action.privateKey
        ));
        store.dispatch(sendSynchronizationMessage());
    });
    ws.addEventListener('message', function (event) {
        var msg = JSON.parse(event.data);
        if (msg.Type === 'WEBSOCKET_MESSAGE') {
            store.dispatch(receiveMessage(
                msg.Data.Id,
                msg.Data.To,
                msg.Data.From,
                msg.Data.Title,
                msg.Data.Content,
                msg.Data.Timestamp,
                msg.Data.DateTime
            ));
        }
        else if (msg.Type === 'WEBSOCKET_MESSAGE_CONFIRMATION'){
            store.dispatch(receiveMessageConfirmation(
                msg.Data.CorrelationId,
                msg.Data.Id,
                msg.Data.Timestamp,
                msg.Data.DateTime
            ));
        }
        else if (msg.Type === 'WEBSOCKET_MESSAGE_ERROR') {
            store.dispatch(receiveMessageError(
                msg.Data.CorrelationId,
                msg.Data.Description,
                msg.Data.DateTime
            ));
        }
        else if (msg.Type === 'WEBSOCKET_ERROR') {
            store.dispatch(receiveError(
                msg.Data.Description,
                msg.Data.Exception
            ));
        }
        else if (msg.Type === 'WEBSOCKET_END_OF_SYNCHRONIZATION') {
            store.dispatch(receiveEndOfSynchronization(
                msg.Data
            ));
        }
        else {
            throw new Error("Not supported message type: " + msg.Type);
        }
    });
    ws.addEventListener('close', function (event) {
        store.dispatch(closeConnection());
    });
    ws.addEventListener('error', function (event) {
        store.dispatch(errorConnection());
    });
    return ws;
}

function createMessagesMiddleware() {
    let activeWebsocket = null;

    const messagesMiddleWare = store => next => action => {
        console.log("Middleware triggered:", action);
        
        switch (action.type) {
            case WEBSOCKET_INIT_CONNECTION:
                activeWebsocket = createWebSocket(Configuration.serverWsUrl + '/messages', store, action);
                break;
            case WEBSOCKET_OPEN_CONNECTION:
                break;
            case WEBSOCKET_CLOSE_CONNECTION:
                if (activeWebsocket !== null) {
                    activeWebsocket.onclose = function () { };
                    activeWebsocket.close();
                    activeWebsocket = null;
                }
                break;
            case WEBSOCKET_ERROR_CONNECTION:
                activeWebsocket = null;
                break;
            case GET_WEBSOCKET_MESSAGE:
                break;
            case SEND_WEBSOCKET_SYNCHRONIZATION_MESSAGE:
            case SEND_WEBSOCKET_CONFIG_MESSAGE:
            case SEND_WEBSOCKET_MESSAGE:
                var { type, ...data } = action; 
                var message = {
                    type,
                    data
                }
                var msgStr = JSON.stringify(message);
                console.info("Sending " + msgStr);
                activeWebsocket.send(msgStr);
                break;
            case GET_WEBSOCKET_MESSAGE_ERROR:
                break;
            default:
                break;
        }
        
        next(action);
    }

    return messagesMiddleWare;
}

const messagesMiddleware = createMessagesMiddleware();

export default messagesMiddleware;