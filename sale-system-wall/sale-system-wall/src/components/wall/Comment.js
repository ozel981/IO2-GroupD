import React, { useState, useEffect } from 'react'
import { deleteCommentFetch, putLikeStatusCommentFetch, putCommentFetch } from '../../apiMethods'
import TextareaAutosize from 'react-textarea-autosize'
import { connect } from 'react-redux'
import { Tooltip, IconButton } from '@material-ui/core'
import FavoriteBorderIcon from '@material-ui/icons/FavoriteBorder'
import FavoriteIcon from '@material-ui/icons/Favorite'
import EditIcon from '@material-ui/icons/Edit'
import DeleteForeverIcon from '@material-ui/icons/DeleteForever'
import SaveIcon from '@material-ui/icons/Save'
import HighlightOffIcon from '@material-ui/icons/HighlightOff'
import { isResponseProper, showResponseSnackbarError, showResponseSnackbarSuccess } from '../response/responseHandler'

const Comment = (props) => {
  const [isEdited, setIsEdited] = useState(false)
  const [content, setContent] = useState('')

  useEffect(() => {
    setStatesDefault()
  }, [props.notificationFlag])

  return (
    <div id="comment-div" style={{ fontSize: 11 }}>
      <div id={'comment-' + props.commentId + '-div'} className="container border border-light rounded-lg" style={{ backgroundColor: '#f5f9fc', padding: '10px' }}>
        <div className="row no-gutters">
          <div className="col-3">
            posted by <label id={'comment-' + props.commentId + '-author'} style={{ color: 'red' }}>{props.authorName}</label>
          </div>
          <div className="col-3">
          </div>
          <div className="col-3">
            <label id={'comment-' + props.commentId + '-date'} style={{ color: 'blue' }}>{(new Date(Date.parse(props.date))).toLocaleString()}</label>
          </div>
          {props.user.isActive &&
          <div className="col-1">
            {!props.isLikedByUser
              ? <Tooltip title="Like it!">
                <IconButton id={'comment-' + props.commentId + '-like-button'} color="inherit" size="small" aria-label="Like"
                  onClick={likeComment}>
                  <FavoriteBorderIcon /> {props.likesCount}
                </IconButton>
              </Tooltip>
              : <Tooltip title="Dislike it...">
                <IconButton id={'comment-' + props.commentId + '-unlike-button'} color="secondary" size="small" aria-label="Dislike"
                  onClick={likeComment}>
                  <FavoriteIcon /> {props.likesCount}
                </IconButton>
              </Tooltip>}
          </div>
          }

          {props.user.isActive &&
          <div className="col-1">
            {props.ownerMode
              ? <Tooltip title="Edit comment">
                <IconButton id={'comment-' + props.commentId + '-edit-button'} color="primary" size="small" aria-label="Edit comment"
                  onClick={() => { setIsEdited(!isEdited); setContent(props.content) }}>
                  <EditIcon />
                </IconButton>
              </Tooltip>
              : ''}
          </div>
          }

          {props.user.isActive &&
          <div className="col-1">
            {props.ownerMode
              ? <Tooltip title="Delete comment">
                <IconButton id={'comment-' + props.commentId + '-delete-button'} color="secondary" size="small" aria-label="Delete comment"
                  onClick={deleteComment}>
                  <DeleteForeverIcon />
                </IconButton>
              </Tooltip>
              : ''}
          </div>
          }
        </div>
        {isEdited
          ? <div className="row no-gutters">
            <div className="col-9" style={{ fontSize: 12 }}>
              <div className="row no-gutters">
                <TextareaAutosize className="rounded"
                  minRows="1"
                  maxLength="512" value={content}
                  onChange={e => setContent(e.target.value)}
                  id={'comment-' + props.commentId + '-edit-content-input'}
                  style={{ resize: 'none' }}
                />
              </div>
            </div>
            <div className="col-1">
            </div>
            <div className="col-1">
              <Tooltip title="Save changes">
                <IconButton id={'comment-' + props.commentId + '-edit-accept-button'} color="primary" size="small" aria-label="Save"
                  onClick={updateComment}>
                  <SaveIcon />
                </IconButton>
              </Tooltip>
            </div>
            <div className="col-1">
              <Tooltip title="Reject changes">
                <IconButton id={'comment-' + props.commentId + '-edit-reject-button'} color="secondary" size="small" aria-label="Reject"
                  onClick={() => setIsEdited(false)}>
                  <HighlightOffIcon />
                </IconButton>
              </Tooltip>
            </div>
          </div>
          : <div className="row no-gutters" style={{ fontSize: 15 }}>
            <span id={'comment-' + props.commentId + '-content'} style={{ whiteSpace: 'pre-wrap' }}>{props.content}</span>
          </div>}
      </div>
      <br />
    </div>
  )

  function setStatesDefault () {
    setContent('')
    setIsEdited(false)
  }

  async function updateComment () {
    const body = JSON.stringify({
      content: content
    })
    const putCommentFetchResponse = await putCommentFetch(props.commentId, body, props.authId)
    if (!isResponseProper(putCommentFetchResponse)) {
      showResponseSnackbarError(putCommentFetchResponse, props.snackbarRef)
    }
    props.setNotificationFlag(!props.notificationFlag)
  }

  async function deleteComment () {
    const deleteCommentFetchResponse = await deleteCommentFetch(props.commentId, props.authId)
    if (isResponseProper(deleteCommentFetchResponse)) {
      showResponseSnackbarSuccess('Comment deleted!', props.snackbarRef)
    } else {
      showResponseSnackbarError(deleteCommentFetchResponse, props.snackbarRef)
    }
    props.setNotificationFlag(!props.notificationFlag)
  }

  async function likeComment () {
    const body = JSON.stringify({
      like: !props.isLikedByUser
    })

    const putLikeStatusCommentFetchResponse = await putLikeStatusCommentFetch(props.commentId, body, props.authId)
    if (!isResponseProper(putLikeStatusCommentFetchResponse)) {
      showResponseSnackbarError(putLikeStatusCommentFetchResponse, props.snackbarRef)
    }
    props.setNotificationFlag(!props.notificationFlag)
  }
}

const mapStateToProps = (state) => {
  return {
    authId: state.authId,
    snackbarRef: state.snackbarRef,
    user: state.user
  }
}

export default connect(mapStateToProps)(Comment)
