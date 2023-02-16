import * as React from "react";
import Paper from "@mui/material/Paper";
import { useQuery } from "react-query";
import axios from "axios";
import { AppointmentModel, ViewState } from "@devexpress/dx-react-scheduler";
import {
  Scheduler,
  DayView,
  Appointments,
} from "@devexpress/dx-react-scheduler-material-ui";

export function DevScheduler() {
  const currentDate = "2023-02-16";

  const { isLoading, error, data, isFetching } = useQuery<
    AppointmentModel[],
    Error
  >("appointmentData", () =>
    axios
      .get(`http://scheduler-server:5152/EWSApiScheduler?startD=${currentDate}`)
      .then((res) => res.data)
  );

  if (isLoading) return <div>Loading...</div>;

  if (error) return <div>An error has occurred: + {error.message}</div>;

  return (
    <Paper>
      <Scheduler data={data}>
        <ViewState currentDate={currentDate} />
        <DayView startDayHour={9} endDayHour={14} />
        <Appointments />
      </Scheduler>
    </Paper>
  );
}

