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

  const deleteAccount = () => {
    axios.delete(`http://localhost:5269/api/Player/${id}?token=${token}`).then(() => {
      logout();
      showAlert(language === "hu" ? "Fiók sikeresen törölve!" : "User successfully deleted!", "success");
      navigate("/");
    }
    )
  }

  const sendEmail = (email) => {
    const sendingEmail = {
      to: email,
      subject: `${language === "hu" ? "Regisztráció" : "Registration"}`,
      body: `
        <html>
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>Ephemeral Courage - Adatmódosítás</title>
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
            <h2>${language === "hu" ? "Adataid sikeresen frissítve!" : "Your data has been successfully updated!"}</h2>
            <p>${language === "hu" ? `Kedves <strong>${newName}</strong>, a fiókod adatai sikeresen módosítva lettek az` : `Dear <strong>${newName}</strong>, your account data has been successfully updated in the`} <strong>Ephemeral Courage</strong> ${language === "hu" ? "világában." : "world."}</p>
            <p>${language === "hu" ? "Ha te végezted a módosítást, nincs további teendőd." : "If you made these changes, no further action is needed."}</p>
            <p>${language === "hu" ? "Ha nem te kezdeményezted a változtatásokat, kérjük, lépj kapcsolatba az ügyfélszolgálattal!" : "If you did not initiate these changes, please contact customer support!"}</p>
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

  const modifyAccount = () => {
    axios.put(`http://localhost:5269/api/Player/${id}`, {
      name: newName,
      email: newEmail,
      password: newPassword,
      isAdmin: IsAdmin
    },
    )
      .then(() => {
        showAlert(language === "hu" ? "Fiók módosítva!" : "Account modified!", "success");
        sendEmail(newEmail);
        setUsername(newName);
        navigate("/");
      })
      .catch((error) => {
        showAlert(language === "hu" ? "Hiba!" : "Error!", "error");
      })
  };

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
    <div className='content'>
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
            <p style={{ textAlign: "center", margin: "auto", marginTop: "20px", cursor: "default" }}>{language === "hu" ? "A változtatni nem kívánt adatokat hagyd változtatlanul." : "Leave the data you don't want to change unchanged."}</p>
            <div className="modal-body">
              <form onSubmit={(e) => {
                e.preventDefault();
                if (newPassword === newPasswordAgain) {
                  modifyAccount();
                  navigate("/");
                }
                else {
                  showAlert(language === "hu" ? "A két jelszó nem egyezik!" : "The two password doesn't match!", "error");
                }
              }}>
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
                    maxlength="30"
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
    </div>
  );
};

export default Settings;