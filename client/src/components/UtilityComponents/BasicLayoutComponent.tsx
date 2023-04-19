import { AppointmentForm } from "@devexpress/dx-react-scheduler-material-ui";
import React, { useState } from "react";
import { AppointmentModel, SelectOption } from "@devexpress/dx-react-scheduler";
import { ICalendar } from "../Support/Models";
import { MenuItem, Select, SelectChangeEvent } from "@mui/material";

interface CustomAppointmentFormProps extends AppointmentForm.BasicLayoutProps {
  calData?: ICalendar[];
}

export function BasicLayout(props: CustomAppointmentFormProps) {
 
  const handleChange = (value: SelectChangeEvent) => {
    props.onFieldChange({ calId: value.target.value });
  };

  return (
    <AppointmentForm.BasicLayout {...props}>
      <Select 
        onChange={handleChange}
        defaultValue={props.appointmentData.calId !== '' ?  props.calData![0].calId : props.appointmentData.calId}
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
