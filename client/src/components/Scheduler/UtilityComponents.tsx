import {Box, FormControlLabel, Checkbox} from '@mui/material'
import React from 'react'
import { CalModel } from '../Support/Models'
import { AppointmentForm } from '@devexpress/dx-react-scheduler-material-ui'
import {    AppointmentModel, SelectOption  } from "@devexpress/dx-react-scheduler";
import { calTitles,calIds } from "./Scheduler";
import { RotatingSquare } from  'react-loader-spinner'

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

export function CheckBoxRender(calTitles:string[],checkBoxState:any[],handleOnChange: (position: number) => void)
{
    return(          
        <Box>
            <Box>
                {calTitles.map((title:string,index)=>{
                    return (
                    <FormControlLabel 
                    key={index}
                    id={`${index}`}
                    label= {title}                    
                    control=<Checkbox />
                    checked={checkBoxState[index]}
                    onChange={() => handleOnChange(index)}
                    />)
                }
                )}   
            </Box>
        </Box>        
    )
}

export let SelectedCal:number=0

export function BasicLayout({ onFieldChange,appointmentData, ...restProps}:any) { 
  
  if(calIds.indexOf(appointmentData.calId)>-1)
    appointmentData.customField= calIds.indexOf(appointmentData.calId)
  else
    appointmentData.customField= "0"

    const CalTitles:SelectOption[]=calTitles.map((CTitle,number)=>{
    let CTitleSelectOpt:SelectOption={
      text:CTitle,
      id:number
    }
    return CTitleSelectOpt
    })
    
    function onValChange(nextValue:string|number) {
        SelectedCal=+nextValue
        return onFieldChange({ customField: nextValue });
      };
      //console.log('fiChng: '+onFieldChange)
    return (
      <AppointmentForm.BasicLayout
      appointmentData={appointmentData}    
      onFieldChange={onFieldChange}  
      {...restProps}
      >
        <AppointmentForm.Select  
        availableOptions={CalTitles}
        onValueChange={onValChange}
        value={appointmentData.customField}
          text="Custom Field" 
          type={AppointmentForm.Select.defaultProps!.type!}   
        />
      </AppointmentForm.BasicLayout>
    );
  };