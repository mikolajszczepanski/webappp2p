import React from 'react'
import PropTypes from 'prop-types'
import { Field, reduxForm } from 'redux-form'
import { ControlLabel, FormControl, FormGroup, HelpBlock, Button, Alert } from "react-bootstrap";
import { LinkContainer } from "react-router-bootstrap"

import Input from './Input'

let LoginForm = (props) => {
    const { handleSubmit, onSubmitHandler, submitting, error } = props;
    return (
        <form onSubmit={handleSubmit(onSubmitHandler)}>
            {!!error && <Alert bsStyle="danger"><strong>{error}</strong></Alert>}
            <Field component={Input} name="publicKey" label="Public key" />
            <Field component={Input} type="password" name="privateKey" label="Private key (secret)" />
            <Button type="submit" bsStyle="primary" disabled={submitting}>Login</Button>
        </form>
    );
}

LoginForm.propTypes = {
    onSubmitHandler: PropTypes.PropTypes.func.isRequired
};

LoginForm = reduxForm({
    form: "enterkeys"
})(LoginForm);

export default LoginForm;