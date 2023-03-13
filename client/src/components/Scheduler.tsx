import * as React from "react";
import "../utils/date-extension"
import Paper from "@mui/material/Paper";
import { useQuery } from "react-query";
import axios from "axios";
import {
  AppointmentModel,
  ViewState
} from "@devexpress/dx-react-scheduler";
import {
  Scheduler,
  DayView,
  Appointments,
  Toolbar,
  DateNavigator,
  TodayButton,
} from "@devexpress/dx-react-scheduler-material-ui";

export function DevScheduler() {

  
 
  const [currentDate, setCurrentDate] = React.useState(new Date());

  const { isLoading, error, data, isFetching } = useQuery<
    AppointmentModel[],
    Error
  >/*("appointmentData", () =>
    axios
      .get(
        `http://localhost:5152/EWSApiScheduler?startD=${currentDate.toISODate()}`
      )
      .then((res) => res.data)
  );*/
  ("appointmentData", () =>
    axios
      .get(
        `http://localhost:5152/EWSApiScheduler/GetAppos?CalendarId=AQMkADAwATNiZmYAZC0zNThjLWYyNzItMDACLTAwCgAuAAADVsPWUeK5pEqLP2nAwws2hwEAru%2F256NrUUC1fGm0RnQ7hQAAAgENAAAA&startDate=${currentDate.toISODate()}`
      )
      .then((res) => res.data)
  );

  if (isLoading) return <div>Loading...</div>;

  if (error) return <div>An error has occurred: + {error.message}</div>;

  return (
    <Paper>
      <Scheduler data={data}>
        <ViewState
          currentDate={currentDate}
          onCurrentDateChange={(currentDate) => setCurrentDate(currentDate)}
        />
        <DayView startDayHour={9} endDayHour={19} />
        <Toolbar />
        <DateNavigator />
        <TodayButton />
        <Appointments />
      </Scheduler>
    </Paper>
  );
}
