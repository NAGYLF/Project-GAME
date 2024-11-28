import React from 'react';
import "./Video.css";

export default function Video() {
  return (
    <div 
      className="container d-flex justify-content-center align-items-center" 
      style={{ height: '75vh' }}
    >
      <div 
        id="video" 
        className="embed-responsive embed-responsive-16by9" 
        style={{ width: '80%', maxWidth: '100%' }}
      >
        <iframe
          id="video"
          className="embed-responsive-item"
          src="https://www.youtube.com/embed/dQw4w9WgXcQ?autoplay=1&mute=1" 
          allowFullScreen
          title="YouTube video"
        ></iframe>
      </div>
    </div>
  );
}
