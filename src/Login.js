import axios from 'axios';
import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { jwtDecode } from 'jwt-decode';  // Helyes importálás

function Login({ language, texts, setIsLoggedIn, setIsAdmin }) {
  const location = useLocation();
  const navigate = useNavigate();

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const login = (email, password) => {
    let user = {
      email: email,
      password: password
    };
    console.log(user);
    

    axios.post('http://localhost:5269/api/auth/login', user).then((response) => {
      if (response.data) {
        const token = response.data.token; // JWT token a válaszból
        localStorage.setItem('token', token); // Token mentése localStorage-ba

        // Token dekódolása
        const decodedToken = jwtDecode(token);
        const isAdmin = decodedToken.IsAdmin;
        console.log(decodedToken);

        // Állapotok beállítása
        setIsLoggedIn(true);
        setIsAdmin(isAdmin);  // Az admin státusz beállítása

        // A felhasználót átirányítjuk a főoldalra
        navigate("/home");
      } else {
        alert(response.data.message);
      }
    }).catch((error) => {
      console.error("Login failed:", error);
      alert("Bejelentkezés hiba!");
    });
  };

  const logout = () => {
    localStorage.deleteItem('token');
    console.log(localStorage.getItem('token'));
    setIsLoggedIn(false);
    setIsAdmin(false);
    navigate("/home");
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
            <form>
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
            <button type="submit" className="btn btn-light" onClick={() => login(email, password)}>
              {texts[language].login}
            </button>
            <button
              type="button"
              className="btn btn-danger"
              data-bs-dismiss="modal"
              onClick={logout}
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
