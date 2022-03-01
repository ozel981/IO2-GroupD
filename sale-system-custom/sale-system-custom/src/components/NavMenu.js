import React, { Component } from 'react'
import { Container, Navbar, NavbarBrand } from 'reactstrap'
import { Link } from 'react-router-dom'
import './NavMenu.css'

export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor (props) {
    super(props)

    this.toggleNavbar = this.toggleNavbar.bind(this)
    this.state = {
      collapsed: true
    }
  }

  toggleNavbar () {
    this.setState({
      collapsed: !this.state.collapsed
    })
  }

  render () {
    return (
      <header>
        <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
          <div className="col-9">
          <Container>
            <NavbarBrand id="go-to-start-button" tag={Link} to="/home">SALE SYSTEM</NavbarBrand>
            <NavbarBrand id="go-to-wall-button" tag={Link} to="/wall">WALL</NavbarBrand>
            <NavbarBrand id="go-to-user-button" tag={Link} to="/user">MY ACCOUNT</NavbarBrand>
          </Container>
          </div>
          <div className="col-3"></div>
        </Navbar>
      </header>
    )
  }
}
