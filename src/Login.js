import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';

function Login({ language, texts, setIsLoggedIn, setIsAdmin }) {
  const location = useLocation();
  const navigate = useNavigate();

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  useEffect(() => {
    if (location.pathname !== "/login") {
      // If we navigate away from /login, modal should hide automatically
      // You could add more logic to reset form or pesrform any other action
    }
  }, [location.pathname]);

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
        setEmail(player.name);
        setIsLoggedIn(true);
        if (player.isAdmin === 1) {
          setIsAdmin(true);
        }
      })
      .catch(error => {
        console.error('Hiba történt:', error);
        setIsLoggedIn(false);
      });
  }

  const handleLogin = (e) => {
    e.preventDefault();
    getPlayerByNameAndPassword(email, password);
  };

  const handleAdditionalEvent = () => {
    if (email && password) {
      navigate("/home");
    } else {
      alert("Please fill in all fields.");
    }
  };

  const handleButtonClick = (e) => {
    handleLogin(e);
    handleAdditionalEvent();
  };

  return (
    <div
      className={`modal fade ${location.pathname === "/login" ? "show" : ""}`}
      id="loginModal"
      tabIndex="-1"
      role="dialog"
      aria-labelledby="loginModalLabel"
      aria-hidden={location.pathname !== "/login" ? "true" : "false"}
      style={{ display: location.pathname === "/login" ? "block" : "none" }}
    >
      <div className="modal-dialog modal-dialog-centered" role="document">
        <div className="modal-content bg-dark text-white">
          <div className="modal-header">
            <h5 className="modal-title" id="loginModalLabel">
              {texts[language].login}
            </h5>
            <button
              type="button"
              className="btn-close bg-light"
              data-bs-dismiss="modal"
              aria-label="Close"
              onClick={() => navigate("/home")}
            ></button>
          </div>
          <div className="modal-body">
            <form onSubmit={handleLogin}>
              <div className="mb-3">
                <label htmlFor="email" className="form-label">
                  {language === "hu" ? 'Email cím' : 'Email'}
                </label>
                <input
                  type="text"
                  className="form-control"
                  id="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                />
              </div>
              <div className="mb-3">
                <label htmlFor="password" className="form-label">
                  {language === "hu" ? 'Jelszó' : 'Password'}
                </label>
                <input
                  type="password"
                  className="form-control"
                  id="password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                />
              </div>
            </form>
          </div>
          <div className="modal-footer">
            <button type="submit" className="btn btn-light" onClick={handleButtonClick}>
              {texts[language].login}
            </button>
            <button
              type="button"
              className="btn btn-danger"
              data-bs-dismiss="modal"
              onClick={() => navigate("/home")}
            >
              {language === "hu" ? "Bezárás" : "Close"}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Login;