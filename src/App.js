import React, { useEffect, useState } from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
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

function App() {
  const [language, setLanguage] = useState('hu');
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [isAdmin, setIsAdmin] = useState(false);
  const [code, setCode] = useState('');
  const [secondsLeft, setSecondsLeft] = useState(30);
  const [username, setUsername] = useState('');

  useEffect(() => {
      const token = localStorage.getItem('token');
      if (token) {
        const decodedToken = jwtDecode(token);
        setIsLoggedIn(true);
        setIsAdmin(decodedToken.IsAdmin);

      }
    }, [setIsLoggedIn, setIsAdmin]);

    
  
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
    <Router>
      <Nav 
        language={language} 
        setLanguage={setLanguage} 
        texts={texts} 
        isLoggedIn={isLoggedIn} 
        setIsLoggedIn={setIsLoggedIn} 
        isAdmin={isAdmin} 
        username={username}
      />
      <Routes>
        {/* Passing language and texts props to all components */}
        <Route path="/description" element={<Description language={language} texts={texts} />} />
        <Route path="/about" element={<About language={language} texts={texts}/>} />
        <Route path="/login" element={<Login language={language} texts={texts} isLoggedIn={isLoggedIn} setIsLoggedIn={setIsLoggedIn} setIsAdmin={setIsAdmin}/>} />
        <Route path="/register" element={<Register language={language} texts={texts} isLoggedIn={isLoggedIn} setIsLoggedIn={setIsLoggedIn}/>} />
        <Route path="/settings" element={<Settings texts={texts} language={language}/>} />
        <Route path="/admin" element={<Admin texts={texts} language={language} code={code} setCode={setCode} secondsLeft={secondsLeft} setSecondsLeft={setSecondsLeft} />} />
        <Route path="/search" element={<Search texts={texts} language={language}/>} />
        <Route path="/player/:id" element={<Player texts={texts} language={language}/>} />
      </Routes>
      <Footer 
        language={language}  
        texts={texts} 
      />
    </Router>
  );
}

export default App;