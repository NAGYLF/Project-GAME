import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { useLocation, useNavigate } from 'react-router-dom';

const Admin = ({ texts, language, code, secondsLeft, id }) => {
  const location = useLocation();
  const navigate = useNavigate();

  const [generatedCode, setGeneratedCode] = useState(code);
  const [seconds, setSeconds] = useState(secondsLeft);
  const [DevConsole, setDevConsole] = useState(false);

  //Lekérjük az adminkódot az adatbázisból
  const GetAdminCode = () => {
    axios.get('http://localhost:5269/api/Player/code')
      .then(res => {
        setGeneratedCode(res.data.code);
        setSeconds(res.data.secondsLeft);
      })
      .catch(err => console.error(err));
  };

  //DevConsolet frissíti, a checkbox alapján
  const ManageDevConsole = (e) => {
    const newDevConsoleState = e.target.checked;
    setDevConsole(newDevConsoleState);

    axios.put(`http://localhost:5269/api/Admin/${id}`, {
      devConsole: newDevConsoleState
    })
      .catch(err => console.error(err));
  };

  //Ha true a DevConsole akkor bepipálja a checkboxot
  const getDevConsole = () => {
    console.log(id);
    axios.get(`http://localhost:5269/api/Admin/${id}`).then(res => {
      setDevConsole(res.data.devConsole);
    });
  };

  //30 secenként újra lekéri az adminkódot
  useEffect(() => {
    if (!generatedCode) {
      GetAdminCode();
    }

    const interval = setInterval(() => {
      setSeconds(prev => {
        if (prev > 1) {
          return prev - 1;
        } else {
          GetAdminCode();
          return 30;
        }
      });
    }, 1000);

    return () => clearInterval(interval);
  }, [generatedCode]);

  useEffect(() => {
    getDevConsole();
  }, [id]);

  return (
    <div className='content'>
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
                onClick={() => navigate("/")}
              ></button>
            </div>
            <div className="modal-body">
              <form>
                <div className="form-check mb-3">
                  <input
                    className="form-check-input"
                    type="checkbox"
                    id="debugMode"
                    onChange={ManageDevConsole}
                    checked={DevConsole}
                  />
                  <label className="form-check-label" htmlFor="debugMode">
                    {language === "hu" ? 'Debug mód engedélyezése' : 'Enable debug mode'}
                  </label>
                </div>

                <div className="mb-3">
                  <label htmlFor="newAdminPassword" className="form-label">
                    {language === "hu" ? 'Új admin jelszó' : 'New admin password'}
                  </label>
                  <input
                    type="text"
                    style={{ margin: "5px", width: "90px" }}
                    value={generatedCode}
                    className="form-control"
                    id="newAdminPassword"
                    readOnly
                  />
                  <label>
                    {language === "hu" ? 'Az új jelszó változni fog: ' : 'New password in: '}{seconds}
                  </label>
                </div>
              </form>
            </div>    
          </div>
        </div>
      </div>
    </div>
  );
};

export default Admin;
