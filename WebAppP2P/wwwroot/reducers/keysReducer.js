import {
    GENERATE_KEYS,
    SUCCESSFULLY_GENERATED_KEYS,
    UNSUCCESSFULLY_GENERATED_KEYS,
    CHECKED_KEYS,
    CHECKED_KEYS_ERROR,
    CHECK_KEYS,
    ENTER_KEYS,
    CLEAR_KEYS
} from '../constants/actionTypes'

export function keysReducer(state = [], action) {
    switch (action.type) {
        case GENERATE_KEYS:
            return Object.assign({}, state, {
               
            });
        case SUCCESSFULLY_GENERATED_KEYS:
            return Object.assign({}, state, {

            });
        case UNSUCCESSFULLY_GENERATED_KEYS:
            return Object.assign({}, state, {

            });
        case CHECKED_KEYS:
            return Object.assign({}, state, {
                keys: {
                    public: JSON.parse(JSON.stringify(state.keys.public)),
                    private: JSON.parse(JSON.stringify(state.keys.private)),
                    valid: action.payload
                }
            });
        case CHECKED_KEYS_ERROR:
            return Object.assign({}, state, {

            });
        case CHECK_KEYS:
            return Object.assign({}, state, {

            });
        case ENTER_KEYS:
            return Object.assign({}, state, {
                keys: {
                    public: action.payload.publicKey,
                    private: action.payload.privateKey,
                    valid: undefined
                }
            });
        case CLEAR_KEYS:
            return Object.assign({}, state, {
                keys: undefined
            });
        default:
            return state;
    }
}