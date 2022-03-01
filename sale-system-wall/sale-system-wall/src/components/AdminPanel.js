import React, { useState, useEffect } from 'react'
import User from './User'
import { useHistory } from 'react-router-dom'
import { connect } from 'react-redux'
import { isResponseProper, showResponseSnackbarError } from './response/responseHandler'
import { getUsersFetch } from '../apiMethods'

const AdminPanel = (props) => {
  const [users, setUsers] = useState([])
  const [loadingUsers, setLoadingUsers] = useState(true)
  const [userFlag, setUserFlag] = useState(true)

  const history = useHistory()

  useEffect(() => {
    if (!isUserAdmin()) { return }
    getUsers()
  }, [userFlag])

  const isUserAdmin = () => {
    if (typeof props.authId === 'undefined' || !props.user.isAdmin) {
      history.push('/')
      return false
    }
    return true
  }

  return (
    <div>
      <div className="row">
        <div className="col-1">
        </div>
        <div id="mainDiv" className="col-10">
          <h3 align="center">Users in Sale System</h3>
          {loadingUsers
            ? <label>Loading...</label>
            : users.map(u => <User key={u.id} user={u} authId={props.authId} userFlag={userFlag} setUserFlag={setUserFlag} />)}
          {users.length === 0 && !loadingUsers
            ? <label>No users found ...</label>
            : ''}
        </div>
        <div className="col-1">
        </div>
      </div>
    </div>
  )

  async function getUsers () {
    const getUsersFetchResponse = await getUsersFetch()
    if (isResponseProper(getUsersFetchResponse)) {
      const usersFetched = await getUsersFetchResponse.json()
      setUsers(usersFetched)
      setLoadingUsers(false)
    } else {
      showResponseSnackbarError(getUsersFetchResponse, props.snackbarRef)
    }
  }
}

const mapStateToProps = (state) => {
  return {
    authId: state.authId,
    snackbarRef: state.snackbarRef,
    user: state.user
  }
}

export default connect(mapStateToProps)(AdminPanel)
