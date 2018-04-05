import React from "react";
import { connect } from "react-redux";
import { ControlLabel, FormControl, FormGroup, HelpBlock, Button, Alert } from "react-bootstrap";
import { clearKeys } from "../actions/keys"
import {
    closeConnection,
    clearMessages
} from "../actions/messages"

import {
    clearAliases
} from "../actions/aliases"

const mapStateToProps = (state) => {
    return {};
}

const mapDispatchToProps = (dispatch) => {
    return {
        clearKeys: () => dispatch(clearKeys()),
        closeConnection: () => dispatch(closeConnection()),
        clearMessages: () => dispatch(clearMessages()),
        clearAliases: () => dispatch(clearAliases())
    };
}

export class LogoutContainer extends React.Component {
    displayName = "Logout";

    componentDidMount() {
        document.title = "Logout";
        this.props.clearKeys();
        this.props.closeConnection();
        this.props.clearMessages();
        this.props.clearAliases();
        this.props.history.push("/");
    }

    render() {
        return (<h3>You will be redirect...</h3>)
    }
}

export default connect(
    mapStateToProps,
    mapDispatchToProps
)(LogoutContainer);


