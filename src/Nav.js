import React, { useState } from 'react';
import Kep from './img/profilkep.jpg';
import Arpi from './img/arpi.png'; 
import Balow from './img/balow.png';
import Levi from './img/levi.png';
import './Nav.css'

export default function Nav() {

  const [isAdmin, setIsAdmin] = useState(true);
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  return (
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
                Leírás
              </a>
            </li>
            <li className="nav-item">
              <a className="nav-link" href="#aboutModal" data-bs-toggle="modal">
                Rólunk
              </a>
            </li>
            {isLoggedIn ? <li className="nav-item">
              <a className="nav-link">
                Letöltés
              </a>
            </li> : ""}
          </ul>

          <form className="d-flex ms-auto align-items-center text-center">
            {isLoggedIn ? <div className="dropstart">
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
                {isAdmin ? <li>
                  <a className="dropdown-item" href="#adminModal" data-bs-toggle="modal">
                    Admin
                  </a>
                </li> : ""}
                <li>
                  <a className="dropdown-item" href="#settingsModal" data-bs-toggle="modal">
                    Beállítások
                  </a>
                </li>
                <li>
                  <a onClick={() => setIsLoggedIn(false)} className="dropdown-item">
                    Kijelentkezés
                  </a>
                </li>
              </ul>
            </div> : <><a className="btn btn-outline-light ms-2" data-bs-toggle="modal" data-bs-target="#loginModal">
              Bejelentkezés
            </a>
            <a className="btn btn-outline-light ms-2" data-bs-toggle="modal" data-bs-target="#registerModal">
              Regisztráció
            </a></>}
            


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
              <h5 className="modal-title" id="descriptionModalLabel">Leírás</h5>
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
              <h5 className="modal-title" id="aboutModalLabel">Rólunk</h5>
            </div>
            <div className="modal-body">
              <div className="row text-center align-items-center">
                <div className="col-md-4">
                  <h2><img src={Levi} style={{width: '30px', height: '30px', borderRadius: '50%'}}></img>Nagy Levente Ferenc</h2>
                  <p>A cél szentesíti az eszközt. Ha megdobnak kővel dobd vissza kézigránáttal.</p>
                </div>
                <div className="col-md-4">
                  <h2><img src={Arpi} style={{width: '30px', height: '30px', borderRadius: '50%'}}></img>Veller Árpád</h2>
                  <p>Az idő pénz, a pénz beszél a kutya ugat. Megy mint a ágybahugyozás.</p>
                </div>
                <div className="col-md-4">
                  <h2><img src={Balow} style={{width: '30px', height: '30px', borderRadius: '50%'}}></img>Csanálosi Bálint</h2>
                  <p>Kicsi a bors, de erős. Jobb száz irigy mint egy zsidó.</p>
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
              <h5 className="modal-title" id="loginModalLabel">Bejelentkezés</h5>
            </div>
            <div className="modal-body">
              <form>
                <div className="mb-3">
                  <label htmlFor="username" className="form-label">Felhasználónév</label>
                  <input type="text" className="form-control" id="username" placeholder="Felhasználónév" />
                </div>
                <div className="mb-3">
                  <label htmlFor="password" className="form-label">Jelszó</label>
                  <input type="password" className="form-control" id="password" placeholder="Jelszó" />
                </div>
                <a onClick={() => setIsLoggedIn(true)} data-bs-dismiss="modal" className="btn btn-secondary">Bejelentkezés</a>
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
              <h5 className="modal-title" id="registerModalLabel">Regisztráció</h5>
            </div>
            <div className="modal-body">
              <form>
                <div className="mb-3">
                  <label htmlFor="registerUsername" className="form-label">Felhasználónév</label>
                  <input type="text" className="form-control" id="registerUsername" placeholder="Felhasználónév" />
                </div>
                <div className="mb-3">
                  <label htmlFor="email" className="form-label">Email</label>
                  <input type="email" className="form-control" id="email" placeholder="Email" />
                </div>
                <div className="mb-3">
                  <label htmlFor="registerPassword" className="form-label">Jelszó</label>
                  <input type="password" className="form-control" id="registerPassword" placeholder="Jelszó" />
                </div>
                <a className="btn btn-secondary" onClick={() => setIsLoggedIn(true)} data-bs-dismiss="modal">Regisztráció</a>
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
        <h5 className="modal-title" id="settingsModalLabel">Beállítások</h5>
      </div>
      <div className="modal-body">
        <form>
          <div className="mb-3">
            <label htmlFor="newName" className="form-label">Új név</label>
            <input type="text" className="form-control" id="newName" placeholder="Új név" />
          </div>
          <div className="mb-3">
            <label htmlFor="newEmail" className="form-label">Új email</label>
            <input type="email" className="form-control" id="newEmail" placeholder="Új email" />
          </div>
          <div className="mb-3">
            <label htmlFor="newPassword" className="form-label">Új jelszó</label>
            <input type="password" className="form-control" id="newPassword" placeholder="Új jelszó" />
          </div>
          <div className="d-flex justify-content-end">
            <a className="btn btn-secondary" data-bs-dismiss="modal">Módosítás</a>
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
              Töröl
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
        <h5 className="modal-title" id="adminModalLabel">Adminisztrációs Beállítások</h5>
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
              Debug mód engedélyezése
            </label>
          </div>

          {/* Új admin jelszó generálása */}
              <div className="mb-3">
                <label htmlFor="newAdminPassword" className="form-label">
                  Új admin jelszó
                </label>
                <input
                  type="text"
                  className="form-control"
                  id="newAdminPassword"
                  placeholder="Generált jelszó itt fog megjelenni"
                  readOnly
                />
              </div>
              <button type="button" className="btn btn-secondary">
                Jelszó generálása
              </button>
            </form>
          </div>
          <div className="modal-footer">
            <a
              className="btn btn-danger"
              data-bs-dismiss="modal"
            >
              Bezárás
            </a>
          </div>
        </div>
      </div>
    </div>
    </nav>
  );
}
