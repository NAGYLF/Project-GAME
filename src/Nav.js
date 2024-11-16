import React, { useState } from 'react';
import Kep from './img/profilkep.jpg';
import './Nav.css';

export default function Nav() {
  let eldont = false;
  const [isDescriptionVisible, setIsDescriptionVisible] = useState(false);
  const [isAboutVisible, setIsAboutVisible] = useState(false);
  const [isLoginVisible, setIsLoginVisible] = useState(false);
  const [isRegisterVisible, setIsRegisterVisible] = useState(false);
  const [isSettingsVisible, setIsSettingsVisible] = useState(false);

  // Bejelentkezéshez
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');

  const handleLoginSubmit = (e) => {
    e.preventDefault();
    alert(`Bejelentkezés: ${username}, Jelszó: ${password}`);
  };

  // Regisztrációhoz
  const [registerUsername, setRegisterUsername] = useState('');
  const [registerPassword, setRegisterPassword] = useState('');
  const [email, setEmail] = useState('');

  const handleRegisterSubmit = (e) => {
    e.preventDefault();
    alert(`Regisztráció: Felhasználónév: ${registerUsername}, Email: ${email}, Jelszó: ${registerPassword}`);
  };

  // Beállításokhoz
  const [newName, setNewName] = useState('');
  const [newEmail, setNewEmail] = useState('');
  const [newPassword, setNewPassword] = useState('');

  const toggleDescription = () => {
    setIsDescriptionVisible(true);
  };

  const closeDescription = () => {
    setIsDescriptionVisible(false);
  };

  const toggleAbout = () => {
    setIsAboutVisible(true);
  };

  const closeAbout = () => {
    setIsAboutVisible(false);
  };

  const openLogin = () => {
    setIsLoginVisible(true);
  };

  const closeLogin = () => {
    setIsLoginVisible(false);
  };

  const openRegister = () => {
    setIsRegisterVisible(true);
  };

  const closeRegister = () => {
    setIsRegisterVisible(false);
  };

  const openSettings = () => {
    setIsSettingsVisible(true);
  };

  const closeSettings = () => {
    setIsSettingsVisible(false);
  };

  if (!isDescriptionVisible && !isAboutVisible && !isLoginVisible && !isRegisterVisible && !isSettingsVisible) {
    eldont = false;
  } else {
    eldont = true;
  }

  return (
    <nav className="navbar navbar-expand-lg custom-navbar">
      <div style={{ display: eldont ? 'none' : 'block' }} className="container-fluid">
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
              <a className="nav-link" onClick={toggleDescription}>
                Leírás
              </a>
            </li>
            <li className="nav-item">
              <a className="nav-link" onClick={toggleAbout}>
                Rólunk
              </a>
            </li>
            <li className="nav-item">
              <a className="nav-link disabled" href="#" aria-disabled="true">
                Letöltés
              </a>
            </li>
          </ul>

          <form className="d-flex ms-auto align-items-center">
            <div className="dropdown">
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
                <li>
                  <a className="dropdown-item" onClick={openSettings}>
                    Beállítások
                  </a>
                </li>
                <li>
                  <a className="dropdown-item" href="#logout">
                    Kijelentkezés
                  </a>
                </li>
              </ul>
            </div>

            <button className="btn btn-outline-light ms-2" onClick={openLogin}>
              Bejelentkezés
            </button>
            <button className="btn btn-outline-light ms-2" onClick={openRegister}>
              Regisztráció
            </button>
          </form>
        </div>
      </div>

      {/* Leírás menüpont */}
      <div
        className="centered-div"
        style={{ display: isDescriptionVisible ? 'block' : 'none' }}
      >
        <button className="close-btn" onClick={closeDescription}>
          X
        </button>
        Lorem ipsum dolor sit amet, consectetur adipiscing elit...
      </div>

      {/* Rólunk menüpont */}
      <div
        className="container centered-div"
        style={{ display: isAboutVisible ? 'block' : 'none' }}
      >
        <button className="close-btn" onClick={closeAbout}>
          X
        </button>
        <div className="row row text-center align-items-center">
          <div className="col-4">
            <h1>Nagy Levente Ferenc</h1>
            <p>Aenean vel orci et purus faucibus consectetur...</p>
          </div>
          <div className="col-4">
            <h1>Veller Árpád</h1>
            <p>Aenean vel orci et purus faucibus consectetur...</p>
          </div>
          <div className="col-4">
            <h1>Csanálosi Bálint</h1>
            <p>Aenean vel orci et purus faucibus consectetur...</p>
          </div>
        </div>
      </div>

      {/* Bejelentkezés */}
      <div
        className="centered-div"
        style={{ display: isLoginVisible ? 'block' : 'none' }}
      >
        <button className="close-btn" onClick={closeLogin}>
          X
        </button>
        <h2 className="text-center mb-4">Bejelentkezés</h2>
        <form onSubmit={handleLoginSubmit}>
          <div className="mb-3">
            <label htmlFor="username" className="form-label">
              Felhasználónév
            </label>
            <input
              type="text"
              className="form-control"
              id="username"
              placeholder="Felhasználónév"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
            />
          </div>
          <div className="mb-3">
            <label htmlFor="password" className="form-label">
              Jelszó
            </label>
            <input
              type="password"
              className="form-control"
              id="password"
              placeholder="Jelszó"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
          </div>
          <button type="submit" className="btn btn-primary">
            Bejelentkezés
          </button>
        </form>
      </div>

      {/* Regisztráció */}
      <div
        className="centered-div"
        style={{ display: isRegisterVisible ? 'block' : 'none' }}
      >
        <button className="close-btn" onClick={closeRegister}>
          X
        </button>
        <h2 className="text-center mb-4">Regisztráció</h2>
        <form onSubmit={handleRegisterSubmit}>
          <div className="mb-3">
            <label htmlFor="registerUsername" className="form-label">
              Felhasználónév
            </label>
            <input
              type="text"
              className="form-control"
              id="registerUsername"
              placeholder="Felhasználónév"
              value={registerUsername}
              onChange={(e) => setRegisterUsername(e.target.value)}
            />
          </div>
          <div className="mb-3">
            <label htmlFor="email" className="form-label">
              Email
            </label>
            <input
              type="email"
              className="form-control"
              id="email"
              placeholder="Email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />
          </div>
          <div className="mb-3">
            <label htmlFor="registerPassword" className="form-label">
              Jelszó
            </label>
            <input
              type="password"
              className="form-control"
              id="registerPassword"
              placeholder="Jelszó"
              value={registerPassword}
              onChange={(e) => setRegisterPassword(e.target.value)}
            />
          </div>
          <button type="submit" className="btn btn-primary">
            Regisztráció
          </button>
        </form>
      </div>

      {/* Beállítások */}
      <div
        className="centered-div"
        style={{ display: isSettingsVisible ? 'block' : 'none' }}
      >
        <button className="close-btn" onClick={closeSettings}>
          X
        </button>
        <h2 className="text-center mb-4">Beállítások</h2>
        <form>
          <div className="mb-3">
            <label htmlFor="newName" className="form-label">
              Új név
            </label>
            <input
              type="text"
              className="form-control"
              id="newName"
              placeholder="Új név"
              value={newName}
              onChange={(e) => setNewName(e.target.value)}
            />
          </div>
          <div className="mb-3">
            <label htmlFor="newEmail" className="form-label">
              Új email
            </label>
            <input
              type="email"
              className="form-control"
              id="newEmail"
              placeholder="Új email"
              value={newEmail}
              onChange={(e) => setNewEmail(e.target.value)}
            />
          </div>
          <div className="mb-3">
            <label htmlFor="newPassword" className="form-label">
              Új jelszó
            </label>
            <input
              type="password"
              className="form-control"
              id="newPassword"
              placeholder="Új jelszó"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
            />
          </div>
          <div className="d-flex justify-content-end">
          <button type="button" className="btn btn-primary">
              Módosítás
            </button>
            <button type="button" className="btn btn-danger">
              Töröl
            </button>
          </div>
        </form>
      </div>
    </nav>
  );
}
