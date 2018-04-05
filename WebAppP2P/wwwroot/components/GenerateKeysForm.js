import React from 'react'
import PropTypes from 'prop-types'
import { Field, reduxForm } from 'redux-form'
import { ControlLabel, FormControl, FormGroup, HelpBlock, Button, Alert } from "react-bootstrap";

import Input from './Input'

let GenerateKeysForm = (props) => {
    const { handleSubmit, onSubmitHandler, submitting, error } = props;
    return (
        <form onSubmit={handleSubmit(onSubmitHandler)}>
            {!!error && <Alert bsStyle="danger"><strong>{error}</strong></Alert>}
            <Button type="submit" bsStyle="primary" disabled={submitting}>Generate</Button>
        </form>
    );
}

GenerateKeysForm.propTypes = {
    onSubmitHandler: PropTypes.PropTypes.func.isRequired
};

GenerateKeysForm = reduxForm({
    form: "genkeys"
})(GenerateKeysForm);

export default GenerateKeysForm;