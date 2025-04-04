import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useNavigate, useParams } from 'react-router-dom';

export default function Player({ language, token, isAdmin, showAlert }) {
  const [isItAdmin, setIsItAdmin] = useState(false);
  const [isItBanned, setIsItBanned] = useState(false);
  const [loading, setLoading] = useState(true);

  const { id } = useParams();
  const [stats, setStats] = useState({
    score: 0,
    enemiesKilled: 0,
    timePlayed: 0,
    deaths: 0
  });
  const [name, setName] = useState('Player');
  const navigate = useNavigate();
  
  //Ha a contentre nyomunk nem a boxra akkor visszamegy a főoldalra
    const handleContentClick = (e) => {
      if(e.target.className === 'content'){
          navigate('/');
      }
    }

    //Lekéri id alapján a playert, bizonyos adatok miatt
  useEffect(() => {
    axios.get(`${process.env.REACT_APP_URL}/GetbyId/${id}`)
      .then(response => {
        const data = response.data;
        if (data.name) {
          setName(data.name);
          setIsItAdmin(data.isAdmin);
          setIsItBanned(data.isBanned);
        }
      })
      .catch(error => console.error('Error fetching player name:', error));

      //Lekéri id alapján a player statjait
      axios.get(`${process.env.REACT_APP_URL}/api/Player/stats/${id}`)
      .then(response => {
        const data = response.data;
        if (data) {
          setStats({
            score: data.score,
            enemiesKilled: data.enemiesKilled,
            deathCount: data.deathCount
          });
        }
      })
      .catch(error => console.error('Error fetching player statistics:', error))
      .finally(() => {
        setTimeout(() => {
          setLoading(false);
        }, 100);
      });
  }, [id]);

  if (loading) {
    return <div id="loader"></div>;
  }

  return (
    <div className='content' onClick={handleContentClick}>
      <div className='box' style={{ height: '50vh', width: "50vw", position: 'relative' }}>
        <h3>{language === "hu" ? `${name} statisztikái` : `${name} statistics`}</h3>
        <hr />
        <div className="left">
          {isItBanned ? <p style={{ color: '#dc3545' }}>{language === "hu" ? 'Ez a játékos bannolva van!' : 'This player is banned!'}</p> : null}
          <p>{language === "hu" ? 'Pontok: ' : 'Score: '}{stats.score == null ? "0" : stats.score}</p>
          <p>{language === "hu" ? 'Megölt ellenfelek: ' : 'Enemies killed: '}{stats.enemiesKilled == null ? "0" : stats.enemiesKilled}</p>
          <p>{language === "hu" ? 'Halálok: ' : 'Deaths: '}{stats.deathCount == null ? "0" : stats.deathCount}</p>
        </div>

        {isAdmin && !isItAdmin ? (
          <button className='btn btn-danger' style={{ position: 'absolute', bottom: '15px', right: '15px', height: '40px' }} onClick={() => {
            if (window.confirm(language === "hu" ? 'Biztosan törölni szeretnéd a fiókot?' : 'Are you sure you want to delete this account?')) {
              axios.delete(`${process.env.REACT_APP_URL}/api/Player/${id}?token=${token}`)
                .then(() => {
                  showAlert(language === "hu" ? "Fiók sikeresen törölve!" : "Account deleted successfully!", "success");
                  navigate("/search");
                })
                .catch(error => console.error('Error deleting player:', error));
            }
          }}>
            <svg xmlns="http://www.w3.org/2000/svg" width="25" height="25" fill="currentColor" className="bi bi-trash" viewBox="0 0 16 16">
              <path d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5m2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5m3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0z" />
              <path d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1zM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4zM2.5 3h11V2h-11z" />
            </svg>
          </button>
        ) : null}

        {isAdmin && !isItAdmin && !isItBanned ? (
          <button className='btn btn-danger' style={{ position: 'absolute', bottom: '15px', left: '15px', height: '40px' }} onClick={() => {
            if (window.confirm(language === "hu" ? 'Biztosan bannolni szeretnéd a fiókot?' : 'Are you sure you want to ban this account?')) {
              axios.put(`${process.env.REACT_APP_URL}/api/Player/${id}/ban`, {
                isBanned: true
              })
                .then(() => {
                  showAlert(language === "hu" ? "Fiók sikeresen bannolva!" : "Account banned successfully!", "success");
                  navigate("/search");
                })
                .catch(error => console.error('Error banning player:', error));
            }
          }}>
            {language === "hu" ? "Játékos bannolása" : "Ban player"}
          </button>
        ) : null}

        {isAdmin && !isItAdmin && isItBanned ? (
          <button className='btn btn-light' style={{ position: 'absolute', bottom: '15px', left: '15px', height: '40px' }} onClick={() => {
            if (window.confirm(language === "hu" ? 'Biztosan unbannolni szeretnéd a fiókot?' : 'Are you sure you want to unban this account?')) {
              axios.put(`${process.env.REACT_APP_URL}/api/Player/${id}/ban`, {
                isBanned: false
              })
                .then(() => {
                  showAlert(language === "hu" ? "Fiók sikeresen unbannolva!" : "Account unbanned successfully!", "success");
                  navigate("/search");
                })
                .catch(error => console.error('Error unbanning player:', error));
            }
          }}>
            {language === "hu" ? "Játékos unbannolása" : "Unban player"}
          </button>
        ) : null}
      </div>
    </div>
  );
}
