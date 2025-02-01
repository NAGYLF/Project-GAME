import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';

export default function Search({ texts, language }) {

  return (
    <div className='content'>
      <div className='box' style={{height: '80vh', width: "80vw"}}>
        <input type="search" placeholder={texts[language].search+" onchange lesz"} style={{width: "100%", height: "50px",border:"2px solid black",borderRadius: "10px", fontSize: "20px", padding: "10px"}}/>
        <div id="searchbox">
          <div className="row">
            <p>Test</p>
            <p>Test</p>
            <p>Test</p>
          </div>
          <div className="row">
            <p>Test</p>
            <p>Test</p>
            <p>Test</p>
          </div>
          <div className="row">
            <p>Test</p>
            <p>Test</p>
            <p>Test</p>
          </div>
          <div className="row">
            <p>Test</p>
            <p>Test</p>
            <p>Test</p>
          </div>
          <div className="row">
            <p>Test</p>
            <p>Test</p>
            <p>Test</p>
          </div>
          <div className="row">
            <p>Test</p>
            <p>Test</p>
            <p>Test</p>
          </div>
          <div className="row">
            <p>Test</p>
            <p>Test</p>
            <p>Test</p>
          </div>
          <div className="row">
            <p>Test</p>
            <p>Test</p>
            <p>Test</p>
          </div>
          <div className="row">
            <p>Test</p>
            <p>Test</p>
            <p>Test</p>
          </div>
          <div className="row">
            <p>Test</p>
            <p>Test</p>
            <p>Test</p>
          </div>
          <div className="row">
            <p>Test</p>
            <p>Test</p>
            <p>Test</p>
          </div>
          <div className="row">
            <p>Test</p>
            <p>Test</p>
            <p>Test</p>
          </div>
          <div className="row">
            <p>Test</p>
            <p>Test</p>
            <p>Test</p>
          </div>
          <div className="row">
            <p>Test</p>
            <p>Test</p>
            <p>Test</p>
          </div>
        </div>
      </div>
    </div>
  );
}