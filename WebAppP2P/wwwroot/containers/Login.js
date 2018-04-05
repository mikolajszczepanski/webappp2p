import React from "react";
import { connect } from "react-redux";
import { Col } from "react-bootstrap";
import { SubmissionError } from 'redux-form'

import LoginForm from "../components/LoginForm";
import { fetchToCheckKeys } from '../actions/keys'

import {
    initConnection
} from "../actions/messages"

import {
    loadAliasesFromLocalStorage,
    initAliases
} from "../actions/aliases"

const mapStateToProps = (state) => {
    return {
        publicKey: state.keysReducer.keys !== undefined && state.keysReducer.keys.public,
        privateKey: state.keysReducer.keys !== undefined && state.keysReducer.keys.private,
    };
}

const mapDispatchToProps = (dispatch,data) => {
    return {
        fetchToCheckKeys: (data) => dispatch(fetchToCheckKeys(data)),
        initConnection: (publicKey, privateKey) => dispatch(initConnection(publicKey, privateKey)),
        initAliases: (publicKey) => dispatch(initAliases(publicKey, loadAliasesFromLocalStorage(publicKey)))
    };
}

export class LoginContainer extends React.Component {

    componentDidMount() {
        document.title = "Login";     
    }

    submit(data){
        return this.props.fetchToCheckKeys(data).then(response => {
            var areKeysValid = response.payload;
            if (areKeysValid) {
                this.props.initConnection(
                    this.props.publicKey,
                    this.props.privateKey
                );
                this.props.history.push("/");
                this.props.initAliases(this.props.publicKey);
            }
            else {
                throw new SubmissionError({
                    _error: "Your public or private key is not valid"
                });
            }
        }).catch(er => {
            if (er instanceof SubmissionError) {
                throw er;
            }
            else if (er.response !== undefined) {
                if (er.response.status === 400) {
                    throw new SubmissionError({
                        _error: "Your public or private key is incorrect"
                    });
                }
            }
            console.error(er);
            throw new SubmissionError({
                _error: "Unexpected error occured"
            });
        });
    }

    render() {
        return (
            <div>
                <Col md={12}>
                    <Col md={4} mdPush={4}>
                        <LoginForm onSubmitHandler={this.submit.bind(this)} />
                    </Col>
                </Col>
            </div>
        )
    }
}

export default connect(
    mapStateToProps,
    mapDispatchToProps
)(LoginContainer);


