import { Box, FormControlLabel, Checkbox } from "@mui/material";
import { ICalendar } from "../Support/Models";
import { useState } from "react";

interface ICheckBoxParams {
  calendar: ICalendar;
  handleChanges(calendar: ICalendar, checked: boolean): void;
}

export function CheckBoxRender(params: ICheckBoxParams) {

    //const [checked,setChecked] = useState<boolean>(false)

  return (
    <Box>
      <Box>
        <FormControlLabel
          key={params.calendar.calId}
          label={params.calendar.title}
          control=<Checkbox />
          onChange={(event: any) => {
            params.handleChanges(params.calendar,  event.target.checked)
            }}
        />
      </Box>
    </Box>
  );
}
