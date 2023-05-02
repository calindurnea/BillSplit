import React, { useState } from "react";

import "./styles.css";

// don't change the Component name "App"
export default function App() {
    const [incrementValue, setIncrementValue] = React.useState(0);

    const incrementHandler = () => {
      setIncrementValue(prevState => prevState + 1 );
    };
  
    return (
      <div>
        <p id="counter">{incrementValue}</p>
        <button onClick={incrementHandler}>Increment</button>
      </div>
    );
}

// // don't change the Component name "App"
// export default function App() {
//   const [messageValidity, setmessageValidity] = React.useState("Invalid");

//   const messageChangeHandler = (event) => {
//     setmessageValidity(() => {
//       if (event.target.value.trim().length > 2) {
//         setmessageValidity("Valid");
//       } else {
//         setmessageValidity("Invalid");
//       }
//     });
//   };

//   return (
//     <form>
//       <label>Your message</label>
//       <input type="text" onChange={messageChangeHandler} />
//       <p>{messageValidity} message</p>
//     </form>
//   );
// }
