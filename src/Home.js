import React from 'react'
import Icon from './img/icon.png'
import "./Home.css";

export default function Home({language, texts}) {
    return (
      <div id='homecontent'>
        <img src={Icon} style={{width: "200px", height: "200px"}}/>
        <h1>{language === "hu" ? `Üdvözöljük az Ephemeral Courage weboldalán!` : `Welcome on the page of Ephemeral Courage!`}</h1>
      </div>
  )
}
