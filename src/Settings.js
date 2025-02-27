import axios from "axios";
import React, { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";

const Settings = ({ texts, language, id, token, logout, showAlert, setUsername }) => {
  const [newName, setNewName] = useState("");
  const [newEmail, setNewEmail] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [newPasswordAgain, setNewPasswordAgain] = useState("");
  const [IsAdmin, setIsAdmin] = useState(false);
  const location = useLocation();
  const navigate = useNavigate();

  const deleteAccount = () =>{
    axios.delete(`http://localhost:5269/api/Player/${id}?token=${token}`).then(() => {
      logout();
      showAlert(language === "hu" ? "Fiók sikeresen törölve!" : "User successfully deleted!" , "success");
      navigate("/");
    }
  )}

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
                ${language === "hu" ? "Adat változtatás sikeres" : "Data change successfull"}
              </div>
              <div class="email-content">
                ${language === "hu" ? "Adatait sikeresen megváltoztatta." : "Your data has been succesfully changed."}
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

  const modifyAccount = () => {
    axios.put(`http://localhost:5269/api/Player/${id}`, {
      name: newName,
      email: newEmail,
      password: newPassword,
      isAdmin: IsAdmin
      }, 
    )
  .then(() => {
    showAlert(language === "hu" ? "Fiók módosítva!" : "Account modified!" , "success");
    sendEmail(newEmail);
    setUsername(newName);
    navigate("/");
  })
  .catch((error) => {
    showAlert(language === "hu" ? "Hiba!" : "Error!", "error");
  })};

  useEffect(() => {
    if (id) {
      axios.get(`http://localhost:5269/GetbyId/${id}`).then((res) => {
        setNewName(res.data.name);
        setNewEmail(res.data.email);
        setIsAdmin(res.data.isAdmin);
      }).catch((error) => {
        showAlert(language === "hu" ? "Hiba történt az adatok lekérésekor!" : "Error fetching data!", "error");
        console.error('Error fetching data:', error);
      });
    }
  }, [id]);

  return (
    <div
      className={`modal fade ${location.pathname === "/settings" ? "show" : ""}`}
      id="settingsModal"
      tabIndex="-1"
      role="dialog"
      aria-labelledby="settingsModalLabel"
      aria-hidden={location.pathname !== "/settings" ? "true" : "false"}
      style={{ display: location.pathname === "/settings" ? "block" : "none" }}
    >
      <div className="modal-dialog modal-dialog-centered" role="document">
        <div className="modal-content bg-dark text-white">
          <div className="modal-header">
            <h5 className="modal-title" id="settingsModalLabel">
              {texts[language].settings}
            </h5>
            <button
              type="button"
              className="btn-close bg-light"
              data-bs-dismiss="modal"
              aria-label="Close"
              onClick={() => navigate("/")}
            ></button>
          </div>
          <p style={{textAlign: "center", margin: "auto", marginTop: "20px", cursor: "default"}}>{language === "hu" ? "A változtatni nem kívánt adatokat hagyd változtatlanul." : "Leave the data you don't want to change unchanged."}</p>
          <div className="modal-body">
            <form onSubmit={(e) => {
              e.preventDefault();
              if(newPassword === newPasswordAgain) {
              modifyAccount();
              navigate("/");
            }
            else{
              showAlert(language === "hu" ? "A két jelszó nem egyezik!" : "The two password doesn't match!" , "error");
            }}}>
              <div className="mb-3">
                <label htmlFor="newName" className="form-label">
                  {language === "hu" ? 'Új felhasználónév' : 'New username'}
                </label>
                <input
                  type="text"
                  className="form-control"
                  id="newName"
                  value={newName}
                  onChange={(e) => setNewName(e.target.value)}
                  maxlength="10"
                  placeholder={language === "hu" ? 'Új felhasználónév' : 'New username'}
                  required
                />
              </div>
              <div className="mb-3">
                <label htmlFor="newEmail" className="form-label">
                  {language === "hu" ? 'Új email' : 'New email'}
                </label>
                <input
                  type="email"
                  className="form-control"
                  id="newEmail"
                  value={newEmail}
                  onChange={(e) => setNewEmail(e.target.value)}
                  placeholder={language === "hu" ? 'Új email' : 'New email'}
                  required
                />
              </div>
              <div className="mb-3">
                <label htmlFor="newPassword" className="form-label">
                  {language === "hu" ? 'Új jelszó' : 'New password'}
                </label>
                <input
                  type="password"
                  className="form-control"
                  id="newPassword"
                  value={newPassword}
                  onChange={(e) => setNewPassword(e.target.value)}
                  placeholder={language === "hu" ? 'Új jelszó' : 'New password'}
                />
              </div>
              <div className="mb-3">
                <label htmlFor="newPasswordAgain" className="form-label">
                  {language === "hu" ? 'Új jelszó újra' : 'New password again'}
                </label>
                <input
                  type="password"
                  className="form-control"
                  id="newPasswordAgain"
                  value={newPasswordAgain}
                  onChange={(e) => setNewPasswordAgain(e.target.value)}
                  placeholder={language === "hu" ? 'Új jelszó újra' : 'New password again'}
                />
              </div>
              <div className="d-flex justify-content-between">
                <button type="submit" className="btn btn-light">
                {language === "hu" ? 'Mentés' : 'Save'}
                </button>
                <button type="button" className="btn btn-danger" onClick={() => (window.confirm("Biztosan törölni szeretnéd a fiókod?") ? deleteAccount() : null)}>
                {language === "hu" ? 'Fiók törlése' : 'Delete account'}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Settings;