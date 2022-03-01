export default function reducer (state, action) {
  switch (action.type) {
    case 'SET_AUTH_ID':
      return {
        ...state,
        authId: action.authId
      }
    case 'SET_USER':
      return {
        ...state,
        user: action.user
      }
    case 'SET_SNACKBAR_REF':
      return {
        ...state,
        snackbarRef: action.snackbarRef
      }
    default:
      return state
  }
}
