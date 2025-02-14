import React, { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import axios from "axios";

function Register({ language, code, login}) {
  const location = useLocation();
  const navigate = useNavigate();

  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [passwordAgain, setPasswordAgain] = useState("");
  const [adminCode, setAdminCode] = useState("");

  return (
    <div
      className={`modal fade ${location.pathname === "/register" ? "show" : ""}`}
      id="registerModal"
      tabIndex="-1"
      role="dialog"
      aria-labelledby="registerModalLabel"
      aria-hidden={location.pathname !== "/register" ? "true" : "false"}
      style={{ display: location.pathname === "/register" ? "block" : "none" }}
    >
      <div className="modal-dialog modal-dialog-centered" role="document">
        <div className="modal-content bg-dark text-white">
          <div className="modal-header">
            <h5 className="modal-title" id="registerModalLabel">
              {language === "hu" ? "Regisztráció" : "Register"}
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
            <form onSubmit={(e) => {
              e.preventDefault();
              if(password === passwordAgain) {
                const isAdmin = adminCode === code;
                axios.post("http://localhost:5269/api/auth/register", { 
                  name: username,
                  password: password,
                  email: email,
                  isAdmin: isAdmin
                })
                .then(() => {
                  login(email, password);
                  navigate("/");
                })
              }
              
            }}>
              <div className="mb-3">
                <label htmlFor="registerUsername" className="form-label">
                  {language === "hu" ? "Felhasználónév" : "Username"}
                </label>
                <input
                  type="text"
                  className="form-control"
                  id="registerUsername"
                  value={username}
                  onChange={(e) => setUsername(e.target.value)}
                  required
                />
              </div>
              <div className="mb-3">
                <label htmlFor="email" className="form-label">
                  {language === "hu" ? "Email" : "Email"}
                </label>
                <input
                  type="email"
                  className="form-control"
                  id="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                />
              </div>
              <div className="mb-3">
                <label htmlFor="registerPassword" className="form-label">
                  {language === "hu" ? "Jelszó" : "Password"}
                </label>
                <input
                  type="password"
                  className="form-control"
                  id="registerPassword"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                />
              </div>
              <div className="mb-3">
                <label htmlFor="registerPasswordAgain" className="form-label">
                  {language === "hu" ? "Jelszó újra" : "Password again"}
                </label>
                <input
                  type="password"
                  className="form-control"
                  id="registerPasswordAgain"
                  value={passwordAgain}
                  onChange={(e) => setPasswordAgain(e.target.value)}
                  required
                />
              </div>
              <div className="mb-3">
                <label htmlFor="adminCode" className="form-label">
                  {language === "hu" ? "Admin kód (opcionális)" : "Admin code (optional)"}
                </label>
                <input
                  type="text"
                  className="form-control"
                  id="adminCode"
                  value={adminCode}
                  onChange={(e) => setAdminCode(e.target.value)}
                />
              </div>
              <div style={{ display: "flex", justifyContent: "space-between" }}>
                <button type="submit" className="btn btn-light">
                {language === "hu" ? "Regisztráció" : "Register"}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Register;
