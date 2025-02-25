import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import axios from 'axios';
import Kep from './img/profilkep.jpg';
import AdminKep from './img/admin.png';

const Search = ({ texts, language, isAdmin }) => {
  const [players, setPlayers] = useState([]);
  const [filteredPlayers, setFilteredPlayers] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');

  useEffect(() => {
    axios.get('http://localhost:5269/api/Player')
      .then(response => {
        const data = response.data;
        setPlayers(data);
        setFilteredPlayers(data);
      })
      .catch(error => console.error('Error fetching player data:', error));
  }, []);

  const handleSearch = (term) => {
    setSearchTerm(term);
    if (term === '') {
      setFilteredPlayers(players);
    } else {
      const filtered = players.filter(player => player.name.toLowerCase().includes(term.toLowerCase()));
      setFilteredPlayers(filtered);
    }
  };

  const renderPlayers = () => {
    const rows = [];
    for (let i = 0; i < filteredPlayers.length; i += 3) {
      rows.push(
        <div className="row" key={i} style={{ display: 'flex', justifyContent: 'space-evenly' }}>
          {filteredPlayers.slice(i, i + 3).map((player) => (
            <Link to={`/player/${player.id}`} className="styled-link" key={player.id} style={{ flex: '1 1 0', textAlign: 'center', margin: '0 10px' }}>
              <img src={Kep} alt={`${player.name} kep`} />
              {player.isAdmin ? <img src={AdminKep} alt='badge' id="badgesrc" /> : null}
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
          id='search'
          style={{ width: '100%', height: '50px', border: '2px solid black', borderRadius: '10px', fontSize: '20px', padding: '10px', color: 'azure', backgroundColor: '#171717' }}
          value={searchTerm}
          onChange={(e) => handleSearch(e.target.value)}
        />
        <div id="searchbox">
          {renderPlayers()}
        </div>
      </div>
    </div>
  );
};

export default Search;
