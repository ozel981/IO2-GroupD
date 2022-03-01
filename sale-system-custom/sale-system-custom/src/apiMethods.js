const baseURL = process.env.REACT_APP_API_SERVER || 'https://salesystemapi.azurewebsites.net/'
const GET_CATEGORIES = baseURL + 'categories'
const GET_MY_POSTS = baseURL + 'posts/'
const GET_POST = baseURL + 'post/'
const POST_POST = baseURL + 'post'
const DELETE_POST = baseURL + 'post/'
const PUT_POST = baseURL + 'post/'
const GET_USER = baseURL + 'users/'
const PUT_USER = baseURL + 'users/'
const DELETE_USER = baseURL + 'users/'
const POST_COMMENT = baseURL + 'comment'
const DELETE_COMMENT = baseURL + 'comment/'
const PUT_COMMENT = baseURL + 'comment/'
const POST_NEWSLETTERS = baseURL + 'newsletter'
const USER_COMMENTS = baseURL + 'comments'
const SUBSCRIBED_CATEGORIES = '/subscribedcategories'
const COMMENTS = '/comments'
const LIKE = '/likedUsers'
const LIKED = '/likedUsers'

export const getUserSubscribedCategoriesFetch = async (userId) => {
  return await fetch(GET_USER + userId + SUBSCRIBED_CATEGORIES)
}

export const postNewslettersFetch = (body, authId) => {
  const requestOptions = {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      userID: authId
    },
    body: body
  }

  return fetch(POST_NEWSLETTERS, requestOptions)
}

export const getCategoriesFetch = async () => {
  return await fetch(GET_CATEGORIES)
}

export const getMyPostsFetch = async (userId) => {
  const requestOptions = {
    headers: {
      'Content-Type': 'application/json'
    }
  }
  return await fetch(GET_MY_POSTS + userId, requestOptions)
}

export const postPostFetch = (body, notificationFlag, setNotificationFlag, authId) => {
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

export const promotePostFetch = (postId, body, authId) => {
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

export const getUserFetch = async (userId) => {
  return await fetch(GET_USER + userId)
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

export const getUserCommentsFetch = async (authId) => {
  const requestOptions = {
    headers: {
      'Content-Type': 'application/json',
      userID: authId
    }
  }
  return await fetch(USER_COMMENTS, requestOptions)
}
