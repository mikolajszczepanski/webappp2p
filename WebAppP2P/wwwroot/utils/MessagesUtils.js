import {
    MESSAGE_STATUS_CONFIRMED,
    MESSAGE_STATUS_ERROR,
    MESSAGE_STATUS_NOTCONFIRMED,
    MESSAGE_STATUS_OK
} from '../constants/messageStatuses'

import React from "react";
import { Table, Glyphicon, Alert, Popover, OverlayTrigger } from "react-bootstrap";


export function getUniqueKey(msg) {
    return (msg.id != null ? msg.id.toString() : "") + '-' + (msg.timestamp.toString());
}

export function getAlias(publicKey, aliases) {
    var foundAlias = aliases.filter(a => publicKey === a.publicKey);
    if (foundAlias.length > 0) {
        return foundAlias[0].alias;
    }
    return undefined;
}

export function addressToShortForm(address, aliases) {
    var foundAlias = getAlias(address, aliases);
    if (foundAlias !== undefined) {
        return (
            <span>
                {foundAlias}
            </span>
        )
    }
    const cut = 10;
    var shortAddress = address.length > (2 * cut) ? address.substring(0, cut) + "..." + address.substring(address.length - cut, address.length) : address;
    return (
        <span>
            {shortAddress}
        </span>
    )
}

export function getStatus(msg) {
    if (msg.status === MESSAGE_STATUS_OK) {
        return (
            <p style={{
                color: "#4CAF50",
                fontSize: "24px"
            }}>
                <Glyphicon glyph="envelope" />
            </p>
        )
    }
    else if (msg.status === MESSAGE_STATUS_ERROR) {

        var popover = <Popover id={getUniqueKey(msg) + '-dropdown'}>{msg.error}</Popover>;
        return (
            <OverlayTrigger
                trigger="click"
                placement="top"
                overlay={popover}>
                <p style={{
                    color: "#F44336",
                    fontSize: "24px",
                    cursor: "pointer"
                }}>
                    <Glyphicon glyph="remove" />
                </p>
            </OverlayTrigger>
        )
    }
    else if (msg.status === MESSAGE_STATUS_CONFIRMED) {
        return (
            <p style={{
                color: "#4CAF50",
                fontSize: "24px"
            }}>
                <Glyphicon glyph="ok" />
            </p>
        )
    }
    else if (msg.status === MESSAGE_STATUS_NOTCONFIRMED) {
        return (
            <p style={{
                color: "#FFEB3B",
                fontSize: "24px"
            }}>
                <Glyphicon glyph="refresh" />
            </p>
        )
    }
    else {
        return (
            <p style={{
                color: "#212121",
                fontSize: "24px"
            }}>
                <Glyphicon glyph="question-sign" />
            </p>
        )
    }
}

export function sort(messages) {
    var messagesCopy = [...messages];
    messagesCopy.sort((a, b) => {
        if (a.timestamp > b.timestamp) {
            return -1;
        }
        else if (a.timestamp < b.timestamp) {
            return 1;
        }
        return 0;
    });
    return messagesCopy;
}
