import { Box, FormControlLabel, Checkbox } from "@mui/material";
import { ICalendar } from "../Support/Models";
import { useState } from "react";

interface ICheckBoxParams {
  calendar: ICalendar;
  fromFavsWindow:boolean,
  handleChanges(calendar: ICalendar, checked: boolean): void;
}

export function CheckBoxRender(params: ICheckBoxParams) {

  let contr=<Checkbox />
  if(params.fromFavsWindow)    
    params.calendar.checkedFav==true?contr=<Checkbox checked/>:contr=<Checkbox />
  else
    params.calendar.checkedBase==true?contr=<Checkbox checked/>:contr=<Checkbox />

  return (
    <Box>
      <Box>
        <FormControlLabel
          key={params.calendar.calId}
          label={params.calendar.title}          
          onChange={(event: any) => {
            params.handleChanges(params.calendar,  event.target.checked)            
            }}
          control={contr}   
       />   
      </Box>
    </Box>
  );
}
