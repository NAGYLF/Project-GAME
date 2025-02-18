import axios from "axios";
import React, { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";

const Settings = ({ texts, language, id, token, logout, showAlert }) => {
  const [newName, setNewName] = useState("");
  const [newEmail, setNewEmail] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [newPasswordAgain, setNewPasswordAgain] = useState("");
  const location = useLocation();
  const navigate = useNavigate();

  const deleteAccount = () =>{
    axios.delete(`http://localhost:5269/api/Player/${id}?token=${token}`).then(() => {
      logout();
      showAlert(language === "hu" ? "Fiók sikeresen törölve!" : "User successfully deleted!" , "success");
      navigate("/");
    }
  )}

  const modifyAccount = () => {
    axios.put(`http://localhost:5269/api/Player/${id}`, {
      name: newName,
      email: newEmail,
      password: newPassword,
      }, 
    )
  .then(() => {
    showAlert(language === "hu" ? "Fiók módosítva!" : "Account modified!" , "success");
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