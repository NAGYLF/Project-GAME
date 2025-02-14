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

  useEffect(() => {
     setToken(localStorage.getItem('token'));
      if (token) {
        const decodedToken = jwtDecode(token);
        setIsLoggedIn(true);
        setIsAdmin(decodedToken.IsAdmin === "True" ? true : false);
        setId(decodedToken.UserId);
      }
    }, [isLoggedIn, setIsAdmin]);

    const admincode=()=>{
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
      setToken(null);
      setIsLoggedIn(false);
    };

    const login = (email, password) => {
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
          const isAdmin = decodedToken.IsAdmin === "True" ? true : false;
  
          // Állapotok beállítása
          setIsLoggedIn(true);
          setIsAdmin(isAdmin);  // Az admin státusz beállítása
  
          // A felhasználót átirányítjuk a főoldalra
          navigate("/");
        } else {
          alert(response.data.message);
        }
      }).catch((error) => {
        console.error("Login failed:", error);
        alert(language === "hu" ? "Sikertelen bejelentkezés!" : "Login failed!");
      });
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
      <Routes>
        {/* Passing language and texts props to all components */}
        <Route path="/description" element={<Description language={language} texts={texts} />} />
        <Route path="/about" element={<About language={language} texts={texts}/>} />
        <Route path="/login" element={<Login language={language} texts={texts} isLoggedIn={isLoggedIn} setIsLoggedIn={setIsLoggedIn} setIsAdmin={setIsAdmin} login={login}/>} />
        <Route path="/register" element={<Register language={language} texts={texts} isLoggedIn={isLoggedIn} setIsLoggedIn={setIsLoggedIn} code={code} login={login}/>} />
        <Route path="/settings" element={<Settings texts={texts} language={language} id={id} token={token} logout={logout}/>} />
        <Route path="/admin" element={<Admin texts={texts} language={language} code={code} setCode={setCode} secondsLeft={secondsLeft} setSecondsLeft={setSecondsLeft} admincode={admincode}/>} />
        <Route path="/search" element={<Search texts={texts} language={language}/>} />
        <Route path="/player/:id" element={<Player texts={texts} language={language} token={token} isAdmin={isAdmin}/>} />
      </Routes>
      <Footer 
        language={language}  
        texts={texts} 
      />
    </div>
  );
}

export default App;