import {
    ADD_ALIAS,
    REMOVE_ALIAS,
    INIT_ALIASES,
    CLEAR_ALIASES
} from "../constants/actionTypes"

export function addAlias(alias, publicKey, userPublicKey) {
    return (dispatch, getState) => {
        var oldAliases = getState().aliasesReducer.aliases;
        if (oldAliases.filter(a => a.alias === alias).length > 0) {
            throw new Error("Aliases cannot duplicate");
        }
        var aliasToAdd = addAliasActionCreator(alias, publicKey);
        dispatch(aliasToAdd);
        var aliases = getState().aliasesReducer.aliases;
        saveAliasesInLocalStorage(userPublicKey, aliases);
    }
}

export function removeAlias(id, userPublicKey) {
    return (dispatch, getState) => {
        var aliasToRemove = removeAliasActionCreator(id);
        dispatch(aliasToRemove);
        var aliases = getState().aliasesReducer.aliases;
        saveAliasesInLocalStorage(userPublicKey, aliases);
    }
}

function addAliasActionCreator(alias,publicKey) {
    return {
        type: ADD_ALIAS,
        id: Date.now(),
        alias,
        publicKey
    }
}

function removeAliasActionCreator(id) {
    return {
        type: REMOVE_ALIAS,
        id
    }
}

export function loadAliasesFromLocalStorage(publicKey) {
    var data = JSON.parse(localStorage.getItem(publicKey));
    if (data === null || data === undefined) {
        return [];
    }
    return data;
}

export function saveAliasesInLocalStorage(publicKey, aliases) {
    localStorage.setItem(publicKey, JSON.stringify(aliases));
}

export function initAliases(publicKey, aliases) {
    return {
        type: INIT_ALIASES,
        aliases
    }
}

export function clearAliases() {
    return {
        type: CLEAR_ALIASES
    }
}