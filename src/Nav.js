import React, { useState } from 'react'; 
import Kep from './img/profilkep.jpg';
import './Nav.css';

export default function Nav() {
  const [isDescriptionVisible, setIsDescriptionVisible] = useState(false);

  const toggleDescription = () => {
    setIsDescriptionVisible(true);
  };

  const closeDescription = () => {
    setIsDescriptionVisible(false);
  };

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
              <a className="nav-link" onClick={toggleDescription}>Leírás</a>
            </li>
            <li className="nav-item">
              <a className="nav-link" href="#">Rólunk</a>
            </li>
            <li className="nav-item">
              <a className="nav-link disabled" href="#" aria-disabled="true">Letöltés</a>
            </li>
          </ul>

          <form className="d-flex ms-auto align-items-center">
            <a id="pic" className="navbar-brand" href="#">
              <img src={Kep} alt="Profilkép" width="30" height="30" />
            </a>
            <div className="dropdown">
              <button
                className="btn btn-secondary dropdown-toggle"
                type="button"
                data-bs-toggle="dropdown"
                aria-expanded="false"
              >
                Profil
              </button>
              <ul className="dropdown-menu">
                <li><a className="dropdown-item" href="#">Statisztikák</a></li>
                <li><a className="dropdown-item" href="#">Profil módosítása</a></li>
              </ul>
            </div>
            <button className="btn btn-outline-light ms-2">Bejelentkezés</button>
            <button className="btn btn-outline-light ms-2">Regisztráció</button>
          </form>
        </div>
      </div>

      <div className="centered-div" style={{ display: isDescriptionVisible ? 'block' : 'none' }}>
        <button className="close-btn" onClick={closeDescription}>X</button>
        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin vel nunc sit amet lacus euismod accumsan vitae ac nisi. Duis sit amet enim metus. Donec fringilla vitae nulla non sodales. In venenatis elit a arcu venenatis luctus eget eget justo. Integer arcu odio, euismod nec eros sed, ultricies tincidunt tortor. Curabitur pharetra viverra leo vel tincidunt. Duis vel arcu lorem. Donec tristique tellus in augue tempus tristique.

        Aenean vel orci et purus faucibus consectetur nec ut ligula. Sed et sem facilisis erat ornare pretium.
        Duis sit amet nisl quis libero maximus maximus. Maecenas ullamcorper odio quis posuere hendrerit. Morbi vehicula nulla sollicitudin, mattis nisl id, rutrum nisi. Quisque eu cursus augue. Sed malesuada nulla eu elit lobortis posuere. Proin bibendum ultricies magna non fringilla. Vivamus aliquet ligula neque. Morbi suscipit laoreet nunc, non suscipit justo convallis sed. Morbi commodo aliquet viverra. Nunc id ante faucibus, sagittis elit eu, semper enim.
      </div>
    </nav>
  );
}
