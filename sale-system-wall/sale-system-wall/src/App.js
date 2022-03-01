import React, { Component } from 'react'
import { Route } from 'react-router'

import Layout from './components/Layout'
import { Home } from './components/Home'
import Wall from './components/Wall'
import Auth from './components/Auth'
import SignUp from './components/SignUp'

import './custom.css'

import { createStore } from 'redux'
import { Provider } from 'react-redux'
import { composeWithDevTools } from 'redux-devtools-extension'
import reducer from './reducer'
import AdminPanel from './components/AdminPanel'

const store = createStore(reducer, { }, composeWithDevTools())

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Provider store={store}>
        <Layout>
          <Route exact path='/' component={Auth} />
          <Route path='/home' component={Home} />
          <Route path='/wall' component={Wall} />
          <Route path='/signup' component={SignUp} />
          <Route path='/admin' component={AdminPanel} />
        </Layout>
      </Provider>
    )
  }
}
