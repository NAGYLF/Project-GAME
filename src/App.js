import React, { useEffect, useState } from 'react';
import { Route, Routes, useNavigate } from 'react-router-dom';
import Nav from './Nav';
import Description from './Description';
import About from './About';
import Login from './Login';
import Register from './Register';
import Settings from "./Settings";
import Search from "./Search";
import Admin from "./Admin";
import Footer from "./Footer";
import Player from "./Player";
import Alert from './Alert'; // Import Alert component
import { jwtDecode } from 'jwt-decode';
import axios from 'axios';

function App() {
  const navigate = useNavigate();
  const [language, setLanguage] = useState('hu');
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [isAdmin, setIsAdmin] = useState(false);
  const [code, setCode] = useState('');
  const [secondsLeft, setSecondsLeft] = useState(0);
  const [username, setUsername] = useState('');
  const [id, setId] = useState('');
  const [token, setToken] = useState(localStorage.getItem('token'));
  const [isItBanned, setIsItBanned] = useState(false);
  const [alertMessage, setAlertMessage] = useState('');

  useEffect(() => {
    setToken(localStorage.getItem('token'));
    if (token) {
      const decodedToken = jwtDecode(token);
      setUsername(decodedToken.Username);
      setIsLoggedIn(true);
      setIsAdmin(decodedToken.IsAdmin === "True" ? true : false);
      setId(decodedToken.UserId);
      setIsItBanned(decodedToken.IsBanned === "True" ? true : false)
    }
  }, [isLoggedIn, setIsAdmin]);

  const admincode = () => {
    axios.get('http://localhost:5269/api/Player/code')
      .then(res => {
        setCode(res.data.code);
      })
      .catch(err => console.error(err));

    const interval = setInterval(() => {
      setSecondsLeft(prev => {
        if (prev > 1) {
          return prev - 1;
        } else {
          axios.get('http://localhost:5269/api/Player/code')
            .then(res => {
              setCode(res.data.code);
              setSecondsLeft(res.data.secondsLeft);
            })
            .catch(err => console.error(err));
        }
      });
    }, 1000);

    return () => clearInterval(interval);
  }

  const logout = () => {
    localStorage.removeItem('token');
    showAlert(language === "hu" ? "Sikeres kijelentkezés!" : "Successful logout!", "success");
    setToken(null);
    setIsLoggedIn(false);
    setIsAdmin(false);
  };

  const login = (email, password, showAlert) => {
    let user = {
      email: email,
      password: password
    };
    console.log(user);

      axios.post('http://localhost:5269/api/auth/login', user).then((response) => {
        if (response.data) {
          const token = response.data.token; // JWT token a válaszból
          localStorage.setItem('token', token); // Token mentése localStorage-ba
          setToken(token);
          // Token dekódolása
          const decodedToken = jwtDecode(token);
          console.log(decodedToken);
          const isAdmin = decodedToken.IsAdmin === "True" ? true : false;
          const isBanned = decodedToken.IsBanned === "True" ? true : false;

          if (!isBanned) {
            showAlert(language === "hu" ? "Sikeres bejelentkezés!" : "Successful login!", "success"); // Show success alert
            // Állapotok beállítása
            setIsLoggedIn(true);
            setIsAdmin(isAdmin);  // Az admin státusz beállítása

            // A felhasználót átirányítjuk a főoldalra
            navigate("/");
          } 
          else{
          showAlert(language === "hu" ? "Ez a felhasználó bannolva van!" : "This user is banned!", "error");
          navigate("/");
        }
        }
        else {
          showAlert(response.data.message, "error"); // Show error alert
        }
        

      }).catch(() => {
        showAlert(language === "hu" ? "Sikertelen bejelentkezés!" : "Login failed!", "error"); // Show error alert
      });

  };

  const showAlert = (message, type) => {
    setAlertMessage(message);
    const snackbar = document.getElementById("snackbar");
    snackbar.className = `show ${type}`;
    snackbar.style.display = "block";
    setTimeout(() => {
      snackbar.className = snackbar.className.replace(`show ${type}`, "");
      snackbar.style.display = "none";
    }, 2900);
  };


  const [texts, setTexts] = useState({
    hu: {
      description: 'Leírás',
      about: 'Rólunk',
      login: 'Bejelentkezés',
      register: 'Regisztráció',
      settings: 'Beállítások',
      signout: 'Kijelentkezés',
      adminSettings: 'Admin beállítások',
      search: 'Keresés',
      footerText: "© 2025 Ephemeral Courage. Minden jog fenntartva.",
    },
    en: {
      description: 'Description',
      about: 'About',
      login: 'Login',
      register: 'Register',
      settings: 'Settings',
      signout: 'Sign Out',
      adminSettings: 'Admin Settings',
      search: 'Search',
      footerText: "© 2025 Ephemeral Courage. All rights reserved.",
    },
  });


  return (
    <div>
      <Nav
        language={language}
        setLanguage={setLanguage}
        texts={texts}
        isLoggedIn={isLoggedIn}
        setIsLoggedIn={setIsLoggedIn}
        isAdmin={isAdmin}
        username={username}
        logout={logout}
      />
      <Alert message={alertMessage} />
      <Routes>
        {/* Passing language and texts props to all components */}
        <Route path="/description" element={<Description language={language} texts={texts} />} />
        <Route path="/about" element={<About language={language} texts={texts} />} />
        <Route path="/login" element={<Login language={language} texts={texts} isLoggedIn={isLoggedIn} setIsLoggedIn={setIsLoggedIn} setIsAdmin={setIsAdmin} login={login} showAlert={showAlert} />} />
        <Route path="/register" element={<Register language={language} texts={texts} isLoggedIn={isLoggedIn} setIsLoggedIn={setIsLoggedIn} code={code} login={login} showAlert={showAlert} admincode={admincode} />} />
        <Route path="/settings" element={<Settings texts={texts} language={language} id={id} token={token} logout={logout} showAlert={showAlert} setUsername={setUsername} />} />
        <Route path="/admin" element={<Admin texts={texts} language={language} id={id} code={code} setCode={setCode} secondsLeft={secondsLeft} setSecondsLeft={setSecondsLeft} admincode={admincode} />} />
        <Route path="/search" element={<Search texts={texts} language={language} isAdmin={isAdmin} />} />
        <Route path="/player/:id" element={<Player texts={texts} language={language} token={token} isAdmin={isAdmin} showAlert={showAlert} />} />
      </Routes>
      <Footer
        language={language}
        texts={texts}
      />
    </div>
  );
}

export default App;