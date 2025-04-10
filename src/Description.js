import React from 'react';
import { useNavigate } from 'react-router-dom';

function Description(props) {

  const navigate = useNavigate();

  //Ha a contentre nyomunk nem a boxra akkor visszamegy a főoldalra
  const handleContentClick = (e) => {
    if(e.target.className === 'content'){
        navigate('/');
    }
  }

  return (
    <div className="content" onClick={handleContentClick}>
      <div className="box">
        <h1>{props.language === "hu" ? "Leírás" : "Description"}</h1>
        <hr/>
        <p>{props.language === "hu" ? `Az Ephemeral Courage világában tisztában kell lenned a taktikázás képességeivel, felkészültnek kell lenned ellenfeleid elleni tűzharcokkal szemben. Játékunk jelenleg pre-alpha verzióban van, de napról napra dolgozunk fejleszésén, hogy játékosaink a lehető legjobb élménynek legyenek részesei.` : `In the world of Ephemeral Courage, you must be aware of the skills required for tactics and be prepared for firefights against your enemies. Our game is currently in pre-alpha, but we are working on its development every day to ensure that our players experience the best possible gameplay.`}</p>
      </div>
    </div>
  );
}

export default Description;
