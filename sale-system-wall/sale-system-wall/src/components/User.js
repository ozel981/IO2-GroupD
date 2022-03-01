import React from 'react'
import { connect } from 'react-redux'
import { Tooltip, IconButton } from '@material-ui/core'
import DeleteForeverIcon from '@material-ui/icons/DeleteForever'
import CheckBoxIcon from '@material-ui/icons/CheckBox'
import IndeterminateCheckBoxIcon from '@material-ui/icons/IndeterminateCheckBox'
import { red, green } from '@material-ui/core/colors'
import { putUserFetch, deleteUserFetch } from '../apiMethods'
import { isResponseProper, showResponseSnackbarError, showResponseSnackbarSuccess, showResponseSnackbarWarning } from './response/responseHandler'

const User = (props) => {
  const activeUser = async (active) => {
    const body = JSON.stringify({
      isVerified: props.user.isVerified,
      isEntrepreneur: props.user.isEntrepreneur,
      isAdmin: props.user.isAdmin,
      isActive: active,
      userName: props.user.userName,
      userEmail: props.user.userEmail
    })
    const putUserFetchResponse = await putUserFetch(props.user.id, body, props.authId)

    if (isResponseProper(putUserFetchResponse)) {
      props.setUserFlag(!props.userFlag)
      showResponseSnackbarSuccess('Changes saved!', props.snackbarRef)
    } else {
      showResponseSnackbarError(putUserFetchResponse, props.snackbarRef)
    }
  }

  const verifyUser = async (verify) => {
    console.log(props.user.isEntrepreneur)
    if (props.user.isEntrepreneur === true) {
      const body = JSON.stringify({
        isVerified: verify,
        isEntrepreneur: props.user.isEntrepreneur,
        isAdmin: props.user.isAdmin,
        isActive: props.user.isAdmin,
        userName: props.user.userName,
        userEmail: props.user.userEmail
      })
      const putUserFetchResponse = await putUserFetch(props.user.id, body, props.authId)

      if (isResponseProper(putUserFetchResponse)) {
        props.setUserFlag(!props.userFlag)
        showResponseSnackbarSuccess('Changes saved!', props.snackbarRef)
      } else {
        showResponseSnackbarError(putUserFetchResponse, props.snackbarRef)
      }
    } else {
      showResponseSnackbarWarning('User is not entrepreneur!', props.snackbarRef)
    }
  }

  const deleteUser = async () => {
    const deleteUserFetchResponse = await deleteUserFetch(props.user.id, props.authId)
    if (isResponseProper(deleteUserFetchResponse)) {
      props.setUserFlag(!props.userFlag)
      showResponseSnackbarSuccess('Account deleted!', props.snackbarRef)
    } else {
      showResponseSnackbarError(deleteUserFetchResponse, props.snackbarRef)
    }
  }

  return (
    <div id="user-div">
      <div id={'user-' + props.user.id + '-div'} className="container border border-light rounded-lg" style={{ backgroundColor: '#f2f5f7', padding: '10px' }}>

        <div className="row no-gutters" align="center">
            <h5 id={'user-' + props.user.id + '-name'} >{props.user.userName}</h5>
            <h5 id={'user-' + props.user.id + '-email'} >{props.user.userEmail}</h5>
        </div>

        <hr style={{ margin: 0 }} />

        <div className="row no-gutters">
        <div className="col-3" align="center">
            IsActive:
            {props.user.isActive
              ? <Tooltip title="Deactivate">
                <IconButton id={'user-' + props.user.id + '-disactive-button'} size="small" aria-label="Deactivate" onClick={() => activeUser(false) }>
                  <CheckBoxIcon style={{ color: green[500] }}/>
                </IconButton>
              </Tooltip>
              : <Tooltip title="Activate">
                <IconButton id={'user-' + props.user.id + '-active-button'} size="small" aria-label="Activate" onClick={() => activeUser(true) }>
                  <IndeterminateCheckBoxIcon style={{ color: red[500] }} />
                </IconButton>
              </Tooltip>}
          </div>

          <div className="col-3" align="center">
            IsEntrepreneur:
            {props.user.isEntrepreneur
              ? <CheckBoxIcon style={{ color: green[500] }} />
              : <IndeterminateCheckBoxIcon style={{ color: red[500] }} /> }
          </div>

          <div className="col-3" align="center">
            IsVerified:
            {props.user.isVerified
              ? <Tooltip title="Unverify">
                <IconButton id={'user-' + props.user.id + '-unverify-button'} size="small" aria-label="Unverify" onClick={() => verifyUser(false)}>
                  <CheckBoxIcon style={{ color: green[500] }}/>
                </IconButton>
              </Tooltip>
              : <Tooltip title="Verify">
                <IconButton id={'user-' + props.user.id + '-verify-button'} size="small" aria-label="Verify" onClick={() => verifyUser(true)}>
                  <IndeterminateCheckBoxIcon style={{ color: red[500] }} />
                </IconButton>
              </Tooltip>}
          </div>

          <div className="col-3" align="center">
            Delete:
            <Tooltip title="Delete">
              <IconButton id={'user-' + props.user.id + '-unverify-button'} size="small" aria-label="Delete" onClick={() => deleteUser()}>
                <DeleteForeverIcon style={{ color: red[500] }}/>
              </IconButton>
            </Tooltip>
          </div>
        </div>
      </div>
    </div >
  )
}

const mapStateToProps = (state) => {
  return {
    authId: state.authId,
    snackbarRef: state.snackbarRef
  }
}

export default connect(mapStateToProps)(User)
