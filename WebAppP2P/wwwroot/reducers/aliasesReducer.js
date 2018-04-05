import {
    ADD_ALIAS,
    REMOVE_ALIAS,
    INIT_ALIASES,
    CLEAR_ALIASES
} from "../constants/actionTypes"

export function aliasesReducer(state = [], action) {
    switch (action.type) {
        case ADD_ALIAS:
            var ar = [...state.aliases];
            ar.push({
                id: action.id,
                alias: action.alias,
                publicKey: action.publicKey
            });
            return Object.assign({}, state, {
                aliases: ar

            });
        case REMOVE_ALIAS:
            ar = [...state.aliases];
            ar = ar.filter(a => a.id !== action.id);
            return Object.assign({}, state, {
                aliases: ar
            });
        case INIT_ALIASES:
            return Object.assign({}, state, {
                aliases: action.aliases
            });
        case CLEAR_ALIASES:
            return Object.assign({}, state, {
                aliases: []
            });
        default:
            return state;
    }
}