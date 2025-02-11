import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';

export default function Player({ texts, language }) {
  const { id } = useParams();
  const [stats, setStats] = useState({
    score: 0,
    enemiesKilled: 0,
    timePlayed: 0,
    deaths: 0
  });
  const [name, setName] = useState('Player');

  useEffect(() => {
    fetch(`http://localhost:5269/GetbyId/${id}`)
      .then(response => response.json())
      .then(data => {
        if (data.statistic) {
          setStats(data.statistic);
        }
        if(data.name) {
          setName(data.name);
          console.log(data.name);
        }
      })
      .catch(error => console.error('Error fetching player statistics:', error));
  }, [id]);

  return (
    <div className='content'>
        <div className='box' style={{height: '50vh', width: "50vw"}}>
            <h3>{language === "hu" ? `${name} statisztikái` : 'Players statistics'}</h3>
            <hr/>
            <div className="left">
                <p>{language === "hu" ? 'Pontok: ' : 'Score: '}{stats.score}</p>
                <p>{language === "hu" ? 'Megölt ellenfelek: ' : 'Enemies killed: '}{stats.enemiesKilled}</p>
                <p>{language === "hu" ? 'Játszott idő: ' : 'Time played: '}{stats.timePlayed}</p>
                <p>{language === "hu" ? 'Halálok: ' : 'Deaths: '}{stats.deathCount}</p>
            </div>
        </div>
    </div>
  )
}