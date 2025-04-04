import React from 'react';
import { Link } from 'react-router-dom';
import Video from './video/video.mp4';
import Logo from './img/logo.png';
import Kep from './img/profilkep.jpg';
import AdminKep from './img/admin.png';
import { useLocation } from 'react-router-dom';
import './Nav.css';

function Nav({ language, setLanguage, texts, isLoggedIn, isAdmin, logout }) {
  const handleLanguageChange = (e) => {
    setLanguage(e.target.value);
  };
  const location = useLocation();

  return (
    <div>
      <video
        className="video-background"
        autoPlay
        muted
        loop
      >
        <source src={Video} type="video/mp4" />
        A böngésző nem támogatja a videó lejátszást.
      </video>
      <nav className="navbar navbar-expand-lg navbar-dark custom-nav">
        <div className="container-fluid">
          <button
            className="navbar-toggler"
            type="button"
            data-bs-toggle="collapse"
            data-bs-target="#navbarNavAltMarkup"
            aria-controls="navbarNavAltMarkup"
            aria-expanded="false"
            aria-label="Toggle navigation"
          >
            <span className="navbar-toggler-icon"></span>
          </button>

          <div className="collapse navbar-collapse justify-content-center" id="navbarNavAltMarkup">
            <div className="navbar-nav">
              <Link to="/description" className="nav-item nav-link active" id={location.pathname === "/description" ? "current" : null}>
                {texts[language].description}
              </Link>
              <Link to="/about" className="nav-item nav-link active" id={location.pathname === "/about" ? "current" : null}>
                {texts[language].about}
              </Link>
              <Link to="/search" className="nav-item nav-link active" id={location.pathname === "/search" ? "current" : null}>
                {texts[language].search}
              </Link>
              <select
                value={language}
                onChange={handleLanguageChange}
                id='languagechanger'
                className='form-select'
                style={{ borderRadius: "5px", minWidth: "100px", cursor: "pointer", textAlign: "left" }}
              >
                <option value="hu">Magyar</option>
                <option value="en">English</option>
              </select>
              {/*isLoggedIn ?
                <a id='download' href="https://drive.google.com/file/d/1Bb0RqMSQm7dDXpHNBP1v6C8h2OqInu9t/view" target='_blank' style={{
                  backgroundColor: "rgb(10,10,10)",
                  borderRadius: "5px",
                  color: "azure",
                  width: "90px!important",
                  textAlign: "center",
                  margin: "0 5px",
                  transition: "0.5s",
                  padding: "6.5px",
                  fontSize: "14px"
                }}>
                  <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="currentColor" className="bi bi-download" viewBox="0 0 16 16">
                    <path d="M.5 9.9a.5.5 0 0 1 .5.5v2.5a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1v-2.5a.5.5 0 0 1 1 0v2.5a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2v-2.5a.5.5 0 0 1 .5-.5" />
                    <path d="M7.646 11.854a.5.5 0 0 0 .708 0l3-3a.5.5 0 0 0-.708-.708L8.5 10.293V1.5a.5.5 0 0 0-1 0v8.793L5.354 8.146a.5.5 0 1 0-.708.708z" />
                  </svg>
                </a> : null
              */}
            </div>

            <Link className="navbar-brand mx-auto" to="/">
              <img src={Logo} alt="Logo" id="logo" className="center-logo" style={{ height: '50px' }} />
            </Link>

            <div className="navbar-nav ms-auto" id="rightnav">
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
                        position: 'relative'
                      }}
                    >
                      <img
                        id='kep'
                        src={Kep}
                        alt="Profil"
                        style={{ width: '30px', height: '30px', borderRadius: '50%' }}
                      />
                      {isAdmin ? <img src={AdminKep} alt='badge' id="badge" className="center-badge"></img> : null}
                    </button>
                    <ul className="dropdown-menu" aria-labelledby="dropdownMenuButton" style={{ position: 'absolute', top: '100%' }}>
                      {isAdmin && (
                        <li>
                          <Link className="dropdown-item" to="/admin">
                            Admin
                          </Link>
                        </li>
                      )}
                      <li>
                        <Link className="dropdown-item" to="/settings">
                          {texts[language].settings}
                        </Link>
                      </li>
                      <li>
                        <button style={{ cursor: "pointer" }} onClick={logout} className="dropdown-item">
                          {texts[language].signout}
                        </button>
                      </li>
                    </ul>
                  </div>
                ) : (
                  <>
                    <Link to="/login" className="nav-item nav-link active" id="logreg">
                      {texts[language].login}
                    </Link>
                    <Link to="/register" className="nav-item nav-link active" id="logreg">
                      {texts[language].register}
                    </Link>
                  </>
                )}
              </form>
            </div>
          </div>
        </div>
      </nav>
    </div>
  );
}

export default Nav;