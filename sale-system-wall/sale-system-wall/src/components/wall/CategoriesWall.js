import React, { useState, useEffect } from 'react'
import FormLabel from '@material-ui/core/FormLabel'
import FormControl from '@material-ui/core/FormControl'
import FormGroup from '@material-ui/core/FormGroup'
import FormControlLabel from '@material-ui/core/FormControlLabel'
import Checkbox from '@material-ui/core/Checkbox'
import { Tooltip, IconButton } from '@material-ui/core'
import SearchIcon from '@material-ui/icons/Search'

const CategoriesWall = (props) => {
  const [categories, setCategories] = useState([])
  const [allIDs, setAllIDs] = useState([])
  const [prevSelected, setPrevSelected] = useState([])
  const [selected, setSelected] = useState([])

  useEffect(() => {
    setCategories([...props.categories])

    const ids = []
    props.categories.map(category => ids.push(category.id))
    setAllIDs([...ids])

    setPrevSelected([])
    setSelected([...props.selected])
  }, [props.categories, props.selected])

  return (
    <div>
      <FormControl component="fieldset">
        <FormLabel component="legend">Show posts from selected ones</FormLabel>
        <FormGroup>
          <FormControlLabel key="all"
            control={<Checkbox
              id={'category-all-checkbox'}
              checked={selected.length === allIDs.length}
              key={'ch-b-all'}
              onChange={e => handleOnAllChange(e)}
            />}
            label="All"
          />
          {categories.map(category =>
            <FormControlLabel key={category.id}
              id={'category-' + category.id + '-label'}
              control={<Checkbox
                id={'category-' + category.id + '-checkbox'}
                checked={selected.includes(category.id)}
                key={'ch-b-' + category.id}
                onChange={e => handleOnChange(e, category.id)}
              />}
              label={category.name}
            />
          )}
        </FormGroup>
      </FormControl>
      <hr />
      <div className="text-right">
        <Tooltip title="Filter posts">
          <IconButton id={'categories-filter-button'} color="secondary" aria-label="Filter posts" onClick={searchClick}>
            <SearchIcon />
          </IconButton>
        </Tooltip>
      </div>
    </div>)

  function handleOnAllChange (e) {
    const checked = e.target.checked
    if (!checked) {
      setSelected([...prevSelected])
    } else {
      setPrevSelected([...selected])
      setSelected([...allIDs])
    }
  }

  function handleOnChange (e, id) {
    const checked = e.target.checked
    const tmpSelected = selected
    if (!checked) {
      tmpSelected.splice(tmpSelected.indexOf(id), 1)
    } else {
      if (tmpSelected.length === allIDs.length - 1) {
        setPrevSelected([])
      }
      tmpSelected.push(id)
    }
    setSelected([...tmpSelected])
  }

  function searchClick () {
    props.setSelected([...selected])
    props.setSearch(!props.search)
  }
}

export default CategoriesWall
