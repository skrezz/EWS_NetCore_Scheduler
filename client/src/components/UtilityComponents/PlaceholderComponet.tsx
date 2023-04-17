import { RotatingSquare } from  'react-loader-spinner'
import React from 'react';

export function PlaceHolder()
{
  return(
    <div
    style={{
      position: "absolute",
      top: "50%",
      left: "50%",
      transform: "translate(-50%, -50%)",
      background: "white"
    }}
  >
  <RotatingSquare
    height="100"
    width="100"
    color="#9ffff5"
    ariaLabel="rotating-square-loading"
    strokeWidth="4"
    wrapperStyle={{}}
    wrapperClass=""
    visible={true}
  />
   </div>)
   //return (<div>...Loading</div>)
}
