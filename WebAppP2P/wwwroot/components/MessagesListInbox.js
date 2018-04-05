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


export function MessagesListInbox(props) {
    var messages = sort(props.messages.filter(m => m.to === props.selfPublicKey));
    return (
        <div>
            <Table striped bordered condensed hover className="messsages-table">
                <thead>
                    <tr style={{
                        display: "hidden"
                    }}>
                        <th width="15%">From</th>
                        <th width="50%">Data</th>
                        <th width="15%">Time</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        messages.map(m => {
                            return (
                                <tr key={getUniqueKey(m)}>
                                    <td onClick={e => props.handleMessageClick(m.id)} className="selectable center">
                                        {addressToShortForm(m.from, props.aliases) }
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