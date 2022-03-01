import React from 'react'
import Button from '@material-ui/core/Button'
import Dialog from '@material-ui/core/Dialog'
import DialogActions from '@material-ui/core/DialogActions'
import DialogContent from '@material-ui/core/DialogContent'
import DialogContentText from '@material-ui/core/DialogContentText'
import DialogTitle from '@material-ui/core/DialogTitle'

export default function DeleteUserDialog (props) {
  const [open, setOpen] = React.useState(false)

  const handleClickOpen = () => {
    setOpen(true)
  }

  const handleClose = () => {
    setOpen(false)
  }

  const handleConfirm = () => {
    setOpen(false)
    props.deleteAccount()
  }

  return (
    <div>
      <Button id="delete-account" onClick={handleClickOpen} variant="contained" color="secondary">
        Delete account
        </Button>
      <Dialog
        open={open}
        onClose={handleClose}
        aria-labelledby="alert-dialog-title"
        aria-describedby="alert-dialog-description"
      >
        <DialogTitle id="alert-dialog-title">{'Are you sure you want to delete your account?'}</DialogTitle>
        <DialogContent>
          <DialogContentText id="alert-dialog-description">
            This action causes irreversible changes! All your posts and comments will be removed!
            </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button id="undo-delete" onClick={handleClose} color="primary">
            Undo
            </Button>
          <Button id="confirm-delete" onClick={handleConfirm} color="primary" autoFocus>
            Confirm
            </Button>
        </DialogActions>
      </Dialog>
    </div>
  )
}
