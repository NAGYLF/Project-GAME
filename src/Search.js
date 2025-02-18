import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import axios from 'axios';
import Kep from './img/profilkep.jpg';

const Search = ({ texts, language }) => {
  const [players, setPlayers] = useState([]);

  // Az első betöltésnél lekérjük az összes játékost
  useEffect(() => {
    axios.get('http://localhost:5269/api/Player')
      .then(response => {
        const data = response.data;
        setPlayers(data);
      })
      .catch(error => console.error('Error fetching player data:', error));
  }, []);

  // Keresés: ha üres, lekérjük az összes játékost, ha van keresési kifejezés, akkor a nevével szűrjük
  const handleSearch = (term) => {
    if (term === '') {
      // Ha üres a keresési mező, lekérjük az összes játékost
      axios.get('http://localhost:5269/api/Player')
        .then(response => {
          const data = response.data;
          setPlayers(data);
        })
        .catch(error => console.error('Error fetching player data:', error));
    } else {
      // Ha van keresési kifejezés, szűrjük a játékosokat a név alapján
      const filteredPlayers = players.filter(player => player.name.toLowerCase().includes(term.toLowerCase()));
      setPlayers(filteredPlayers);
    }
  };

  // A játékosokat soronként rendezzük
  const renderPlayers = () => {
    const rows = [];
    for (let i = 0; i < players.length; i += 3) {
      rows.push(
        <div className="row" key={i} style={{ display: 'flex', justifyContent: 'space-evenly' }}>
          {players.slice(i, i + 3).map((player) => (
            <Link to={`/player/${player.id}`} className="styled-link" key={player.id} style={{ flex: '1 1 0', textAlign: 'center', margin: '0 10px' }}>
              <img src={Kep} alt={`${player.name} kep`} />
              <div className="player-name">{player.name}</div>
            </Link>
          ))}
        </div>
      );
    }
    return rows;
  };

  return (
    <div className="content">
      <div className="box" style={{ height: '80vh', width: '80vw' }}>
        <input
          type="search"
          placeholder={texts[language].search}
          style={{ width: '100%', height: '50px', border: '2px solid black', borderRadius: '10px', fontSize: '20px', padding: '10px', color: 'azure', backgroundColor: '#171717' }}
          onChange={(e) => {
            handleSearch(e.target.value);
          }}
        />
        <div id="searchbox">
          {renderPlayers()}
        </div>
      </div>
    </div>
  );
};

export default Search;