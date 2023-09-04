import { Box, FormControlLabel, Checkbox } from "@mui/material";
import { ICalendar } from "../Support/Models";
import { useState } from "react";

interface ICheckBoxParams {
  calendar: ICalendar;
  checked:boolean;
  handleChanges(calendar: ICalendar, checked: boolean): void;
}

export function CheckBoxRender(params: ICheckBoxParams) {
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
          checked={params.checked}          
          />  
       />   
      </Box>
    </Box>
  );
}
