import React, { useState, useEffect } from 'react'
import NewPost from './wall/NewPost'
import Post from './wall/Post'
import CategoriesWall from './wall/CategoriesWall'
import { getCategoriesFetch, getPostsFetch, getPostsFilteredFetch } from './../apiMethods'
import { useHistory } from 'react-router-dom'
import { connect } from 'react-redux'
import { isResponseProper, showResponseSnackbarError } from './response/responseHandler'

const Wall = (props) => {
  const [posts, setPosts] = useState([])
  const [loadingPosts, setLoadingPosts] = useState(true)
  const [newPostFlag, setNewPostFlag] = useState(false)
  const [searchFlag, setSearchFlag] = useState(false)
  const [postsEditFlag, setPostsEditFlag] = useState(false)
  const [categories, setCategories] = useState([])
  const [selectedCategories, setSelectedCategories] = useState([])
  const [loadingCategories, setLoadingCategories] = useState(true)

  const history = useHistory()

  useEffect(() => {
    if (!isUserLogged()) { return }
    getCategories()
  }, [])

  useEffect(() => {
    if (!isUserLogged()) { return }
    getPosts()
  }, [newPostFlag, searchFlag, postsEditFlag])

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
    <div>
      <div className="row">
        <div className="col-1">
        </div>
        <div id="mainDiv" className="col-7">
          <h1>Wall</h1>
          {props.user.isActive &&
          <div>
            <h4>Add your post</h4>
            <NewPost notificationFlag={newPostFlag} setNotificationFlag={setNewPostFlag} />
            <br />
            <h3>Posts</h3>
          </div>
          }

          {loadingPosts
            ? <label>Loading...</label>
            : posts.map(p => <Post key={p.id} post={p}
              notificationFlag={postsEditFlag} setNotificationFlag={setPostsEditFlag} />)}
          {posts.length === 0 && !loadingPosts
            ? <label>No posts found...</label>
            : ''}
        </div>
        <div className="col-1">
        </div>
        <div id="filtersDiv" className="col-2">
          <h3>Categories</h3>
          {
            loadingCategories
              ? <label>Loading...</label>
              : <CategoriesWall categories={categories} selected={selectedCategories} setSelected={setSelectedCategories} search={searchFlag} setSearch={setSearchFlag} />
          }
        </div>
        <div className="col-1">
        </div>
      </div>
    </div>
  )

  async function getPosts () {
    const getPostsFetchResponse =
      categories.length === selectedCategories.length
        ? await getPostsFetch(props.authId)
        : await getPostsFilteredFetch(props.authId, selectedCategories)
    if (isResponseProper(getPostsFetchResponse)) {
      const postsFetched = await getPostsFetchResponse.json()
      setPosts([...postsFetched])
      setLoadingPosts(false)
    } else {
      showResponseSnackbarError(getPostsFetchResponse, props.snackbarRef)
    }
  }

  async function getCategories () {
    const getCategoriesFetchResponse = await getCategoriesFetch()
    if (isResponseProper(getCategoriesFetchResponse)) {
      const categories = await getCategoriesFetchResponse.json()
      setCategories([...categories])
      const allIDs = []
      categories.map(category => allIDs.push(category.id))
      setSelectedCategories([...allIDs])
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
