import React from 'react'
import Snackbar from '@material-ui/core/Snackbar'
import MuiAlert from '@material-ui/lab/Alert'
import { makeStyles } from '@material-ui/core/styles'

function Alert (props) {
  return <MuiAlert elevation={6} variant="filled" {...props} />
}

const useStyles = makeStyles((theme) => ({
  root: {
    width: '100%',
    '& > * + *': {
      marginTop: theme.spacing(2)
    }
  }
}))

export default function ResponseSnackbar ({ myRef }) {
  const classes = useStyles()
  const [open, setOpen] = React.useState(false)

  const [type, setType] = React.useState()
  const [message, setMessage] = React.useState()

  const show = (type, message) => {
    setType(type)
    setMessage(message)
    setOpen(true)
  }
  myRef.current.show = show

  const handleClose = (event, reason) => {
    if (reason === 'clickaway') {
      return
    }

    setOpen(false)
  }

  return (
    <div className={ classes.root }>
      <Snackbar open={open} autoHideDuration={5000} onClose={handleClose} anchorOrigin={{ vertical: 'bottom', horizontal: 'left' }}>
        <Alert onClose={handleClose} severity={type}>
          {message}
        </Alert>
      </Snackbar>
    </div>
  )
}

ResponseSnackbar.defaultProps = {
  myRef: {
    current: {}
  }
}
