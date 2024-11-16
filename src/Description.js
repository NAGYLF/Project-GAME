import React, { useState, useEffect } from 'react';
import './Description.css';

const CenteredDiv = ({ onClose }) => {
  return (
    <div className="centered-div">
      <button className="close-btn" onClick={onClose}>X</button>
      Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin vel nunc sit amet lacus euismod accumsan vitae ac nisi. Duis sit amet enim metus. Donec fringilla vitae nulla non sodales. In venenatis elit a arcu venenatis luctus eget eget justo. Integer arcu odio, euismod nec eros sed, ultricies tincidunt tortor. Curabitur pharetra viverra leo vel tincidunt. Duis vel arcu lorem. Donec tristique tellus in augue tempus tristique.

      Aenean vel orci et purus faucibus consectetur nec ut ligula. Sed et sem facilisis erat ornare pretium.
      Duis sit amet nisl quis libero maximus maximus. Maecenas ullamcorper odio quis posuere hendrerit. Morbi vehicula nulla sollicitudin, mattis nisl id, rutrum nisi. Quisque eu cursus augue. Sed malesuada nulla eu elit lobortis posuere. Proin bibendum ultricies magna non fringilla. Vivamus aliquet ligula neque. Morbi suscipit laoreet nunc, non suscipit justo convallis sed. Morbi commodo aliquet viverra. Nunc id ante faucibus, sagittis elit eu, semper enim.
    </div>
  );
};

export default CenteredDiv;