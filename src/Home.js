import React from 'react'
import Icon from './img/icon.png'

export default function Home({ language, texts }) {
  return (
    <div id='homecontent'>
      <img src={Icon} style={{ width: "200px", height: "200px" }} />
      <h1>{language === "hu" ? `Üdvözöljük az Ephemeral Courage weboldalán!` : `Welcome on the page of Ephemeral Courage!`}</h1>
      <a id="download" href="https://drive.google.com/file/d/1Bb0RqMSQm7dDXpHNBP1v6C8h2OqInu9t/view" target='_blank'>
        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="currentColor" className="bi bi-download" style={{marginRight: "10px"}} viewBox="0 0 16 16">
          <path d="M.5 9.9a.5.5 0 0 1 .5.5v2.5a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1v-2.5a.5.5 0 0 1 1 0v2.5a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2v-2.5a.5.5 0 0 1 .5-.5" />
          <path d="M7.646 11.854a.5.5 0 0 0 .708 0l3-3a.5.5 0 0 0-.708-.708L8.5 10.293V1.5a.5.5 0 0 0-1 0v8.793L5.354 8.146a.5.5 0 1 0-.708.708z" />
        </svg>
        {language === "hu" ? "Letöltés" : "Download"}
        </a>
    </div>
  )
}
