import React from "react";
import { connect } from "react-redux";
import { Col, Row, Label } from "react-bootstrap";
import MessageForm from "../components/MessageForm";

import {
    sendMessage,
    currentMessage,
    clearCurrentMessage
} from "../actions/messages"

import { getAlias } from "../utils/MessagesUtils"

const mapStateToProps = (state) => {
    return {
        messages: state.messagesReducer.messages,
        publicKey: state.keysReducer.keys !== undefined && state.keysReducer.keys.public,
        message: state.messagesReducer.current,
        aliases: state.aliasesReducer.aliases
    };
}

const mapDispatchToProps = (dispatch) => {
    return {
        sendMessage: (to, from, title, content) => dispatch(sendMessage(to, from, title, content)),
        currentMessage: (msg) => dispatch(currentMessage(msg)),
        clearCurrentMessage: () => dispatch(clearCurrentMessage())
    };
}

export class MessageDetailsContainer extends React.Component {
    displayName = "MessageDetails";

    constructor(props) {
        super(props);
    }

    componentDidMount() {
        var id = this.props.match.params.id;
        var currentMessage = this.props.messages.filter(m => m.id === id)[0];
        this.props.currentMessage(currentMessage);
    }

    componentWillUnmount() {
        this.props.clearCurrentMessage();
    }

    submit(data) {
        this.props.sendMessage(
            data.to,
            this.props.publicKey,
            data.title,
            data.content
        );
        this.props.history.push("/messages#sent");
        return true;
    }

    render() {
        if (this.props.message === undefined) {
            return (
                <div>
                    <h1>404 <small>This message doesn't exist.</small></h1>
                </div>
            );
        }
        var fromAlias = getAlias(this.props.message.from, this.props.aliases);
        var toAlias = getAlias(this.props.message.to, this.props.aliases);
        var from = fromAlias !== undefined ? fromAlias : this.props.message.from;
        var to = toAlias !== undefined ? toAlias : this.props.message.to;
        return (
            <div>
                <div className="details">
                    <Col xs={12} md={12} className="details-info">
                        <Row>
                            <Col xs={12} md={2} >
                                <h4>From</h4>
                            </Col>
                            <Col xs={12} md={10} >
                                <h4 className="address">
                                    <small>
                                        {from}
                                    </small>
                                </h4>
                            </Col>
                        </Row>
                        <Row>
                            <Col xs={12} md={2} >
                                <h4>To</h4>
                            </Col>
                            <Col xs={12} md={10} >
                                <h4 className="address">
                                    <small>
                                        {to}
                                    </small>
                                </h4>
                            </Col>
                        </Row>
                        <Row>
                            <Col xs={12} md={2} >
                                <h4>Date (timestamp)</h4>
                            </Col>
                            <Col xs={12} md={10} >
                                <h4>
                                    <small>
                                        {this.props.message.datetime} ({this.props.message.timestamp})
                                    </small>
                                </h4>
                            </Col>
                        </Row>
                        <Row>
                            <Col xs={12} md={12} >
                                {this.props.publicKey === this.props.message.from && <Label bsStyle="primary">You sent this message</Label>}
                                {this.props.publicKey === this.props.message.to && <Label bsStyle="primary">This message was sent to you</Label>}
                            </Col>
                        </Row>
                    </Col>
                </div>
                <Col xs={12} md={12}>
                    <hr />
                    <Row className="message-title">
                        <Col xs={12} md={12} >
                            <h4>
                                {this.props.message.title}
                            </h4>
                        </Col>
                    </Row>
                </Col>  
                <Col xs={12} md={12} >
                    <hr />
                    <Row className="message-content">
                        <Col xs={12} md={12} >
                            <p>
                                {this.props.message.content}
                            </p>
                        </Col>
                    </Row>
                </Col>
                <Col xs={12} md={12} className="quick-response" >
                    <hr />
                    <h4>Quick response:</h4>
                    <MessageForm
                        onSubmitHandler={this.submit.bind(this)}
                        aliases={this.props.aliases}
                    />
                </Col>
            </div>
        )
    }
}

export default connect(
    mapStateToProps,
    mapDispatchToProps
)(MessageDetailsContainer);
