import { Box, FormControlLabel, Checkbox } from "@mui/material";
import { ICalendar } from "../Support/Models";
import { useState } from "react";

interface ICheckBoxParams {
  calendar: ICalendar;
  fromFavsWindow:boolean,
  handleChanges(calendar: ICalendar, checked: boolean): void;
}

export function CheckBoxRender(params: ICheckBoxParams) {

  let chkd=false
  if(params.fromFavsWindow)    
    params.calendar.checkedFav==true?chkd=true:chkd=false
  else
    params.calendar.checkedBase==true?chkd=true:chkd=false

  return (
    <Box>
      <Box>
        <FormControlLabel
          key={params.calendar.calId}
          label={params.calendar.title}          
          onChange={(event: any) => {
            params.handleChanges(params.calendar,  event.target.checked)            
            }}
          control=<Checkbox 
          checked={chkd}
          />  
       />   
      </Box>
    </Box>
  );
}
