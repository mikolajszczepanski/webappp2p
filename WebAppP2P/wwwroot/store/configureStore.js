import { createStore, applyMiddleware } from 'redux'
import thunk from 'redux-thunk'
import { composeWithDevTools } from 'redux-devtools-extension';

import rootReducer from '../reducers/rootReducer'

import messagesMiddleware from './messagesMiddleware'
import {
    MESSAGE_WEBSOCKET_CONNECTION_CLOSED
} from '../constants/messageWebSocketStatuses'

export function configureStore() {

    let initialStoreDataEmpty = {
        messagesReducer: {
            messages: [],
            websocket: {
                status: MESSAGE_WEBSOCKET_CONNECTION_CLOSED
            },
            synchronization: {
                synchronized: undefined,
                timestampFrom: undefined,
                timestampTo: undefined
            }
        },
        aliasesReducer: {
            aliases: []
        }
    };

    let initialStoreData = initialStoreDataEmpty;

    const store = createStore(
        rootReducer,
        initialStoreData,
        composeWithDevTools(applyMiddleware(
            thunk,
            messagesMiddleware
        ))
    );

    if (module.hot) {
        // Enable Webpack hot module replacement for reducers
        module.hot.accept('../index', () => {
            const nextRootReducer = require('../reducers/rootReducer');
            store.replaceReducer(nextRootReducer);
        });
    }

    return store;
}