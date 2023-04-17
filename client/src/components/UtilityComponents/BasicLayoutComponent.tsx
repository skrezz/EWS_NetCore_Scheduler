import { AppointmentForm } from "@devexpress/dx-react-scheduler-material-ui";
import React, { useState } from "react";
import { AppointmentModel, SelectOption } from "@devexpress/dx-react-scheduler";
import { ICalendar } from "../Support/Models";

interface CustomAppointmentFormProps extends AppointmentForm.BasicLayoutProps {
  calData?: ICalendar[];
}

export function BasicLayout(props: CustomAppointmentFormProps) {
  const options: SelectOption[] | undefined = props.calData?.map((calendar) => {
    return {
      text: calendar.title,
      id: calendar.calId,
    } as SelectOption;
  });
  const onCustomFieldChange = (value: any) => {
    props.onFieldChange({ calId: value });
  };

  return (
    <AppointmentForm.BasicLayout {...props}>
      <AppointmentForm.Select
        availableOptions={options}
        onValueChange={onCustomFieldChange}
        value={props.appointmentData.calId}
        type="filledSelect"
      />
    </AppointmentForm.BasicLayout>
  );
}
