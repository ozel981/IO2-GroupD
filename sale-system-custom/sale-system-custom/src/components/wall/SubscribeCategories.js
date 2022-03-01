import React, { useState, useEffect } from 'react'
import FormControl from '@material-ui/core/FormControl'
import { connect } from 'react-redux'
import { getUserSubscribedCategoriesFetch, postNewslettersFetch } from '../../apiMethods'
import CheckBoxIcon from '@material-ui/icons/CheckBox'
import CheckBoxOutlineBlankIcon from '@material-ui/icons/CheckBoxOutlineBlank'
import HelpIcon from '@material-ui/icons/Help'
import { Tooltip, IconButton } from '@material-ui/core'
import { isResponseProper, showResponseSnackbarError } from '../response/responseHandler'

const SubscribeCategories = (props) => {
  const [categories, setCategories] = useState([])
  const [loadingSubCat, setLoadingSubCat] = useState(true)
  const [subscribedCategoriesIds, setSubscribedCategoriesIds] = useState([])
  const [categoriesNF, setCategoriesNF] = useState(false)

  useEffect(() => {
    setCategories([...props.categories])
  }, [props.categories])

  useEffect(() => {
    getSubscribedCategories()
  }, [categoriesNF])

  return (
    <div id="categories-subs">
      <FormControl component="fieldset">
        <div>
          {categories.map(category =>
            <div className="row" key={category.id}>
              <div id={'category-' + category.id + '-label'} className="col-8">
                {category.name}
              </div>
              <div className="col-2"></div>
              <div className="col-2">
                {
                  loadingSubCat
                    ? <HelpIcon />
                    : subscribedCategoriesIds.includes(category.id)
                      ? <Tooltip title="Unsubscribe category">
                        <IconButton id={'unsub-cat-' + category.id + '-b'} color="primary" size="small" aria-label="Unsubscribe"
                          onClick={() => { subscribeCategory(category.id, false) }} >
                          <CheckBoxIcon />
                        </IconButton>
                      </Tooltip>
                      : <Tooltip title="Subscribe category">
                        <IconButton id={'sub-cat-' + category.id + '-b'} color="default" size="small" aria-label="Subscribe"
                          onClick={() => { subscribeCategory(category.id, true) }} >
                          <CheckBoxOutlineBlankIcon />
                        </IconButton>
                      </Tooltip>
                }
              </div>
            </div>
          )}
        </div>
      </FormControl>
      <hr />
    </div>
  )

  async function getSubscribedCategories () {
    const subscribedCategoriesFetchResponse = await getUserSubscribedCategoriesFetch(props.authId)
    if (isResponseProper(subscribedCategoriesFetchResponse)) {
      const subscribedCategories = await subscribedCategoriesFetchResponse.json()
      let subCategoriesIds = []
      subscribedCategories.forEach(subCategory => {
        subCategoriesIds = [...subCategoriesIds, subCategory.id]
      })
      setSubscribedCategoriesIds([...subCategoriesIds])
      setLoadingSubCat(false)
    } else {
      showResponseSnackbarError(subscribedCategoriesFetchResponse, props.snackbarRef)
    }
  }

  async function subscribeCategory (categoryId, isSubscribed) {
    const body = JSON.stringify({
      categoryID: categoryId,
      isSubscribed: isSubscribed
    })

    const postNewslettersFetchResponse = await postNewslettersFetch(body, props.authId)
    if (!isResponseProper(postNewslettersFetchResponse)) {
      showResponseSnackbarError(postNewslettersFetchResponse, props.snackbarRef)
    }
    setCategoriesNF(!categoriesNF)
  }
}

const mapStateToProps = (state) => {
  return {
    authId: state.authId,
    snackbarRef: state.snackbarRef
  }
}

export default connect(mapStateToProps)(SubscribeCategories)
