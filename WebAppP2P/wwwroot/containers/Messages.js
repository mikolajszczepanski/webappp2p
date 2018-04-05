import React from "react";
import { connect } from "react-redux";
import { ControlLabel, FormControl, FormGroup, HelpBlock, Button, Alert, Modal, Tabs, Tab, Badge } from "react-bootstrap";
import { SubmissionError } from 'redux-form'
import timestamp from 'unix-timestamp'
import { Circle } from 'better-react-spinkit';

import MessageForm from "../components/MessageForm";
import { MessagesListInbox } from "../components/MessagesListInbox";
import { MessagesListSent } from "../components/MessagesListSent";

import {
    closeConnection,
    configMessage,
    errorConnection,
    receiveMessage,
    sendMessage,
    sendSynchronizationMessage,
    ONE_DAY_IN_SECONDS
} from "../actions/messages"

import {
    MESSAGE_WEBSOCKET_CONNECTION_OPENED
} from '../constants/messageWebSocketStatuses'

const mapStateToProps = (state) => {
    return {
        userLogged: state.keysReducer.keys !== undefined && state.keysReducer.keys.valid === true,
        messages: state.messagesReducer.messages,
        isConnectionOpened: state.messagesReducer.websocket.status === MESSAGE_WEBSOCKET_CONNECTION_OPENED,
        publicKey: state.keysReducer.keys !== undefined && state.keysReducer.keys.public,
        synchronization: state.messagesReducer.synchronization,
        aliases: state.aliasesReducer.aliases
    };
}

const mapDispatchToProps = (dispatch) => {
    return {
        sendMessage: (to, from, title, content) => dispatch(sendMessage(to, from, title, content)),
        sendSynchronizationMessage: (timestampFrom, timestampTo) => dispatch(sendSynchronizationMessage(timestampFrom, timestampTo))
    };
}

const tabName = (text, value) => {
    if (value == 0) {
        <span>{text} <Badge></Badge></span>
    }
    return (
        <span>{text} <Badge>{value}</Badge></span>
    );
}

export class MessagesContainer extends React.Component {
    displayName = "Messages";

    constructor(props) {
        super(props);
        this.state = {
            showNewMessageModal: false,
            key: this.props.location.hash === "#sent" ? 2 : 1
        }
    }

    componentDidMount() {
        document.title = "Messages";
        if (!this.props.userLogged) {
            this.props.history.push("/");
        }
    }

    submit(data) {
        this.props.sendMessage(
            data.to,
            this.props.publicKey,
            data.title,
            data.content
        );
        this.setState({
            showNewMessageModal: false,
            key: 2
        });

        return true;
    }

    handleCloseNewMessageModal() {
        this.setState({
            showNewMessageModal: false
        });
    }

    handleComposeClick() {
        this.setState({
            showNewMessageModal: true
        });
    }

    handleMessageClick = id => {
        if (id === null) {
            return;
        }
        this.props.history.push("/messages/" + id);
    }

    handleSelect(key) {
        this.setState({ key });
    }

    handleSynchronization(e) {
        e.preventDefault();
        this.props.sendSynchronizationMessage(
            this.props.synchronization.timestampFrom,
            this.props.synchronization.timestampTo
        );
    }

    render() {
        var inboxCounter = this.props.messages.filter(m => m.to === this.props.publicKey).length;
        var sentCounter = this.props.messages.filter(m => m.from === this.props.publicKey).length;
        var tabs = (
            <Tabs activeKey={this.state.key} onSelect={this.handleSelect.bind(this)} id="messages-tabs">
                <Tab eventKey={1} title={tabName("Inbox", inboxCounter)}>
                    <MessagesListInbox
                        messages={this.props.messages}
                        aliases={this.props.aliases}
                        submitting={!this.props.isConnectionOpened}
                        selfPublicKey={this.props.publicKey}
                        handleMessageClick={this.handleMessageClick}
                    />
                </Tab>
                <Tab eventKey={2} title={tabName("Sent", sentCounter)}>
                    <MessagesListSent
                        messages={this.props.messages}
                        aliases={this.props.aliases}
                        submitting={!this.props.isConnectionOpened}
                        selfPublicKey={this.props.publicKey}
                        handleMessageClick={this.handleMessageClick}
                    />
                </Tab>
            </Tabs>
        );

        if ((this.props.synchronization.synchronized === undefined || this.props.synchronization.synchronized === true) &&
            (this.props.synchronization.timestampFrom !== undefined && this.props.synchronization.timestampTo !== undefined)
            ) {
            
            var from = timestamp.toDate(this.props.synchronization.timestampFrom).toString();
            var to = timestamp.toDate(this.props.synchronization.timestampTo).toString();
            var nextFrom = timestamp.toDate(this.props.synchronization.timestampFrom - ONE_DAY_IN_SECONDS * 14).toString();
            var nextTo = timestamp.toDate(this.props.synchronization.timestampTo - ONE_DAY_IN_SECONDS * 7).toString();
            var sync = (
                <div>
                    <p>Last messages history synchronization: {from} and {to}</p>
                    <p>
                        <a onClick={this.handleSynchronization.bind(this)} href="#">Load more history messages between {nextFrom} and {nextTo}</a>
                    </p>
                </div>
                );
        }
        else {
            sync = (
                <div>
                    <Circle size={30} />
                    <p>Please wait for end of synchronization...</p>
                </div>
                );
        }

        return (
            <div>
                <div id="newMessage">
                    <Button bsStyle="primary" onClick={this.handleComposeClick.bind(this)}>Compose</Button>
                </div>
                {tabs}
                {sync}
                <hr />
                <Modal
                    show={this.state.showNewMessageModal}
                    onHide={this.handleCloseNewMessageModal.bind(this)}
                    dialogClassName="composeModal"
                >
                    <Modal.Header closeButton>
                        <Modal.Title>New message</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <MessageForm onSubmitHandler={this.submit.bind(this)} aliases={this.props.aliases} />
                    </Modal.Body>
                </Modal>
            </div>
        )
    }
}

export default connect(
    mapStateToProps,
    mapDispatchToProps
)(MessagesContainer);


