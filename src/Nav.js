import React, { useEffect, useState } from 'react';
import Kep from './img/profilkep.jpg';
import Arpi from './img/arpi.png'; 
import Balow from './img/balow.png';
import Levi from './img/levi.png';
import Video from './video/video.mp4';
import './Nav.css'

export default function Nav() {

  const [isAdmin, setIsAdmin] = useState(false);
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [username, setUsername] = useState('');
  const [code, setCode] = useState('');
  const [secondsLeft, setSecondsLeft] = useState(30);
  const [language, setLanguage] = useState('hu'); // Alapértelmezett nyelv magyar

  const texts = {
    hu: {
      description: "Leírás",
      about: "Rólunk",
      login: "Bejelentkezés",
      register: "Regisztráció",
      download: "Letöltés",
      settings: "Beállítások",
      signout: "Kijelentkezés",
    },
    en: {
      description: "Description",
      about: "About",
      login: "Login",
      register: "Register",
      download: "Download",
      settings: "Settings",
      signout: "Sign Out",
    },
  };

  const changeLanguage = (lang) => {
    setLanguage(lang);
  };

  //Bejelentkezés
  function getPlayerByNameAndPassword(name, password) {
    fetch(`http://localhost:5269/UnityController/${name},${password}`)
      .then(response => {
        if (!response.ok) {
          throw new Error('Játékos nem található');
        }
        return response.json();
      })
      .then(player => {
        console.log('Játékos megtalálva:', player);
        setUsername(player.name);
        setIsLoggedIn(true);
        if(player.isAdmin == 1){
          setIsAdmin(true);
        }
      })
      .catch(error => {
        console.error('Hiba történt:', error);
        setIsLoggedIn(false);
      });
  }

  const handleLogin = () => {
    const name = document.getElementById('username').value;
    const password = document.getElementById('password').value;
    getPlayerByNameAndPassword(name, password);
  };

  //Kód
  useEffect(() => {
    fetch('http://localhost:5269/UnityController/code')
      .then(res => res.json())
      .then(data => {
        setCode(data.code);
        setSecondsLeft(data.secondsLeft);
      })
      .catch(err => console.error(err));

    const interval = setInterval(() => {
      setSecondsLeft(prev => {
        if (prev > 1) {
          return prev - 1;
        } else {
          fetch('http://localhost:5269/UnityController/code')
            .then(res => res.json())
            .then(data => {
              setCode(data.code);
              setSecondsLeft(data.secondsLeft);
            })
            .catch(err => console.error(err));
          return 30;
        }
      });
    }, 1000);

    return () => clearInterval(interval);
  }, []);


  return (
<>
{/* Háttér videó */}
<video 
  className="video-background" 
  autoPlay 
  muted 
  loop 
  style={{
    position: 'absolute', 
    top: 0, 
    left: 0, 
    width: '100%', 
    height: '100%', 
    objectFit: 'cover',
    zIndex: -1
  }}
>
  <source src={Video} type="video/mp4" />
  A böngésző nem támogatja a videó lejátszást.
</video>
    <nav className="navbar navbar-expand-lg custom-navbar">
      <div className="container-fluid">
        <button
          className="navbar-toggler"
          type="button"
          data-bs-toggle="collapse"
          data-bs-target="#navbarSupportedContent"
          aria-controls="navbarSupportedContent"
          aria-expanded="false"
          aria-label="Toggle navigation"
        >
          <span className="navbar-toggler-icon"></span>
        </button>

        <div className="collapse navbar-collapse" id="navbarSupportedContent">
            <ul className="navbar-nav me-auto mb-2 mb-lg-0">
              <li className="nav-item">
                <a className="nav-link" href="#descriptionModal" data-bs-toggle="modal">
                  {texts[language].description}
                </a>
              </li>
              <li className="nav-item">
                <a className="nav-link" href="#aboutModal" data-bs-toggle="modal">
                  {texts[language].about}
                </a>
              </li>
              <div className="dropdown ms-2 nav-item" style={{border: "none"}}>
              <button
                className="btn btn-outline-light dropdown-toggle"
                type="button"
                id="dropdownMenuButtonLanguage"
                data-bs-toggle="dropdown"
                aria-expanded="false"
              >
                {language === 'hu' ? 'Magyar' : 'English'}
              </button>
              <ul className="dropdown-menu" aria-labelledby="dropdownMenuButtonLanguage">
                <li>
                  <a className="dropdown-item" onClick={() => changeLanguage('hu')}>
                    Magyar
                  </a>
                </li>
                <li>
                  <a className="dropdown-item" onClick={() => changeLanguage('en')}>
                    English
                  </a>
                </li>
              </ul>
            </div>
              {isLoggedIn && (
                <li className="nav-item">
                  <a className="nav-link">
                    {texts[language].download}
                  </a>
                </li>
              )}
            </ul>

            <form className="d-flex ms-auto align-items-center text-center">
              {isLoggedIn ? (
                <div className="dropstart">
                  <button
                    type="button"
                    id="dropdownMenuButton"
                    data-bs-toggle="dropdown"
                    aria-expanded="false"
                    style={{
                      background: 'none',
                      border: 'none',
                      padding: 0,
                      margin: 0,
                      display: 'inline-block',
                    }}
                  >
                    <img
                      src={Kep}
                      alt="Profil"
                      style={{ width: '30px', height: '30px', borderRadius: '50%' }}
                    />
                  </button>
                  <ul className="dropdown-menu" aria-labelledby="dropdownMenuButton">
                    {isAdmin && (
                      <li>
                        <a className="dropdown-item" href="#adminModal" data-bs-toggle="modal">
                          Admin
                        </a>
                      </li>
                    )}
                    <li>
                      <a className="dropdown-item" href="#settingsModal" data-bs-toggle="modal">
                      {texts[language].settings}
                      </a>
                    </li>
                    <li>
                      <a onClick={() => setIsLoggedIn(false)} className="dropdown-item">
                      {texts[language].signout}
                      </a>
                    </li>
                  </ul>
                  <p style={{margin:"-1px"}}>{username ? username : ''}</p>
                </div>
              ) : (
                <>
                  <a className="btn btn-outline-light ms-2" data-bs-toggle="modal" data-bs-target="#loginModal">
                    {texts[language].login}
                  </a>
                  <a className="btn btn-outline-light ms-2" data-bs-toggle="modal" data-bs-target="#registerModal">
                    {texts[language].register}
                  </a>
                </>
              )}

            </form>
          </div>
        </div>

      {/* Leírás Modal */}
      <div
        className="modal fade"
        id="descriptionModal"
        tabIndex="-1"
        aria-labelledby="descriptionModalLabel"
        aria-hidden="true"
      >
        <div className="modal-dialog modal-dialog-centered">
          <div className="modal-content">
            <div className="modal-header">
              <h5 className="modal-title" id="descriptionModalLabel">{texts[language].description}</h5>
            </div>
            <div className="modal-body">
              Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus lacinia est eget ante
              euismod, id sollicitudin leo malesuada. Integer convallis malesuada leo, ac auctor
              lacus interdum et. Cras vestibulum, sapien at consequat tristique, risus metus
              tincidunt orci, eu posuere ligula ligula non turpis. Etiam vitae efficitur libero.
            </div>
          </div>
        </div>
      </div>

      {/* Rólunk Modal */}
      <div
        className="modal fade"
        id="aboutModal"
        tabIndex="-1"
        aria-labelledby="aboutModalLabel"
        aria-hidden="true"
      >
        <div className="modal-dialog modal-dialog-centered modal-xl">
          <div className="modal-content">
            <div className="modal-header">
              <h5 className="modal-title" id="aboutModalLabel">{texts[language].about}</h5>
            </div>
            <div className="modal-body">
              <div className="row text-center align-items-center">
                <div className="col-md-4">
                  <h2><img src={Levi} style={{width: '30px', height: '30px', borderRadius: '50%'}}></img>{language === "hu" ? 'Nagy Levente Ferenc' : 'Levente Ferenc Nagy'}</h2>
                  <p>{language === "hu" ? 'A cél szentesíti az eszközt. Ha megdobnak kővel dobd vissza kézigránáttal.' : 'English idézet'}</p>
                </div>
                <div className="col-md-4">
                  <h2><img src={Arpi} style={{width: '30px', height: '30px', borderRadius: '50%'}}></img>{language === "hu" ? 'Veller Árpád' : 'Árpád Veller'}</h2>
                  <p>{language === "hu" ? 'Az idő pénz, a pénz beszél a kutya ugat. Megy mint a ágybahugyozás.' : 'English idézet'}</p>
                </div>
                <div className="col-md-4">
                  <h2><img src={Balow} style={{width: '30px', height: '30px', borderRadius: '50%'}}></img>{language === "hu" ? 'Csanálosi Bálint' : 'Bálint Csanálosi'}</h2>
                  <p>{language === "hu" ? 'Kicsi a bors, de erős. Jobb száz irigy mint egy zsidó.' : 'English idézet'}</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Bejelentkezés Modal */}
      <div
        className="modal fade"
        id="loginModal"
        tabIndex="-1"
        aria-labelledby="loginModalLabel"
        aria-hidden="true"
      >
        <div className="modal-dialog modal-dialog-centered">
          <div className="modal-content">
            <div className="modal-header">
              <h5 className="modal-title" id="loginModalLabel">{texts[language].login}</h5>
            </div>
            <div className="modal-body">
              <form>
                <div className="mb-3">
                  <label htmlFor="username" className="form-label">{language === "hu" ? 'Felhasználónév' : 'Username'}</label>
                  <input type="text" className="form-control" id="username" placeholder={language === "hu" ? 'Felhasználónév' : 'Username'} />
                </div>
                <div className="mb-3">
                  <label htmlFor="password" className="form-label">{language === "hu" ? 'Jelszó' : 'Password'}</label>
                  <input type="password" className="form-control" id="password" placeholder={language === "hu" ? 'Jelszó' : 'Password'} />
                </div>
                <a onClick={handleLogin} data-bs-dismiss="modal" className="btn btn-secondary">{texts[language].login}</a>
              </form>
            </div>
          </div>
        </div>
      </div>

      {/* Regisztráció Modal */}
      <div
        className="modal fade"
        id="registerModal"
        tabIndex="-1"
        aria-labelledby="registerModalLabel"
        aria-hidden="true"
      >
        <div className="modal-dialog modal-dialog-centered">
          <div className="modal-content">
            <div className="modal-header">
              <h5 className="modal-title" id="registerModalLabel">{texts[language].register}</h5>
            </div>
            <div className="modal-body">
              <form>
                <div className="mb-3">
                  <label htmlFor="registerUsername" className="form-label">{language === "hu" ? 'Felhasználónév' : 'Username'}</label>
                  <input type="text" className="form-control" id="registerUsername" placeholder={language === "hu" ? 'Felhasználónév' : 'Username'} />
                </div>
                <div className="mb-3">
                  <label htmlFor="email" className="form-label">Email</label>
                  <input type="email" className="form-control" id="email" placeholder="Email" />
                </div>
                <div className="mb-3">
                  <label htmlFor="registerPassword" className="form-label">{language === "hu" ? 'Jelszó' : 'Password'}</label>
                  <input type="password" className="form-control" id="registerPassword" placeholder={language === "hu" ? 'Jelszó' : 'Password'} />
                </div>
                <div className="mb-3">
                  <label htmlFor="registerPasswordAgain" className="form-label">{language === "hu" ? 'Jelszó újra' : 'Password again'}</label>
                  <input type="password" className="form-control" id="registerPasswordAgain" placeholder={language === "hu" ? 'Jelszó újra' : 'Password again'}/>
                </div>
                <a className="btn btn-secondary" onClick={() => setIsLoggedIn(true)} data-bs-dismiss="modal">{texts[language].register}</a>
              </form>
            </div>
          </div>
        </div>
      </div>

      {/* Beállítások Modal */}
      <div
  className="modal fade"
  id="settingsModal"
  tabIndex="-1"
  aria-labelledby="settingsModalLabel"
  aria-hidden="true"
>
  <div className="modal-dialog modal-dialog-centered">
    <div className="modal-content">
      <div className="modal-header">
        <h5 className="modal-title" id="settingsModalLabel">{texts[language].settings}</h5>
      </div>
      <div className="modal-body">
        <form>
          <div className="mb-3">
            <label htmlFor="newName" className="form-label">{language === "hu" ? 'Új felhasználónév' : 'New username'}</label>
            <input type="text" className="form-control" id="newName" placeholder={language === "hu" ? 'Új felhasználónév' : 'New username'} />
          </div>
          <div className="mb-3">
            <label htmlFor="newEmail" className="form-label">{language === "hu" ? 'Új email' : 'New email'}</label>
            <input type="email" className="form-control" id="newEmail" placeholder={language === "hu" ? 'Új email' : 'New email'} />
          </div>
          <div className="mb-3">
            <label htmlFor="newPassword" className="form-label">{language === "hu" ? 'Új jelszó' : 'New password'}</label>
            <input type="password" className="form-control" id="newPassword" placeholder={language === "hu" ? 'Új jelszó' : 'New password'} />
          </div>
          <div className="d-flex justify-content-end">
            <a className="btn btn-secondary" data-bs-dismiss="modal">{language === "hu" ? 'Módosítás' : 'Modify'}</a>
            <a
              className="btn btn-danger ms-2"
              data-bs-dismiss="modal"
              onClick={() => {
                if (window.confirm("Biztos törölni akarod?")) {
                  console.log("Törlés megtörtént!");
                } else {
                  console.log("Törlés megszakítva.");
                }
              }}
            >
              {language === "hu" ? 'Törlés' : 'Delete'}
            </a>
          </div>
        </form>
      </div>
    </div>
  </div>
</div>

      {/* Admin Modal */}
<div
  className="modal fade"
  id="adminModal"
  tabIndex="-1"
  aria-labelledby="adminModalLabel"
  aria-hidden="true"
>
  <div className="modal-dialog modal-dialog-centered">
    <div className="modal-content">
      <div className="modal-header">
        <h5 className="modal-title" id="adminModalLabel">{language === "hu" ? 'Adminisztrációs beállítások' : 'Admin settings'}</h5>
        <button
          type="button"
          className="btn-close"
          data-bs-dismiss="modal"
          aria-label="Close"
        ></button>
      </div>
      <div className="modal-body">
        <form>
          {/* Debug mód beállítás */}
          <div className="form-check mb-3">
            <input
              className="form-check-input"
              type="checkbox"
              id="debugMode"
            />
            <label className="form-check-label" htmlFor="debugMode">
            {language === "hu" ? 'Debug mód engedélyezése' : 'Enable debug mode'}
            </label>
          </div>

          {/* Új admin jelszó generálása */}
              <div className="mb-3">
                <label htmlFor="newAdminPassword" className="form-label">
                {language === "hu" ? 'Új admin jelszó' : 'New admin password'}
                </label>
                <input
                  type="text"
                  style={{margin:"5px", width:"90px"}}
                  value={code}
                  className="form-control"
                  id="newAdminPassword"
                  readOnly
                />
                <label>
                  {language === "hu" ? 'Az új jelszó változni fog: ' : 'New password in: '}{secondsLeft}
                </label>
              </div>
            </form>
          </div>
          <div className="modal-footer">
            <a
              className="btn btn-danger"
              data-bs-dismiss="modal"
            >
              {language === "hu" ? 'Bezárás' : 'Close'}
            </a>
          </div>
        </div>
      </div>
    </div>
    </nav>
    </>
  );
}
