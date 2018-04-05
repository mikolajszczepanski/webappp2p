import React from "react";
import { connect } from "react-redux";
import { ControlLabel, FormControl, FormGroup, HelpBlock, Button, Alert, Label } from "react-bootstrap";
import { SubmissionError } from 'redux-form'
import { CopyToClipboard } from 'react-copy-to-clipboard';
import FileSaver from 'file-saver'

import GenerateKeysForm from '../components/GenerateKeysForm'
import { fetchToGenerateKeys } from '../actions/keys'

const mapStateToProps = (state) => {
    return {

    };
}

const mapDispatchToProps = (dispatch) => {
    return {
        fetchToGenerateKeys: (ms) => dispatch(fetchToGenerateKeys())
    };
}

export class GenerateKeysContainer extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            publicKeyCopied: false,
            privateKeyCopied: false
        };
    }

    componentDidMount() {
        document.title = "Generate New Keys";
    }

    submit() {
        return this.props.fetchToGenerateKeys()
            .then(response => {
                this.setState({
                    keys: {
                        public: response.payload.publicKey,
                        private: response.payload.privateKey
                    }
                });
            })
            .catch(er => {
                throw new SubmissionError({
                    _error: er.message
                });
            });
    }

    handleDownloadKeys() {
        var data = "Public key:\n" + this.state.keys.public + "\nPrivate key(secret):\n" + this.state.keys.private + "\n";
        var blob = new Blob([data], { type: "text/plain;charset=utf-8" });
        FileSaver.saveAs(blob, "data.txt");
    }

    render() {
        if (this.state.keys !== undefined) {
            return (
                <div>
                    <FormGroup>
                        <ControlLabel>Public Key</ControlLabel>
                        <FormControl
                            componentClass="textarea"
                            value={this.state.keys.public}
                            readOnly={true}
                            cols="160"
                            rows="4"
                            id="publicKey"
                        />
                        <CopyToClipboard text={this.state.keys.public}
                            onCopy={() => this.setState({ publicKeyCopied: true, privateKeyCopied: false })}>
                            <Button bsStyle="primary">Copy</Button>
                        </CopyToClipboard>
                        {this.state.publicKeyCopied && <Label style={{ marginLeft: "5px" }} bsStyle="success">Copied</Label>}
                    </FormGroup>
                    <FormGroup>
                        <ControlLabel>Private Key</ControlLabel>
                        <FormControl
                            componentClass="textarea"
                            value={this.state.keys.private}
                            readOnly={true}
                            cols="160"
                            rows="8"
                            id="publicKey"
                        />
                        <CopyToClipboard text={this.state.keys.private}
                            onCopy={() => this.setState({ publicKeyCopied: false, privateKeyCopied: true })}>
                            <Button bsStyle="primary">Copy</Button>
                        </CopyToClipboard>
                        {this.state.privateKeyCopied && <Label style={{ marginLeft: "5px" }} bsStyle="success">Copied</Label>}
                    </FormGroup>
                    <Button bsStyle="primary" onClick={this.handleDownloadKeys.bind(this)}>Download both</Button>
                </div>
            )
        }
        return (
            <div>
                <h4>Generating new private key</h4>
                <p>Click button below to generate your unique public and private keys.
                Remember to do not share your private key except the login page.</p>
                <div style={{ marginLeft: "15px" }}>
                    <GenerateKeysForm onSubmitHandler={this.submit.bind(this)} />
                </div>
            </div>
            )
    }
}

export default connect(
    mapStateToProps,
    mapDispatchToProps
)(GenerateKeysContainer);