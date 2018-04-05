import React from "react";
import { connect } from "react-redux";
import { ControlLabel, FormControl, FormGroup, HelpBlock, Button, Alert, Table, Modal, Label, Col, Row } from "react-bootstrap";
import { CopyToClipboard } from 'react-copy-to-clipboard';
import { reset } from 'redux-form';
import { SubmissionError } from 'redux-form'

import AddAliasForm from "../components/AddAliasForm";
import { Configuration } from "../common";

import { addAlias, removeAlias } from "../actions/aliases"

const mapStateToProps = (state) => {
    return {
        userLogged: state.keysReducer.keys !== undefined && state.keysReducer.keys.valid === true,
        publicKey: state.keysReducer.keys !== undefined && state.keysReducer.keys.public,
        aliases: state.aliasesReducer.aliases
    };
}

const mapDispatchToProps = (dispatch) => {
    return {
        add: (alias, publicKey, userPublicKey) => dispatch(addAlias(alias, publicKey, userPublicKey)),
        remove: (id, userPublicKey) => dispatch(removeAlias(id, userPublicKey)),
        reset: (name) => dispatch(reset(name))
    };
}

export class AliasesContainer extends React.Component {
    displayName = "Aliases";

    constructor() {
        super();
        this.state = {
            showAliasModal: false
        };
    }

    componentDidMount() {
        document.title = "Aliases";
        if (!this.props.userLogged) {
            this.props.history.push("/");
        }
    }

    addAlias(data) {
        try {
            this.props.add(data.alias, data.publicKey, this.props.publicKey);
            this.props.reset('addalias');
        }
        catch(ex){
            throw new SubmissionError({
                _error: ex.message
            });
        }
    }

    removeAlias(id, e) {
        e.preventDefault();
        this.props.remove(id, this.props.publicKey);
    }

    showAlias(id, e) {
        e.preventDefault();
        this.setState({
            showAliasModal: true,
            alias: this.props.aliases.filter(a => a.id === id)[0]
        });
    }

    handleCloseAliasModal() {
        this.setState({
            showAliasModal: false,
            alias: undefined,
            publicKeyCopied: false
        });
    }

    render() {
        var table = (
            <Table striped bordered condensed hover className="messsages-table">
                <thead>
                    <tr>
                        <th width="20%">Alias</th>
                        <th width="60%">Public Key</th>
                        <th width="20%">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        this.props.aliases.map(a => {
                            return (
                                <tr key={a.id}>
                                    <td>{a.alias}</td>
                                    <td>{a.publicKey}</td>
                                    <td>
                                        <Button onClick={this.showAlias.bind(this, a.id)}>Show</Button>
                                        &nbsp;
                                        <Button className="btn-danger" onClick={this.removeAlias.bind(this, a.id)}>Remove</Button>
                                    </td>
                                </tr>
                            )
                        })
                    }
                </tbody>
            </Table>
        );
        var modal = (
            <Modal show={this.state.showAliasModal} onHide={this.handleCloseAliasModal.bind(this)}>
                <Modal.Header closeButton>
                    <Modal.Title>Public key for {this.state.alias && this.state.alias.alias}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <p>{this.state.alias && this.state.alias.publicKey}</p>
                    {this.state.publicKeyCopied && <Label style={{ marginLeft: "5px" }} bsStyle="success">Public key was copied</Label>}
                </Modal.Body>
                <Modal.Footer>
                    <CopyToClipboard text={this.state.alias && this.state.alias.publicKey}
                        onCopy={() => this.setState({ publicKeyCopied: true })}>
                        <Button bsStyle="primary">Copy</Button>
                    </CopyToClipboard>
                    <Button onClick={this.handleCloseAliasModal.bind(this)}>Close</Button>
                </Modal.Footer>
            </Modal>
            );
        return (
            <div>
                <Row>
                    <h3>Aliases</h3>
                    <Col md={2}>
                        <AddAliasForm onSubmitHandler={this.addAlias.bind(this)} />
                    </Col>
                    <Col md={10}>
                        {table}
                    </Col>
                </Row>
                {modal}
            </div>
        )
    }
}

export default connect(
    mapStateToProps,
    mapDispatchToProps
)(AliasesContainer);


