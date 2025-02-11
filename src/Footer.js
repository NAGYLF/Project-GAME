import React from 'react';

const Footer = ({ texts, language }) => {
  return (
    <footer className="footer text-white text-center py-3">
      <div className="container">
        <p className="mb-0">{texts[language].footerText}</p>
      </div>
    </footer>
  );
};

export default Footer;