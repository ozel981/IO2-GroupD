import React, { useState, useEffect } from 'react'
import Select from 'react-select'
import TextareaAutosize from 'react-textarea-autosize'
import { getCategoriesFetch, postPostFetch } from '../../apiMethods'
import { connect } from 'react-redux'
import { isResponseProper, showResponseSnackbarError } from '../response/responseHandler'

const NewPost = (props) => {
  const [loading, setLoading] = useState(true)
  const [categories, setCategories] = useState([])
  const [title, setTitle] = useState('')
  const [content, setContent] = useState('')
  const [selected, setSelected] = useState(null)

  useEffect(() => {
    setStatesDefault()
  }, [props.notificationFlag])

  useEffect(() => {
    getCategories()
  }, [])

  return (
    <div className="container border border-light rounded-lg" style={{ backgroundColor: '#ebeff2' }}>
      <div className="container" style={{ padding: '10px' }}>
        <div className="row no-gutters">
          <div className="col-8">
            <div className="row no-gutters">
              <input className="rounded"
                id="post-title-input"
                type="text"
                maxLength="64"
                value={title}
                onChange={e => setTitle(e.target.value)}
                placeholder="Title..."
              />
            </div>
          </div>
          <div className="col-4">
            <div className="row no-gutters">
              {loading
                ? <label>Loading...</label>
                : <Select
                  id="post-category-selector"
                  placeholder="Choose category..."
                  options={categories}
                  value={selected}
                  onChange={setSelected}
                />}
            </div>
          </div>
        </div>
        <div className="row no-gutters">
          <div className="col-8">
            <div className="row no-gutters">
              <TextareaAutosize className="rounded"
                id="post-content-input"
                minRows="4"
                maxLength="512" value={content}
                onChange={e => setContent(e.target.value)}
                style={{ resize: 'none' }}
                placeholder="Text..."
              />
            </div>
          </div>
          <div className="col-3">
          </div>
          <div className="col-1">
            <div className="row no-gutters" style={{ position: 'absolute', bottom: 0 }}>
              <button id="post-create-button" type="button" className="btn btn-success" style={{ fontSize: 14 }} onClick={sendPost}>Post</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  )

  function setStatesDefault () {
    setTitle('')
    setContent('')
    setSelected(null)
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
      setLoading(false)
    } else {
      showResponseSnackbarError(getCategoriesFetchResponse, props.snackbarRef)
    }
  }

  async function sendPost () {
    if (title === '' || content === '' || selected === null) {
      alert('Some value is empty. Cannot post.')
      return
    }

    const body = JSON.stringify({
      title: title,
      content: content,
      categoryID: selected.value
    })

    const postPostResponse = await postPostFetch(body, props.notificationFlag, props.setNotificationFlag, props.authId)
    if (!isResponseProper(postPostResponse)) {
      showResponseSnackbarError(postPostResponse, props.snackbarRef)
    }
    props.setNotificationFlag(!props.notificationFlag)
  }
}

const mapStateToProps = (state) => {
  return {
    authId: state.authId,
    snackbarRef: state.snackbarRef
  }
}

export default connect(mapStateToProps)(NewPost)
