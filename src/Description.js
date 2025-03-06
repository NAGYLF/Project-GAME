import React from 'react';
import './Nav.css';
import { useNavigate } from 'react-router-dom';

function Description(props) {

  const navigate = useNavigate();

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
        <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus lacinia est eget ante
            euismod, id sollicitudin leo malesuada. Integer convallis malesuada leo, ac auctor
            lacus interdum et. Cras vestibulum, sapien at consequat tristique, risus metus
            tincidunt orci, eu posuere ligula ligula non turpis. Etiam vitae efficitur libero.</p>
      </div>
    </div>
  );
}

export default Description;
