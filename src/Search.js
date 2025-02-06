import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';

const Search = ({ texts, language }) => {
  const [players, setPlayers] = useState([]);

  useEffect(() => {
    fetch('http://localhost:5269/UnityController/PlayerWebList')
      .then(response => response.json())
      .then(data => setPlayers(data))
      .catch(error => console.error('Error fetching player data:', error));
  }, []);

  const renderPlayers = () => {
    const rows = [];
    for (let i = 0; i < players.length; i += 3) {
      rows.push(
        <div className="row" key={i} style={{ display: 'flex', justifyContent: 'flex-start' }}>
          {players.slice(i, i + 3).map((player, index) => (
            <Link to={`/player/${player.id}`} className="styled-link" key={index} style={{ flex: 1, textAlign: 'center' }}>
              {player.name}
            </Link>
          ))}
        </div>
      );
    }
    return rows;
  };

  return (
    <div className='content'>
      <div className='box' style={{ height: '80vh', width: "80vw" }}>
        <input type="search" placeholder={texts[language].search + " onchange lesz"} style={{ width: "100%", height: "50px", border: "2px solid black", borderRadius: "10px", fontSize: "20px", padding: "10px" }} />
        <div id="searchbox">
          {renderPlayers()}
        </div>
      </div>
    </div>
  );
};

export default Search;