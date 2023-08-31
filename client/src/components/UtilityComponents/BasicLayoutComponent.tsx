import { AppointmentForm } from "@devexpress/dx-react-scheduler-material-ui";
import React, { useEffect, useState } from "react";
import { ICalendar } from "../Support/Models";
import { MenuItem, Select, SelectChangeEvent } from "@mui/material";

interface CustomAppointmentFormProps extends AppointmentForm.BasicLayoutProps {
  calData?: ICalendar[];
}

export function BasicLayout(props: CustomAppointmentFormProps) {

  const [calendarId, setCalendarId] = useState<string>(props.appointmentData.calId === undefined ? props.calData![0].calId : props.appointmentData.calId)
 
 useEffect(() => {
  props.onFieldChange({ calId: calendarId });
 },[calendarId])

  const handleChange = (value: SelectChangeEvent) => {
    props.onFieldChange({ calId: value.target.value });
    setCalendarId(value.target.value)
  };
  console.log("4") 
  return (
    <AppointmentForm.BasicLayout {...props}>
      <Select 
        onChange={handleChange}
        defaultValue={calendarId}
        value={calendarId}
      >
      {
      props.calData?.map((calendar) => {
        return <MenuItem key={calendar.calId} value={calendar.calId}>{calendar.title}</MenuItem>
      })
      }
        
       </Select>
    </AppointmentForm.BasicLayout>
  );
}
