import React from 'react'
import { connect } from 'react-redux'
import PropTypes from 'prop-types'
import { Field, reduxForm, change } from 'redux-form'
import { ControlLabel, FormControl, FormGroup, HelpBlock, Button, Alert, Glyphicon, Col, Row } from "react-bootstrap";
import { LinkContainer } from "react-router-bootstrap"

import Input from './Input'

const validate = values => {
    const errors = {}

    if (!values.to || values.to.length < 1) {
        errors.to = 'Required'
    }

    if (!values.title || values.title.length < 1) {
        errors.title = 'Required'
    }

    if (values.title !== undefined && values.title.length > 50) {
        errors.title = 'Too long'
    }

    if (!values.content || values.content.length < 1) {
        errors.content = 'Required'
    }

    if (values.content !== undefined && values.content.length > 760) {
        errors.content = 'Too long'
    }

    return errors
}


class MessageForm extends React.Component {

    handleChange(e) {
        var selectedPublicKey = e.target.value;
        this.props.change("to", selectedPublicKey);
    }

    render() {
        const { handleSubmit, onSubmitHandler, submitting, error, aliases } = this.props;

        return (
            <form onSubmit={handleSubmit(onSubmitHandler)}>
                {!!error && <Alert bsStyle="danger"><strong>{error}</strong></Alert>}
                <Row>
                    <Col md={10}>
                        <Field component={Input} name="to" label="To" />
                    </Col>
                    <Col md={2}>
                        <FormGroup>
                            <ControlLabel>Select from aliases</ControlLabel>
                            <FormControl componentClass="select" onChange={this.handleChange.bind(this)}>
                                <option>-select-</option>
                                {
                                    aliases.map(a => {
                                        return (
                                            <option value={a.publicKey}>{a.alias}</option>
                                        );
                                    })
                                }
                            </FormControl>
                        </FormGroup>
                    </Col>
                </Row>

                <Field component={Input} name="title" label="Title" />
                <Field component={Input} name="content" label="Content" componentClass="textarea" id="composeContent" />

                <Button type="submit" bsStyle="primary" disabled={submitting}>
                    <span style={{
                        verticalAlign: "top"
                    }}>
                        <Glyphicon style={{
                            fontSize: 16 + "px",
                            marginTop: -4 + "px",
                            color: "#FFFFFF",
                            paddingRight: 5 + "px"
                        }} glyph="send" />
                        Send
                </span>
                </Button>
            </form>
        );
    }
}

MessageForm.propTypes = {
    onSubmitHandler: PropTypes.PropTypes.func.isRequired
};

MessageForm = reduxForm({
    form: "message",
    validate
})(MessageForm);

MessageForm = connect(
    state => (
        {
            initialValues: {
                to: state.messagesReducer.current !== undefined ? state.messagesReducer.current.from : '',
                title: state.messagesReducer.current !== undefined ? 'Re: ' + state.messagesReducer.current.title : ''
            }
        }
    )
)(MessageForm)


export default MessageForm;