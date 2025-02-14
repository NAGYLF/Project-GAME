import React, { useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';

const Admin = ({ texts, language, code, setCode ,secondsLeft ,setSecondsLeft, admincode }) => {
  const location = useLocation();
  const navigate = useNavigate();

  //Kód
  useEffect(() => {
    admincode();
  }, []);

  return (
    <div
      className={`modal fade ${location.pathname === "/admin" ? "show" : ""}`}
      id="adminModal"
      tabIndex="-1"
      role="dialog"
      aria-labelledby="adminModalLabel"
      aria-hidden={location.pathname !== "/admin" ? "true" : "false"}
      style={{ display: location.pathname === "/admin" ? "block" : "none" }}
      data-bs-backdrop="static"
      data-bs-keyboard="false"
    >
      <div className="modal-dialog modal-dialog-centered" role="document">
        <div className="modal-content bg-dark text-white">
          <div className="modal-header">
            <h5 className="modal-title" id="adminModalLabel">
              {texts[language].adminSettings}
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
          {/* Debug mód beállítás */}
          <div className="form-check mb-3">
            <input
              className="form-check-input"
              type="checkbox"
              id="debugMode"
            />
            <label className="form-check-label" htmlFor="debugMode">
            {language === "hu" ? 'Debug mód engedélyezése' : 'Enable debug mode'}
            </label>
          </div>

          {/* Új admin jelszó generálása */}
              <div className="mb-3">
                <label htmlFor="newAdminPassword" className="form-label">
                {language === "hu" ? 'Új admin jelszó' : 'New admin password'}
                </label>
                <input
                  type="text"
                  style={{margin:"5px", width:"90px"}}
                  value={code}
                  className="form-control"
                  id="newAdminPassword"
                  readOnly
                />
                <label>
                  {language === "hu" ? 'Az új jelszó változni fog: ' : 'New password in: '}{secondsLeft}
                </label>
              </div>
            </form>
          </div>
          <div className="modal-footer">
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

export default Admin;