import React from 'react';
import Levi from './img/levi.png';
import Arpi from './img/arpi.png';
import Balow from './img/balow.png';

function About(props) {
  return (
    <div className="content">
      <div className="box" id="about">
        <div className="row text-center align-items-center">
          <div className="col-md-4">
            <h2><img src={Levi} alt="Levente" style={{width: '30px', height: '30px', borderRadius: '50%'}}></img>{props.language === "hu" ? 'Nagy Levente Ferenc' : 'Levente Ferenc Nagy'}</h2>
            <p>{props.language === "hu" ? 'A cél szentesíti az eszközt. Ha megdobnak kővel dobd vissza kézigránáttal.' : 'English idézet'}</p>
          </div>
          <div className="col-md-4">
            <h2><img src={Arpi} alt="Arpad" style={{width: '30px', height: '30px', borderRadius: '50%'}}></img>{props.language === "hu" ? 'Veller Árpád' : 'Árpád Veller'}</h2>
            <p>{props.language === "hu" ? 'Az idő pénz, a pénz beszél a kutya ugat. Megy mint a ágybahugyozás.' : 'English idézet'}</p>
          </div>
          <div className="col-md-4">
            <h2><img src={Balow} alt="Balint" style={{width: '30px', height: '30px', borderRadius: '50%'}}></img>{props.language === "hu" ? 'Csanálosi Bálint' : 'Bálint Csanálosi'}</h2>
            <p>{props.language === "hu" ? 'Kicsi a bors, de erős. Jobb száz irigy mint egy zsidó.' : 'English idézet'}</p>
          </div>
        </div>
      </div>
    </div>
      );
}

export default About;
