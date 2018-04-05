import React from "react";
import { connect } from 'react-redux';

import { Button, Grid, Row, Col, Clearfix, Navbar, Nav, NavItem, MenuItem, NavDropdown, Glyphicon, Modal, Label } from 'react-bootstrap';
import { LinkContainer, IndexLinkContainer } from 'react-router-bootstrap'
import { CopyToClipboard } from 'react-copy-to-clipboard';

import { getVersion } from "../common"

import {
    initConnection
} from "../actions/messages"

import {
    MESSAGE_WEBSOCKET_CONNECTION_CLOSED,
    MESSAGE_WEBSOCKET_CONNECTION_OPENED,
    MESSAGE_WEBSOCKET_CONNECTION_INIT
} from '../constants/messageWebSocketStatuses'

const mapStateToProps = (state) => {
    return {
        userLogged: state.keysReducer.keys !== undefined && state.keysReducer.keys.valid === true,
        connectionStatus: state.messagesReducer.websocket.status,
        publicKey: state.keysReducer.keys !== undefined && state.keysReducer.keys.public,
        privateKey: state.keysReducer.keys !== undefined && state.keysReducer.keys.private,
        error: state.messagesReducer.error
    };
}

const mapDispatchToProps = (dispatch, data) => {
    return {
        initConnection: (publicKey, privateKey) => dispatch(initConnection(publicKey, privateKey))
    };
}

class Main extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            showPublicKeyModal: false,
            showError: true,
            publicKeyCopied: false
        }
    }

    componentDidMount() {
        if (this.props.publicKey !== undefined && this.props.privateKey !== undefined) {
            this.props.initConnection(
                this.props.publicKey,
                this.props.privateKey
            );
        }
    }

    handleConnectionStatusClick() {
        if (this.props.connectionStatus !== MESSAGE_WEBSOCKET_CONNECTION_CLOSED) {
            return;
        }
        this.props.initConnection(
            this.props.publicKey,
            this.props.privateKey
        );
    }

    getConnectionStatusJsx(status) {
        if (status === MESSAGE_WEBSOCKET_CONNECTION_OPENED) {
            return (
                <span style={{
                    color: "#388E3C",
                }}>
                    <Glyphicon style={{
                        fontSize: 24 + "px",
                        marginTop: -4 + "px",
                        verticalAlign: "top"
                    }} glyph="ok-sign" />
                    Connected
                 </span>
                )
        }
        else if (status === MESSAGE_WEBSOCKET_CONNECTION_INIT) {
            return (
                <span style={{
                    color: "#FFB300",
                }}>
                    <Glyphicon style={{
                        fontSize: 24 + "px",
                        marginTop: -4 + "px",
                        verticalAlign: "top",
                        cursor: "pointer"
                    }} glyph="refresh" />
                    Connecting...
                </span>
                )
        }
        else if (status === MESSAGE_WEBSOCKET_CONNECTION_CLOSED) {
            return (
                <span style={{
                    color: "#B71C1C",
                    cursor: "pointer"
                }}>
                    <Glyphicon style={{
                        fontSize: 24 + "px",
                        marginTop: -4 + "px",
                        verticalAlign: "top"
                    }} glyph="exclamation-sign" />
                    Disconnected
                </span>
            )
        }
        else {
            return null;
        }
    }

    handleShowPublicKeyClick() {
        this.setState({
            showPublicKeyModal: true
        });
    }

    handleClosePublicKeyModal() {
        this.setState({
            showPublicKeyModal: false,
            publicKeyCopied: false
        });
    }

    handleCloseErrorModal() {
        this.setState({
            showError: false
        });
    }

    render() {
        if (this.props.userLogged) {
            return (
                <div>
                    <Modal show={this.props.error !== undefined && this.state.showError} onHide={this.handleCloseErrorModal.bind(this)}>
                        <Modal.Header closeButton>
                            <Modal.Title style={{
                                color: "#C62828",
                                }}>
                                <span style={{
                                    fontSize: "16px",
                                    paddingRight: "5px"
                                }}>
                                    <Glyphicon glyph="alert" />
                                </span>
                                <span>Error occured</span>
                            </Modal.Title>
                        </Modal.Header>
                        <Modal.Body>
                            <h4>
                                {this.props.error !== undefined && this.props.error.description}
                            </h4>
                        </Modal.Body>
                        <Modal.Footer>
                            <Button onClick={this.handleCloseErrorModal.bind(this)}>Close</Button>
                        </Modal.Footer>
                    </Modal>
                    <div className="menuBar">
                        <Row>
                            <Navbar>
                                <Navbar.Header>
                                    <Navbar.Brand>
                                        <a href="#">WebApp P2P</a>
                                    </Navbar.Brand>
                                    <Navbar.Toggle />
                                </Navbar.Header>
                                <Nav activeKey="1" onSelect={this.handleSelect}>
                                    <IndexLinkContainer to="/"><NavItem>Home</NavItem></IndexLinkContainer>
                                    <LinkContainer to="/messages"><NavItem>Messages</NavItem></LinkContainer>
                                    <LinkContainer to="/aliases"><NavItem>Aliases</NavItem></LinkContainer>
                                    <LinkContainer to="/logout"><NavItem>Logout</NavItem></LinkContainer>
                                </Nav>
                                <Nav pullRight>
                                    <Navbar.Text onClick={this.handleConnectionStatusClick.bind(this)}>
                                        {this.getConnectionStatusJsx(this.props.connectionStatus)}
                                    </Navbar.Text>
                                    <NavDropdown title="Account">
                                        <MenuItem onClick={this.handleShowPublicKeyClick.bind(this)}>Show my public key</MenuItem>
                                    </NavDropdown>
                                </Nav>
                            </Navbar>
                        </Row>
                        <Modal show={this.state.showPublicKeyModal} onHide={this.handleClosePublicKeyModal.bind(this)}>
                            <Modal.Header closeButton>
                                <Modal.Title>Your public key, you can share it to contact with other people</Modal.Title>
                            </Modal.Header>
                            <Modal.Body>
                                <p>{this.props.publicKey}</p>
                                {this.state.publicKeyCopied && <Label style={{ marginLeft: "5px" }} bsStyle="success">Public key was copied</Label>}
                            </Modal.Body>
                            <Modal.Footer>
                                <CopyToClipboard text={this.props.publicKey}
                                    onCopy={() => this.setState({ publicKeyCopied: true})}>
                                    <Button bsStyle="primary">Copy</Button>
                                </CopyToClipboard>   
                                <Button onClick={this.handleClosePublicKeyModal.bind(this)}>Close</Button>
                            </Modal.Footer>
                        </Modal>
                    </div>
                </div>
            )
        }
        return (
            <div className="menuBar">
                <Row>
                    <Navbar>
                        <Navbar.Header>
                            <Navbar.Brand>
                                <a href="#">WebApp P2P</a>
                            </Navbar.Brand>
                            <Navbar.Toggle />
                        </Navbar.Header>
                        <Nav activeKey="1" onSelect={this.handleSelect}>
                            <IndexLinkContainer to="/"><NavItem>Home</NavItem></IndexLinkContainer>
                            <LinkContainer to="/genkeys"><NavItem>Generate new keys</NavItem></LinkContainer>
                            <LinkContainer to="/login"><NavItem>Login</NavItem></LinkContainer>
                        </Nav>
                    </Navbar>
                </Row>
                
            </div>
        )
    }
}

export default connect(
    mapStateToProps,
    mapDispatchToProps
)(Main);