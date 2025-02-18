import React, { useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';

function Login({ language, texts, login, showAlert }) {
  const location = useLocation();
  const navigate = useNavigate();

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

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
              onClick={() => navigate("/")}
            ></button>
          </div>
          <div className="modal-body">
            <form onSubmit={(e) => {
              e.preventDefault();
              login(email, password, showAlert);
              navigate("/");
            } }>
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
              <button type="submit" className="btn btn-light">
              {texts[language].login}
            </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Login;
