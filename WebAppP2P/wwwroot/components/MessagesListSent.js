import {
    MESSAGE_STATUS_CONFIRMED,
    MESSAGE_STATUS_ERROR,
    MESSAGE_STATUS_NOTCONFIRMED,
    MESSAGE_STATUS_OK
} from '../constants/messageStatuses'

import React from "react";
import { Table } from "react-bootstrap";
import { 
    addressToShortForm,
    getStatus,
    getUniqueKey,
    sort
} from "../utils/MessagesUtils";

import timestamp from "unix-timestamp"

export function MessagesListSent(props) {
    var messages = sort(props.messages.filter(m => m.from === props.selfPublicKey));
    return (
        <div>
            <Table striped bordered condensed hover className="messsages-table">
                <thead>
                    <tr style={{
                        display: "hidden"
                    }}>
                        <th width="5%">Status</th>
                        <th width="15%">To</th>
                        <th width="65%">Data</th>
                        <th width="15%">Time</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        messages.map(m => {
                            if (m.id === null) {
                                return (
                                    <tr key={getUniqueKey(m)}>
                                        <td>
                                            {getStatus(m)}
                                        </td>
                                        <td className="center">
                                            {addressToShortForm(m.to, props.aliases)}
                                        </td>
                                        <td>
                                            <h4>
                                                {m.title}
                                                &nbsp;
                                                <small>
                                                    {m.content}
                                                </small>
                                            </h4>
                                        </td>
                                        <td className="center">
                                            <p>
                                                {m.datetime}
                                            </p>
                                        </td>
                                        </tr>
                                    )
                            }
                            return (
                                <tr key={getUniqueKey(m)}>
                                    <td>
                                        { getStatus(m) } 
                                    </td>
                                    <td onClick={e => props.handleMessageClick(m.id)} className="selectable center">
                                        {addressToShortForm(m.to, props.aliases) }
                                    </td>
                                    <td onClick={e => props.handleMessageClick(m.id)} className="selectable">
                                        <h4>
                                            {m.title}
                                            &nbsp;
                                            <small>
                                                {m.content}
                                            </small>
                                        </h4>
                                        
                                    </td>
                                    <td onClick={e => props.handleMessageClick(m.id)} className="selectable center">
                                        <p>
                                            {timestamp.toDate(m.timestamp).toString()}
                                        </p>
                                    </td>
                                </tr>
                            )
                        })
                }
                </tbody>
            </Table>
        </div>
    );
}