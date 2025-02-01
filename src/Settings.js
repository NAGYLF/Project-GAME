import React, { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";

const Settings = ({ texts, language }) => {
  const [newName, setNewName] = useState("");
  const [newEmail, setNewEmail] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const location = useLocation();
  const navigate = useNavigate();

  const handleSave = (e) => {
    e.preventDefault();
    console.log("New settings saved:", { newName, newEmail, newPassword });
    navigate("/home");
  };

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
              onClick={() => navigate("/home")}
            ></button>
          </div>
          <div className="modal-body">
            <form onSubmit={handleSave}>
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
                  required
                />
              </div>
            </form>
          </div>
          <div className="modal-footer">
            <button type="submit" className="btn btn-light" onClick={handleSave}>
              {language === "hu" ? 'Mentés' : 'Save'}
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
};

export default Settings;