import React, { useState, useEffect } from 'react'
import { connect } from 'react-redux'
import TextField from '@material-ui/core/TextField'
import Select from '@material-ui/core/Select'
import MenuItem from '@material-ui/core/MenuItem'
import InputLabel from '@material-ui/core/InputLabel'
import Button from '@material-ui/core/Button'
import { putUserFetch, getUserFetch, deleteUserFetch } from '../apiMethods'
import { useHistory } from 'react-router-dom'
import DeleteUserDialog from './user/DeleteUserDialog'
import { isResponseProper, showResponseSnackbarError, showResponseSnackbarSuccess } from './response/responseHandler'

const MyAccount = (props) => {
  const [user, setUser] = useState()
  const [isLoading, setIsLoading] = useState(true)

  const history = useHistory()

  useEffect(() => {
    if (!isUserLogged()) { return }
    getUser()
  }, [])

  const isUserLogged = () => {
    if (typeof props.authId === 'undefined') {
      history.push('/')
      return false
    }
    return true
  }

  return (
    <div id="my-account-tab">
      <div className="row">
        <div className="col-5">
        </div>
        <div className="col-2">
          {isLoading
            ? <p>Loading...</p>
            : <>
              <div className="row text-center">
                <h2>Edit your profile</h2>
                <hr />
                {user.isActive && <p className="m-0">Your account is active!</p>}
                {!user.isActive && <p className="m-0">Your account is not active!</p>}
                {user.isVerified && <p className="m-0">Your account is verified!</p>}
                {!user.isVerified && <p className="m-0">Your account is verified!</p>}
              </div> <br />
              <div className="row">
                <TextField
                  id="username-textfield"
                  label="Username"
                  value={user.userName}
                  onChange={(event) => setUser({ ...user, userName: event.target.value })}
                />
              </div> <br />
              <div className="row">
                <TextField
                  id="email-textfield"
                  label="Email address"
                  value={user.userEmail}
                  onChange={(event) => setUser({ ...user, userEmail: event.target.value })}
                />
              </div> <br />
              <div className="row">
                <InputLabel shrink id="accountType">Account type</InputLabel>
                <Select
                  id="accountType-select"
                  labelId="accountType"
                  value={user.isEntrepreneur ? 1 : 0}
                  onChange={(event) => setUser({ ...user, isEntrepreneur: Boolean(event.target.value) })}
                >
                  <MenuItem id="type-normal" value={0}>Normal</MenuItem>
                  <MenuItem id="type-entrepreneur" value={1}>Entrepreneur</MenuItem>
                </Select>
              </div> <br /> <br />
              <div className="row">
                <div className="col-6">
                  <DeleteUserDialog deleteAccount={deleteUser} />
                </div>
                <div className="col-6">
                  <Button id="save-changes" onClick={updateUser} variant="contained" color="primary">
                    Save Changes
                  </Button>
                </div>
              </div>
            </>}
        </div>
      </div>
    </div>
  )

  async function updateUser () {
    const body = JSON.stringify({
      isVerified: user.isVerified,
      isEntrepreneur: user.isEntrepreneur,
      isAdmin: user.isAdmin,
      isActive: user.isActive,
      userName: user.userName,
      userEmail: user.userEmail
    })
    const putUserFetchResponse = await putUserFetch(user.id, body, props.authId)
    if (isResponseProper(putUserFetchResponse)) {
      showResponseSnackbarSuccess('Changes saved!', props.snackbarRef)
    } else {
      showResponseSnackbarError(putUserFetchResponse, props.snackbarRef)
    }
  }

  async function deleteUser () {
    const deleteUserFetchResponse = await deleteUserFetch(user.id, props.authId)
    if (isResponseProper(deleteUserFetchResponse)) {
      showResponseSnackbarSuccess('Account deleted!', props.snackbarRef)
    } else {
      showResponseSnackbarError(deleteUserFetchResponse, props.snackbarRef)
    }
  }

  async function getUser () {
    setIsLoading(true)
    const getUserFetchResponse = await getUserFetch(props.authId)
    if (isResponseProper(getUserFetchResponse)) {
      const userFetched = await getUserFetchResponse.json()
      setUser(userFetched)
      setIsLoading(false)
    } else {
      showResponseSnackbarError(getUserFetchResponse, props.snackbarRef)
    }
  }
}

const mapStateToProps = (state) => {
  return {
    authId: state.authId,
    snackbarRef: state.snackbarRef
  }
}

export default connect(mapStateToProps)(MyAccount)
