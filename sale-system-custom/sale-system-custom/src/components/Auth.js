import React, { useState } from 'react'
import TextField from '@material-ui/core/TextField'
import Button from '@material-ui/core/Button'
import { useHistory } from 'react-router-dom'
import { connect } from 'react-redux'
import { getUserFetch } from '../apiMethods'
import { isResponseProper, showResponseSnackbarError } from './response/responseHandler'

const Auth = (props) => {
  const history = useHistory()
  const [userId, setUserId] = useState('')

  const signIn = async () => {
    const result = await getUser()
    if (result) {
      props.setAuthId(userId)
      history.push('/wall')
    }
  }

  async function getUser () {
    const getUserFetchResponse = await getUserFetch(userId)
    if (isResponseProper(getUserFetchResponse)) {
      const userFetched = await getUserFetchResponse.json()
      props.setUser(userFetched)
      return true
    } else {
      showResponseSnackbarError(getUserFetchResponse, props.snackbarRef)
      return false
    }
  }

  return (
    <div>
      <div className="row">
        <div className="col-5">
        </div>
        <div className="col-2">
          <div className="row">
            <TextField id="log-in-input-userID" onChange={(event) => setUserId(event.target.value)} label="User ID" variant="outlined" />
          </div>
          <div className="row">
            <Button id="log-in-button" onClick={async () => await signIn()} variant="contained" size="large" color="primary">
              Sign in
            </Button>
          </div>
        </div>
        <div className="col-5">
        </div>
      </div>
    </div>
  )
}

const mapDispatchToProps = (dispatch) => {
  return {
    setAuthId: (authIdToSet) => dispatch({ type: 'SET_AUTH_ID', authId: authIdToSet }),
    setUser: (user) => dispatch({ type: 'SET_USER', user: user })
  }
}

const mapStateToProps = (state) => {
  return {
    authId: state.authId,
    snackbarRef: state.snackbarRef,
    user: state.user
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(Auth)
