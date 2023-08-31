import { RotatingSquare } from  'react-loader-spinner'
import React from 'react';

export function PlaceHolder()
{
  console.log("6") 
  return(
    <div
    style={{
      display: "flex",
      height: "100vh",
      justifyContent: "center",
      alignItems: "center"
      
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
