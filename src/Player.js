import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';

export default function Player({ texts, language }) {
  return (
    <div className='content'>
        <div className='box' style={{height: '50vh', width: "50vw"}}>
            <h3>{language === "hu" ? 'Játékos statisztikái' : 'Players statistics'}</h3>
            <hr/>
            <div className="left">
                <p>{language === "hu" ? 'Pontok: ' : 'Score: '}0</p>
                <p>{language === "hu" ? 'Megölt ellenfelek: ' : 'Enemies killed: '}0</p>
                <p>{language === "hu" ? 'Játszott idő: ' : 'Time played: '}0</p>
                <p>{language === "hu" ? 'Halálok: ' : 'Deaths: '}0</p>
            </div>
        </div>
    </div>
  )
}