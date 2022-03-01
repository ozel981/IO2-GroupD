import React, { useRef } from 'react'
import { NavMenu } from './NavMenu'
import ResponseSnackbar from './response/ResponseSnackbar'
import { connect } from 'react-redux'

const Layout = (props) => {
  const responseSnackbarRef = useRef({})

  props.setSnackbarRef(responseSnackbarRef)

  return (
    <div>
      <NavMenu />
      {props.children}
      <ResponseSnackbar myRef={responseSnackbarRef}/>
    </div>
  )
}
const mapDispatchToProps = (dispatch) => {
  return {
    setSnackbarRef: (snackbarRefToSet) => dispatch({ type: 'SET_SNACKBAR_REF', snackbarRef: snackbarRefToSet })
  }
}

const mapStateToProps = (state) => {
  return {
    authId: state.authId,
    snackbarRef: state.snackbarRef
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(Layout)
