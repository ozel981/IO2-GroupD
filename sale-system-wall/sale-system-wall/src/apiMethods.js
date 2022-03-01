const baseURL = process.env.REACT_APP_API_SERVER || 'https://salesystemapi.azurewebsites.net/'
const GET_CATEGORIES = baseURL + 'categories'
const GET_USER = baseURL + 'users/'
const PUT_USER = baseURL + 'users/'
const DELETE_USER = baseURL + 'users/'
const GET_POSTS = baseURL + 'posts'
const GET_POST = baseURL + 'post/'
const GET_POSTS_FILTERED = baseURL + 'posts/filtered'
const POST_POST = baseURL + 'post'
const DELETE_POST = baseURL + 'post/'
const PUT_POST = baseURL + 'post/'
const POST_COMMENT = baseURL + 'comment'
const DELETE_COMMENT = baseURL + 'comment/'
const PUT_COMMENT = baseURL + 'comment/'
const COMMENTS = '/comments'
const LIKE = '/likedUsers'
const LIKED = '/likedUsers'
const USERS = 'users'

export const getCategoriesFetch = async () => {
  return await fetch(GET_CATEGORIES)
}

export const getPostsFetch = async (authId) => {
  const requestOptions = {
    headers: {
      'Content-Type': 'application/json',
      userID: authId
    }
  }

  return await fetch(GET_POSTS, requestOptions)
}

export const getPostsFilteredFetch = async (authId, categoriesIDs) => {
  const requestOptions = {
    headers: {
      'Content-Type': 'application/json',
      userID: authId,
      categoriesIDs: categoriesIDs
    }
  }

  return await fetch(GET_POSTS_FILTERED, requestOptions)
}

export const postPostFetch = (body, authId) => {
  const requestOptions = {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      userID: authId
    },
    body: body
  }

  return fetch(POST_POST, requestOptions)
}

export const putPostFetch = (postId, body, authId) => {
  const requestOptions = {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      userID: authId
    },
    body: body
  }

  return fetch(PUT_POST + postId, requestOptions)
}

export const putLikeStatusPostFetch = (postId, body, authId) => {
  const requestOptions = {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      userID: authId
    },
    body: body
  }

  return fetch(PUT_POST + postId + LIKE, requestOptions)
}

export const deletePostFetch = async (postID, authID) => {
  const myHeaders = new Headers()
  myHeaders.append('userID', authID)
  myHeaders.append('Content-Type', 'application/json')

  const date = new Date()
  const isoDateTime = new Date(date.getTime() - (date.getTimezoneOffset() * 60000)).toISOString()
  const content = JSON.stringify({
    deletionDateTime: isoDateTime
  })

  const requestOptions = {
    method: 'DELETE',
    headers: myHeaders,
    body: content
  }

  return fetch(DELETE_POST + postID, requestOptions)
}

export const getPostCommentsFetch = async (postId, authId) => {
  const requestOptions = {
    headers: {
      'Content-Type': 'application/json',
      userID: authId
    }
  }
  return await fetch(GET_POST + postId + COMMENTS, requestOptions)
}

export const postCommentFetch = (body, authId) => {
  const requestOptions = {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      userID: authId
    },
    body: body
  }

  return fetch(POST_COMMENT, requestOptions)
}

export const putCommentFetch = (commentId, body, authId) => {
  const requestOptions = {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      userID: authId
    },
    body: body
  }

  return fetch(PUT_COMMENT + commentId, requestOptions)
}

export const putLikeStatusCommentFetch = (commentId, body, authId) => {
  const requestOptions = {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      userID: authId
    },
    body: body
  }

  return fetch(PUT_COMMENT + commentId + LIKED, requestOptions)
}

export const deleteCommentFetch = (commentId, authId) => {
  const requestOptions = {
    method: 'DELETE',
    headers: {
      'Content-Type': 'application/json',
      userID: authId
    }
  }

  return fetch(DELETE_COMMENT + commentId, requestOptions)
}

export const postUserFetch = async (body) => {
  const raw = JSON.stringify(body)

  const requestOptions = {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      userID: 1000
    },
    body: raw
  }

  return await fetch(baseURL + USERS, requestOptions)
}

export const getUserFetch = async (userId) => {
  return await fetch(GET_USER + userId)
}

export const getUsersFetch = async () => {
  return await fetch(baseURL + USERS)
}

export const putUserFetch = async (userId, body, authId) => {
  const requestOptions = {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      userID: authId
    },
    body: body
  }

  return await fetch(PUT_USER + userId, requestOptions)
}

export const deleteUserFetch = (userId, authId) => {
  const requestOptions = {
    method: 'DELETE',
    headers: {
      'Content-Type': 'application/json',
      userID: authId
    }
  }

  return fetch(DELETE_USER + userId, requestOptions)
}
