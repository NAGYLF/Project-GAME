import React, { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import axios from "axios";

function Register({ language, code, login, admincode, showAlert }) {
  const location = useLocation();
  const navigate = useNavigate();

  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [passwordAgain, setPasswordAgain] = useState("");
  const [adminCode, setAdminCode] = useState("");

  useEffect(() => {
    admincode();
  }, []);

  const sendEmail = (email) => {
    const sendingEmail = {
      to: email,
      subject: `${language === "hu" ? "Regisztráció" : "Registration"}`,
      body: `
        <html>
          <head>
            <style>
              body {
                font-family: Arial, sans-serif;
                color: #333;
                background-color: #f4f4f4;
                margin: 0;
                padding: 20px;
              }
              .email-container {
                background-color: black;
                border-radius: 16px;
                padding: 20px;
                box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                text-align: center;
                margin: auto;
                width: 75%;
              }
              .email-header {
                font-size: 20px;
                font-weight: bold;
                color: #28a745;
              }
              .email-content {
                font-size: 16px;
                line-height: 1.5;
                color: azure;
                margin-top: 50px;
              }
              .email-footer {
                font-size: 12px;
                color: #888;
                margin-top: 60px;
              }
            </style>
          </head>
          <body>
            <div class="email-container">
              <div class="email-header">
                ${language === "hu" ? "Fiók regisztráció sikeres" : "Account Registration Successful"}
              </div>
              <div class="email-content">
                ${language === "hu" ? `Fiókját ${username} néven sikeresen regisztráltuk.` : `Your account named ${username} has been successfully registered!`}
              </div>
              <div class="email-footer">
                ${language === "hu" ? "Üdvözlettel, a csapat" : "Best regards, the team"}
              </div>
            </div>
          </body>
        </html>
      `
    };
  
    axios.post('http://localhost:5269/api/email', sendingEmail, {
      headers: {
        'Content-Type': 'application/json'
      }
    })
      .then(response => {
        console.log('Email sent successfully');
      })
      .catch(error => {
        console.error('Error sending email', error);
      });
  };

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
              onClick={() => navigate("/")}
            ></button>
          </div>
          <div className="modal-body">
            <form onSubmit={(e) => {
              e.preventDefault();
              if (password === passwordAgain) {
                console.log(adminCode);
                console.log(code);
                const isAdmin = adminCode == code;
                axios.post("http://localhost:5269/api/auth/register", { 
                  name: username,
                  password: password,
                  email: email,
                  isAdmin: isAdmin
                })
                .then(() => {
                  showAlert(language === "hu" ? "Sikeres regisztráció!" : "Successful registration!" , "success");
                  login(email, password, showAlert);
                  sendEmail(email); // Email küldése a regisztráció után
                  navigate("/");
                })
                .catch((error) => {
                  showAlert(error.response.data , "error");
                });
              }
              else{
                showAlert(language === "hu" ? "A két jelszó nem egyezik!" : "The two passwords doesn't match!", "error");
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
                  maxlength="10"
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
