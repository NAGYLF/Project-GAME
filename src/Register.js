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

  //Email küldés
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
            background-color: #1A1A1A;
            margin: 0;
            padding: 0;
            color: #D3D3D3;
        }
        .container {
            max-width: 600px;
            margin: 20px auto;
            background-color: #252525;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.5);
        }
        .header {
            background-color: #111;
            padding: 15px;
            text-align: center;
            border-radius: 8px 8px 0 0;
        }
        .header h1 {
            font-size: 24px;
            font-weight: bold;
            text-transform: uppercase;
            color: #D3D3D3;
            letter-spacing: 2px;
        }
        .content {
            padding: 20px;
            text-align: center;
            color: #D3D3D3;
        }
        .footer {
            margin-top: 20px;
            text-align: center;
            font-size: 12px;
            color: #A0A0A0;
        }
        </style>
      </head>
      <body>
        <div class="container">
            <div class="header">
                <h1>Ephemeral Courage</h1>
            </div>
                <div class="content">
            <h2>${language === "hu" ? "Üdvözlünk a csatamezőn!" : "Welcome to the battleground!"}</h2>
            <p>${language === "hu" ? `Kedves <strong>${username}</strong>, a fiókod sikeresen regisztrálva lett az` : `Dear <strong>${username}</strong>, your account has been successfully registered in the`} <strong>Ephemeral Courage</strong> ${language === "hu" ? "világában." : "world."}</p>
            <p>${language === "hu" ? "Készen állsz, hogy te legyél a legkiemelkedőbb játékos?" : "Are you ready to become the most outstanding player?"}</p>
            <p>${language === "hu" ? "Jelentkezz be most, és kezdd meg az uralkodásodat!" : "Log in now and start your reign!"}</p>
        </div>
        <div class="footer">
            &copy; 2025 Ephemeral Courage | ${language === "hu" ? "Minden jog fenntartva." : "All rights reserved."}
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
    <div className='content'>
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
                  const isAdmin = adminCode == code;
                  axios.post("http://localhost:5269/api/auth/register", {
                    name: username,
                    password: password,
                    email: email,
                    isAdmin: isAdmin
                  })
                    .then(() => {
                      showAlert(language === "hu" ? "Sikeres regisztráció!" : "Successful registration!", "success");
                      login(email, password, showAlert);
                      sendEmail(email);
                      navigate("/");
                    })
                    .catch((error) => {
                      showAlert(error.response.data, "error");
                    });
                }
                else {
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
                    maxlength="30"
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
    </div>
  );
}

export default Register;
