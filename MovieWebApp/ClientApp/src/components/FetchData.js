import React, { Component } from "react";
import ReadMoreReact from 'read-more-react';
export class FetchData extends Component {
  displayName = FetchData.name;

  constructor(props) {
    super(props);
    this.state = { movies: [], loading: true,error:false };

      fetch("api/movies")
          .then(response => response.json())
          .then(data => {
              console.log(data);
              this.setState({ movies: data, loading: false });
          }).catch(err => {
              console.log("Error Reading data " + err);
          });
  }

   
    static renderMoviesTable(movies) {
    return (
        <table className="table">
            <thead>

            </thead>
            <tbody>
                {movies.map(movie => (
                    <tr key={movie.id}>
                        <td> <div className="row placeholders">
                            <div className="col-xs-6 col-sm-4 placeholder">
                                <h4>{movie.title}</h4>
                                <div>
                                    <img src={movie.poster} alt={movie.title} height="140" width="140" className="img-thumbnail" />
                                </div>
                                <div className="glyphicon glyphicon-heart color">

                                </div>
                                <span className="padding-right"><b>{movie.metascore}%</b></span>
                                <span className="padding-right">{movie.genre} | {movie.language}  </span>

                                <button className="btn btn-primary margin-top" type="button">
                                    Price <span className="badge">{movie.price}</span>
                                </button>

                            </div>
                            <div className="col-xs-6 col-sm-1">
                                </div>
                            <div className="col-xs-6 col-sm-7">
                                <br />
                                <br/>
                                <div>
                                    <div className="padding-right"><b>Actors:</b></div>
                                    <div className="padding-right">{movie.actors}</div>
                                </div>
                                <div>
                                    <div className="padding-right"><b>Director:</b></div>
                                    <div className="padding-right">{movie.director}</div>
                                </div>
                                <div>
                                    <div className="padding-right"><b>Writer:</b></div>
                                    <div className="padding-right">{movie.writer}</div>
                                </div>
                                <div>
                                    <div className="padding-right"><b>Plot:</b></div>
                                    <div className="cursor"> <ReadMoreReact text={movie.plot}
                                        min={30}
                                        ideal={40}
                                        max={1000}
                                        readMoreText="click here to read more" /></div>
                                </div>
                                <div className="padding-right"><b>Provider:</b> {movie.provider}</div>
                                
                            </div>
                          
                        </div>
                        </td>


                    </tr>
                ))}
            </tbody>
        </table>
    );
  }

    render() {

    let contents = this.state.loading ? (
      <p>
        <em>Loading...</em>
      </p>
    ) : (
            this.state.movies.length > 0 ? FetchData.renderMoviesTable(this.state.movies) : 
                <p>
                    <em>No Movies...</em>
                </p>

            
            );
        if (this.state.error) {
            <div>
                <h1>Exception Occured</h1>
            </div>
        }

    return (
      <div>
        <h1>Movies</h1>
      
        {contents}
      </div>
    );
  }
}
