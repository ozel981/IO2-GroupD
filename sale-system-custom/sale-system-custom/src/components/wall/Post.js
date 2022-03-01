import React, { useEffect, useState } from 'react'
import TextareaAutosize from 'react-textarea-autosize'
import { getCategoriesFetch, postCommentFetch, deletePostFetch, putPostFetch, putLikeStatusPostFetch, promotePostFetch, getPostCommentsFetch } from '../../apiMethods'
import Comment from './Comment'
import { connect } from 'react-redux'
import { Tooltip, IconButton } from '@material-ui/core'
import FavoriteBorderIcon from '@material-ui/icons/FavoriteBorder'
import FavoriteIcon from '@material-ui/icons/Favorite'
import EditIcon from '@material-ui/icons/Edit'
import DeleteForeverIcon from '@material-ui/icons/DeleteForever'
import SaveIcon from '@material-ui/icons/Save'
import HighlightOffIcon from '@material-ui/icons/HighlightOff'
import SendIcon from '@material-ui/icons/Send'
import MonetizationOnIcon from '@material-ui/icons/MonetizationOn'
import MonetizationOnOutlinedIcon from '@material-ui/icons/MonetizationOnOutlined'
import ExpandMoreIcon from '@material-ui/icons/ExpandMore'
import ExpandLessIcon from '@material-ui/icons/ExpandLess'
import { isResponseProper, showResponseSnackbarError, showResponseSnackbarSuccess } from '../response/responseHandler'

const Post = (props) => {
  const [comments, setComments] = useState([])
  const [commentNF, setCommentNF] = useState(false)
  const [categories, setCategories] = useState([])
  const [commentsExpanded, setCommentsExpanded] = useState(false)
  const [isEdited, setIsEdited] = useState(false)
  const [title, setTitle] = useState('')
  const [content, setContent] = useState('')
  const [commentContent, setCommentContent] = useState('')

  useEffect(() => {
    setStatesDefault()
    getCategories()
  }, [props.notificationFlag, commentNF])

  useEffect(() => {
    if (commentsExpanded) { getPostComments() }
  }, [commentNF])

  return (
    <div id="post-div">
      <div id={'post-' + props.post.id + '-div'} className="container border border-light rounded-lg" style={{ backgroundColor: '#f2f5f7', padding: '10px' }}>
        <div className="row no-gutters">
          <div className="col-3" style={{ fontSize: 12 }}>
            posted by <label id={'post-' + props.post.id + '-author'} style={{ color: 'red' }}>{props.post.authorName}</label>
          </div>
          <div className="col-3" style={{ fontSize: 12 }}>
            from <label id={'post-' + props.post.id + '-category'} style={{ color: 'red' }}>{props.post.category}</label>
          </div>
          <div className="col-2" style={{ fontSize: 12 }}>
            <label id={'post-' + props.post.id + '-date'} style={{ color: 'blue' }}>{(new Date(Date.parse(props.post.dateTime))).toLocaleString()}</label>
          </div>
          {props.user.isEntrepreneur && props.user.isVerified && props.user.isActive &&
          <div className="col-1">
            {!props.post.isPromoted
              ? <Tooltip title="Promote!">
                <IconButton id={'post-' + props.post.id + '-promote-button'} color="primary" size="small" aria-label="Promote!"
                  onClick={promotePost}>
                  <MonetizationOnIcon />
                </IconButton>
              </Tooltip>
              : <Tooltip title="Unpromote!">
                <IconButton id={'post-' + props.post.id + '-unpromote-button'} color="secondary" size="small" aria-label="Unpromote!"
                  onClick={promotePost}>
                  <MonetizationOnOutlinedIcon />
                </IconButton>
              </Tooltip>}
          </div>
          }
          {props.user.isActive &&
          <div className="col-1">
            {!props.post.isLikedByUser
              ? <Tooltip title="Like it!">
                <IconButton id={'post-' + props.post.id + '-like-button'} color="inherit" size="small" aria-label="Like"
                  onClick={likePost}>
                  <FavoriteBorderIcon /> {props.post.likesCount}
                </IconButton>
              </Tooltip>
              : <Tooltip title="Dislike it...">
                <IconButton id={'post-' + props.post.id + '-unlike-button'} color="secondary" size="small" aria-label="Dislike"
                  onClick={likePost}>
                  <FavoriteIcon /> {props.post.likesCount}
                </IconButton>
              </Tooltip>}
          </div>
          }
          {props.user.isActive &&
          <div className="col-1">
            {props.post.authorID.toString() === props.authId.toString()
              ? <Tooltip title="Edit post">
                <IconButton id={'post-' + props.post.id + '-edit-button'} color="primary" size="small" aria-label="Edit post"
                  onClick={() => { setIsEdited(!isEdited); setTitle(props.post.title); setContent(props.post.content) }}>
                  <EditIcon />
                </IconButton>
              </Tooltip>
              : ''}
          </div>
          }
          <div className="col-1">
            {props.post.authorID.toString() === props.authId.toString()
              ? <Tooltip title="Delete post">
                <IconButton id={'post-' + props.post.id + '-delete-button'} color="secondary" size="small" aria-label="Delete post"
                  onClick={deletePost}>
                  <DeleteForeverIcon />
                </IconButton>
              </Tooltip>
              : ''}
          </div>
        </div >
        {!isEdited
          ? <h5 id={'post-' + props.post.id + '-title'} >{props.post.title}</h5>
          : <TextareaAutosize className="rounded"
            id={'post-' + props.post.id + '-edit-title-input'}
            maxLength="64" value={title}
            onChange={e => setTitle(e.target.value)}
            style={{ resize: 'none', width: '75%' }}
          />}
        {isEdited
          ? <div className="row no-gutters">
            <div className="col-9" style={{ fontSize: 13 }}>
              <div className="row no-gutters">
                <TextareaAutosize className="rounded"
                  minRows="1"
                  maxLength="512" value={content}
                  onChange={e => setContent(e.target.value)}
                  id={'post-' + props.post.id + '-edit-content-input'}
                  style={{ resize: 'none' }}
                />
              </div>
            </div>
            <div className="col-1">
            </div>
            <div className="col-1">
              <Tooltip title="Save changes">
                <IconButton id={'post-' + props.post.id + '-edit-accept-button'} color="primary" size="small" aria-label="Save"
                  onClick={updatePost}>
                  <SaveIcon />
                </IconButton>
              </Tooltip>
            </div>
            <div className="col-1">
              <Tooltip title="Reject changes">
                <IconButton id={'post-' + props.post.id + '-edit-reject-button'} color="secondary" size="small" aria-label="Reject"
                  onClick={() => setIsEdited(false)}>
                  <HighlightOffIcon />
                </IconButton>
              </Tooltip>
            </div>
          </div>
          : <div className="row no-gutters">
            <span id={'post-' + props.post.id + '-content'} style={{ whiteSpace: 'pre-wrap' }}>{props.post.content}</span>
          </div>}
        <br />
        <div className="row no-gutters">
          <div className="col-9">
            <div className="row no-gutters">
              <TextareaAutosize className="rounded"
                id={'post-' + props.post.id + '-comment-content-input'}
                maxLength="512" value={commentContent}
                onChange={e => setCommentContent(e.target.value)}
                style={{ resize: 'none' }}
                placeholder="Comment..."
              />
            </div>
          </div>
          <div className="col-1">
          </div>
          <div className="col-2">
            <Tooltip title="Send comment">
              <IconButton id={'post-' + props.post.id + '-comment-create-button'} color="primary" size="small" aria-label="Send comment" onClick={sendComment}>
                <SendIcon />
              </IconButton>
            </Tooltip>
          </div>
        </div>
        <div>
          {commentsExpanded
            ? <Tooltip title="Roll comments" placement="right">
              <IconButton id={'post-' + props.post.id + '-roll-button'} color="primary" size="small" aria-label="Roll" onClick={postCommentsExpandRoll}>
                <ExpandLessIcon />
              </IconButton>
            </Tooltip>
            : <Tooltip title="Expand comments" placement="right">
              <IconButton id={'post-' + props.post.id + '-expand-button'} color="primary" size="small" aria-label="Expand" onClick={postCommentsExpandRoll}>
                <ExpandMoreIcon />
              </IconButton>
            </Tooltip>}
          {commentsExpanded
            ? <div id={'comments-' + props.post.id + '-div'}> {comments.length === 0
              ? <p style={{ fontSize: 12, margin: 0, paddingLeft: '2%' }}>No comments for this post.</p>
              : comments.map(c => <Comment commentId={c.id} key={c.id} ownerMode={c.ownerMode} authorID={c.authorID} authorName={c.authorName} content={c.content} date={c.date} isLikedByUser={c.isLikedByUser} likesCount={c.likesCount} notificationFlag={commentNF} setNotificationFlag={setCommentNF} />)
               } </div>
            : ''
          }
        </div>
      </div>
      <br />
    </div >
  )

  function setStatesDefault () {
    setTitle('')
    setContent('')
    setCommentContent('')
    setIsEdited(false)
  }

  async function getCategories () {
    const getCategoriesFetchResponse = await getCategoriesFetch()
    if (isResponseProper(getCategoriesFetchResponse)) {
      const getCategoriesFetch = await getCategoriesFetchResponse.json()
      let convertedCategories = []
      getCategoriesFetch.map(category => convertedCategories = [ // eslint-disable-line no-return-assign
        ...convertedCategories,
        {
          label: category.name,
          value: category.id
        }
      ])
      setCategories(convertedCategories)
    } else {
      showResponseSnackbarError(getCategoriesFetchResponse, props.snackbarRef)
    }
  }

  async function getPostComments () {
    const getPostCommentsFetchResponse = await getPostCommentsFetch(props.post.id, props.authId)
    if (isResponseProper(getPostCommentsFetchResponse)) {
      const getPostCommentsFetch = await getPostCommentsFetchResponse.json()
      setComments([...getPostCommentsFetch])
    } else {
      showResponseSnackbarError(getPostCommentsFetchResponse, props.snackbarRef)
    }
  }

  async function sendComment () {
    if (commentContent === '') {
      alert('Content is empty. Cannot post.')
      return
    }

    const body = JSON.stringify({
      postID: props.post.id,
      content: commentContent
    })

    const postCommentFetchResponse = await postCommentFetch(body, props.authId)
    if (!isResponseProper(postCommentFetchResponse)) {
      showResponseSnackbarError(postCommentFetchResponse, props.snackbarRef)
    }
    setCommentNF(!commentNF)
  }

  async function updatePost () {
    const body = JSON.stringify({
      title: title,
      content: content,
      categoryID: categories.find(element => element.label === props.post.category).value,
      isPromoted: props.post.isPromoted
    })

    const putPostFetchResponse = await putPostFetch(props.post.id, body, props.authId)
    if (!isResponseProper(putPostFetchResponse)) {
      showResponseSnackbarError(putPostFetchResponse, props.snackbarRef)
    }
    props.setNotificationFlag(!props.notificationFlag)
  }

  async function deletePost () {
    const deletePostFetchResponse = await deletePostFetch(props.post.id, props.authId, props.setNotificationFlag, props.notificationFlag)
    if (isResponseProper(deletePostFetchResponse)) {
      showResponseSnackbarSuccess('Post deleted!', props.snackbarRef)
    } else {
      showResponseSnackbarError(deletePostFetchResponse, props.snackbarRef)
    }
    props.setNotificationFlag(!props.notificationFlag)
  }

  async function likePost () {
    const body = JSON.stringify({
      like: !props.post.isLikedByUser
    })

    const putLikeStatusPostFetchResponse = await putLikeStatusPostFetch(props.post.id, body, props.authId)
    if (!isResponseProper(putLikeStatusPostFetchResponse)) {
      showResponseSnackbarError(putLikeStatusPostFetchResponse, props.snackbarRef)
    }
    props.setNotificationFlag(!props.notificationFlag)
  }

  async function promotePost () {
    const body = JSON.stringify({
      title: props.post.title,
      content: props.post.content,
      categoryID: categories.find(element => element.label === props.post.category).value,
      isPromoted: !props.post.isPromoted
    })
    const promotePostFetchResponse = await promotePostFetch(props.post.id, body, props.authId)
    if (!isResponseProper(promotePostFetchResponse)) {
      showResponseSnackbarError(promotePostFetchResponse, props.snackbarRef)
    }
    props.setNotificationFlag(!props.notificationFlag)
  }

  async function postCommentsExpandRoll () {
    commentsExpanded ? setComments([]) : await getPostComments()
    setCommentsExpanded(!commentsExpanded)
  }
}

const mapStateToProps = (state) => {
  return {
    authId: state.authId,
    snackbarRef: state.snackbarRef,
    user: state.user
  }
}

export default connect(mapStateToProps)(Post)
