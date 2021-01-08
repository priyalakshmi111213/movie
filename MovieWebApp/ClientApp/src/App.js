import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchData } from './components/FetchData';
import { ErrorBoundary } from './components/ErrorBoundary';



export default class App extends Component {
  displayName = App.name
    componentWillMount() {
        this.renderMyData();
    }

    renderMyData() {
        fetch("api/movies")
            .then(response => response.json())
            .then(data => {
                console.log(data);
            }).catch(err => {
                console.log("Error Reading data " + err);
            });
    }

  render() {
      return (
          <ErrorBoundary>
        <Layout>
        <Route exact path='/' component={Home} />
                <Route path='/fetchdata' component={FetchData} />
              </Layout>
              </ErrorBoundary>
        
    );
  }
}
