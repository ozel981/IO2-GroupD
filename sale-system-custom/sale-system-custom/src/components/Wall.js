import React, { useState, useEffect } from 'react'
import NewPost from './wall/NewPost'
import Post from './wall/Post'
import Comment from './wall/Comment'
import SubscribeCategories from './wall/SubscribeCategories'
import { getCategoriesFetch, getMyPostsFetch, getUserCommentsFetch } from './../apiMethods'
import { useHistory } from 'react-router-dom'
import { connect } from 'react-redux'
import { isResponseProper, showResponseSnackbarError } from './response/responseHandler'
import Paper from '@material-ui/core/Paper'
import Tabs from '@material-ui/core/Tabs'
import Tab from '@material-ui/core/Tab'

const Wall = (props) => {
  const [posts, setPosts] = useState([])
  const [loadingPosts, setLoadingPosts] = useState(true)
  const [newPostFlag, setNewPostFlag] = useState(false)
  const [postsEditFlag, setPostsEditFlag] = useState(false)
  const [categories, setCategories] = useState([])
  const [loadingCategories, setLoadingCategories] = useState(true)
  const [tabNumber, setTabNumber] = React.useState(0)
  const [myComments, setMyComments] = useState([])
  const [commentNF, setCommentNF] = useState(false)

  const history = useHistory()

  useEffect(() => {
    if (!isUserLogged()) { return }
    getCategories()
  }, [])

  useEffect(() => {
    if (!isUserLogged()) { return }
    getPosts()
  }, [newPostFlag, postsEditFlag])

  useEffect(() => {
    if (!isUserLogged()) { return }
    if (tabNumber) {
      getMyComments()
    }
  }, [commentNF, tabNumber])

  const isUserLogged = () => {
    if (typeof props.authId === 'undefined' || typeof props.user === 'undefined') {
      history.push('/')
      return false
    }
    return true
  }

  if (!isUserLogged()) {
    return (
      <div></div>
    )
  }

  return (
    <div id="wall-tab">
      <div className="row">
        <div className="col-1">
        </div>
        <div id="mainDiv" className="col-7">
          <Paper>
            <Tabs
              value={tabNumber}
              onChange={(event, newValue) => setTabNumber(newValue)}
              indicatorColor="primary"
              textColor="primary"
            >
              <Tab label="My posts" />
              <Tab label="My comments" />
            </Tabs>
          </Paper> <br />
          {tabNumber
            ? myComments.length === 0
              ? <p style={{ fontSize: 12, margin: 0, paddingLeft: '2%' }}>No comments found...</p>
              : myComments.map(c => <Comment commentId={c.id} key={c.id} ownerMode={c.ownerMode} authorID={c.authorID} authorName={c.authorName} content={c.content} date={c.date} isLikedByUser={c.isLikedByUser} likesCount={c.likesCount} notificationFlag={commentNF} setNotificationFlag={setCommentNF} />)
            : <>
              {props.user.isActive &&
              <div>
                <h4>Add new post</h4>
                <NewPost notificationFlag={newPostFlag} setNotificationFlag={setNewPostFlag} />
              </div>
              }
              <br />
              <h3>Posts</h3>
              {loadingPosts
                ? <label>Loading...</label>
                : posts.map(p => <Post key={p.id} post={p}
                  notificationFlag={postsEditFlag} setNotificationFlag={setPostsEditFlag} />)}
              {posts.length === 0 && !loadingPosts
                ? <label>No posts found...</label>
                : ''}
            </>
            }
        </div>
        <div className="col-1">
        </div>
        <div className="col-2">
          <h3>Subscribed categories</h3>
          {
            loadingCategories
              ? <label>Loading...</label>
              : <SubscribeCategories categories={categories} />
          }
        </div>
        <div className="col-1">
        </div>
      </div>
    </div>
  )

  async function getPosts () {
    const getMyPostsFetchResponse = await getMyPostsFetch(props.authId)
    if (isResponseProper(getMyPostsFetchResponse)) {
      const postsFetched = await getMyPostsFetchResponse.json()
      setPosts([...postsFetched])
      setLoadingPosts(false)
    } else {
      showResponseSnackbarError(getMyPostsFetchResponse, props.snackbarRef)
    }
  }

  async function getMyComments () {
    const getUserCommentsFetchResponse = await getUserCommentsFetch(props.authId)
    if (isResponseProper(getUserCommentsFetchResponse)) {
      const commentsFetched = await getUserCommentsFetchResponse.json()
      setMyComments([...commentsFetched])
    } else {
      showResponseSnackbarError(getUserCommentsFetchResponse, props.snackbarRef)
    }
  }

  async function getCategories () {
    const getCategoriesFetchResponse = await getCategoriesFetch()
    if (isResponseProper(getCategoriesFetchResponse)) {
      const categories = await getCategoriesFetchResponse.json()
      setCategories([...categories])
      setLoadingCategories(false)
    } else {
      showResponseSnackbarError(getCategoriesFetchResponse, props.snackbarRef)
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

export default connect(mapStateToProps)(Wall)
