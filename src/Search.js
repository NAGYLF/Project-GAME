import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import Kep from './img/profilkep.jpg';

const Search = ({ texts, language }) => {
  const [players, setPlayers] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');

  // Az első betöltésnél lekérjük az összes játékost
  useEffect(() => {
    console.log('Fetching all players...');
    fetch('http://localhost:5269/api/Player')
      .then(response => response.json())
      .then(data => {
        console.log('Fetched all players:', data); // Itt logoljuk a teljes választ
        setPlayers(data);
      })
      .catch(error => console.error('Error fetching player data:', error));
  }, []);

  // Keresési funkció: ha üres, lekérjük az összes játékost, ha van keresési kifejezés, akkor a nevével szűrjük
  const handleSearch = (term) => {
    console.log('Searching for:', term); // A keresési kifejezés logolása
    if (term === '') {
      // Ha üres a keresési mező, lekérjük az összes játékost
      fetch('http://localhost:5269/api/Player')
        .then(response => response.json())
        .then(data => {
          console.log('Fetched all players after clearing search:', data); // Keresés törlés után
          setPlayers(data);
        })
        .catch(error => console.error('Error fetching player data:', error));
    } else {
      // Ha van keresési kifejezés, szűrjük a játékosokat a név alapján
      fetch(`http://localhost:5269/api/Player/GetByName/${term}`)
        .then(response => response.json())
        .then(data => {
          console.log(`Fetched players for search term "${term}":`, data); // Keresési eredmény logolása
          let tomb = [];
          tomb.push(data);
          setPlayers(tomb);
        })
        .catch(error => console.error('Error fetching player data:', error));
    }
  };

  // A játékosokat soronként rendezzük
  const renderPlayers = () => {
    console.log('Rendering players:', players); // Itt logoljuk a játékosokat, mielőtt renderelésre kerülnek
    const rows = [];
    for (let i = 0; i < players.length; i += 3) {
      rows.push(
        <div className="row" key={i} style={{ display: 'flex', justifyContent: 'space-evenly' }}>
          {players.slice(i, i + 3).map((player) => (
            <Link to={`/player/${player.id}`} className="styled-link" key={player.id} style={{ flex: '1 1 0', textAlign: 'center', margin: '0 10px' }}>
              <img src={Kep} alt={`${player.name} Image`} />
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
          value={searchTerm}
          onChange={(e) => {
            setSearchTerm(e.target.value);
            handleSearch(e.target.value); // Keresés indítása
          }}
        />
        <div id="searchbox">
          {renderPlayers()} {/* A játékosok renderelése */}
        </div>
      </div>
    </div>
  );
};

export default Search;
