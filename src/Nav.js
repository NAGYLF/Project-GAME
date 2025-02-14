import React from 'react';
import { Link } from 'react-router-dom'; // Import Link from react-router-dom
import Video from './video/video.mp4';
import Logo from './img/logo.png';
import Kep from './img/profilkep.jpg';
import './Nav.css';

function Nav({ language, setLanguage, texts, isLoggedIn, setIsLoggedIn, isAdmin, username, logout }) {
  const handleLanguageChange = (e) => {
    setLanguage(e.target.value);
  };


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
              <Link to="/description" className="nav-item nav-link active">
                {texts[language].description}
              </Link>
              <Link to="/about" className="nav-item nav-link active">
                {texts[language].about}
              </Link>
              <Link to="/search" className="nav-item nav-link active">
                {texts[language].search}
              </Link>
              <select 
                value={language} 
                onChange={handleLanguageChange} 
                id='languagechanger' 
                className='form-select'
                style={{ borderRadius:"5px",minWidth:"100px", cursor: "pointer", textAlign:"left"}}
              >
                <option value="hu">Magyar</option>
                <option value="en">English</option>
              </select>
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
                        <button style={{cursor: "pointer"}} onClick={logout} className="dropdown-item">
                        {texts[language].signout}
                        </button>
                      </li>
                    </ul>
                  </div>
                ) : (
                  <>
                  <Link to="/login" className="nav-item nav-link active" id='logreg'>
                  {texts[language].login}
                  </Link>
                  <Link to="/register" className="nav-item nav-link active" id='logreg'>
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