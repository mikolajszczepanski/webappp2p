import React from "react";
import { connect } from "react-redux";
import { ControlLabel, FormControl, FormGroup, HelpBlock, Button, Alert } from "react-bootstrap";

import { Configuration } from "../common";

const mapStateToProps = (state) => {
    return {
        userLogged: state.keysReducer.keys !== undefined && state.keysReducer.keys.valid === true,
        publicKey: state.keysReducer.keys !== undefined && state.keysReducer.keys.public
    };
}

const mapDispatchToProps = (dispatch) => {
    return {};
}

export class HomeContainer extends React.Component {
    displayName = "Home";

    componentDidMount() {
        document.title = "Home";
    }
    
    render() {
        if (this.props.userLogged) {
            return (
                <div>
                    <h3>You are logged</h3>
                    <Alert bsStyle="info">
                        <strong>Version</strong> {Configuration.version}
                    </Alert>
                </div>
            )
        }
        return (
            <div>
                <h3>Please log in</h3>
                <Alert bsStyle="info">
                    <strong>Version</strong> {Configuration.version}
                </Alert>
            </div>
        )
    }
}

export default connect(
    mapStateToProps,
    mapDispatchToProps
)(HomeContainer);


