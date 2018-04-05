import React from "react";
import ReactDOM from "react-dom";
import {
    Route,
    Switch,
    IndexRoute
} from 'react-router-dom'
import { Provider } from 'react-redux'
import { BrowserRouter } from 'react-router-dom'

import { configureStore } from '../store/configureStore'
import Main from './Main'
import Home from './Home'
import GenerateKeys from './GenerateKeys'
import Login from './Login'
import Logout from './Logout'
import Messages from './Messages'
import MessageDetails from './MessageDetails'
import Aliases from './Aliases'

let store = configureStore();

class App extends React.Component {

    render() {
        return (
            <Provider store={store}>
                <BrowserRouter>
                    <div className="container-fluid">
                        <Route component={Main} />
                        <Switch>
                            <Route exact path='/' component={Home} />
                            <Route path='/genkeys' component={GenerateKeys} />
                            <Route path='/login' component={Login} />
                            <Route path='/logout' component={Logout} />
                            <Route exact path='/messages' component={Messages} />
                            <Route path='/messages/:id' component={MessageDetails} />
                            <Route path='/aliases' component={Aliases} />
                        </Switch>
                        <br />
                        <hr />
                        <br />
                        <p>Copyright Mikołaj Szczepański</p>
                    </div>
                </BrowserRouter>
            </Provider>
        )
    }
}

export default App;