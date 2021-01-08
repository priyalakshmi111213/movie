import React, { Component } from 'react';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

export class Home extends Component {
    displayName = Home.name
    cacheClear() {
        fetch("api/clearcache")
            .then(response => response.json())
            .then(data => {
                if (data) {
                    toast("Cache Cleared !")
                }
            });
    }

   

  render() {
    return (
      <div>
        <h1>Sample Movie Web App</h1>
        <p>This sample app is built using </p>
        <ul>
          <li>ASP.NET Core</li>
          <li>React for client-side code</li>
          <li>Bootstrap for layout and styling</li>
        </ul>
          
            <b>About The Sample Web App</b>
            <ul className="styleNone"> 
                <li>Providers are configured though configuration file (appsettings.json) , based on the providers we fetch the movies.</li>
                <li>Now we have configured <strong>filmworld, cinemaworld</strong> providers.</li>
                <li> Based on the providers we fetch all the movies and there details,from below API's </li>
                <li>/api/(cinemaworld or filmworld)/movies </li>
                <li>/api/(cinemaworld or filmworld)/movie/(ID) </li>
                <li>The data is stored in Cache for the first request and then retrieved from the cache for other requests instead of calling exetrnal API</li>
                <li>From the two providers movies with least prices are returned to the UI. </li>
            </ul>
           
        <p>Please click below to clear the cache and request the data from the server</p>
        <ul>
                <button className="btn btn-primary margin-top" type="button" onClick={() => this.cacheClear()}>
                    cache clear
                </button>
                <ToastContainer />
        </ul>
      </div>
    );
  }
}
