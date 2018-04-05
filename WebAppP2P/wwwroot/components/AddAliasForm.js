import React from 'react'
import PropTypes from 'prop-types'
import { Field, reduxForm } from 'redux-form'
import { ControlLabel, FormControl, FormGroup, HelpBlock, Button, Alert } from "react-bootstrap";

import Input from './Input'

const validate = values => {
    const errors = {}

    if (!values.alias || values.alias.length < 1) {
        errors.alias = 'Required'
    }

    if (!values.publicKey || values.publicKey.length < 1) {
        errors.publicKey = 'Required'
    }

    return errors
}


let AddAliasForm = (props) => {
    const { handleSubmit, onSubmitHandler, submitting, error } = props;
    return (
        <form onSubmit={handleSubmit(onSubmitHandler)}>
            {!!error && <Alert bsStyle="danger"><strong>{error}</strong></Alert>}
            <Field component={Input} name="alias" label="Alias" />
            <Field component={Input} name="publicKey" label="Public Key" />
            <Button type="submit" bsStyle="primary" disabled={submitting}>Add</Button>
        </form>
    );
}

AddAliasForm.propTypes = {
    onSubmitHandler: PropTypes.PropTypes.func.isRequired
};

AddAliasForm = reduxForm({
    form: "addalias",
    validate
})(AddAliasForm);

export default AddAliasForm;