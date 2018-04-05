import { combineReducers } from 'redux'
import { reducer as formReducer } from 'redux-form'

import { keysReducer } from './keysReducer'
import { messagesReducer } from './messagesReducer'
import { aliasesReducer } from './aliasesReducer'


const rootReducer = combineReducers({
    keysReducer,
    messagesReducer,
    aliasesReducer,
    form: formReducer
});

export default rootReducer;