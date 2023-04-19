import * as React from "react";
import "../../utils/date-extension";
import Paper from "@mui/material/Paper";

import {
  AppointmentModel,
  ViewState,
  EditingState,
  IntegratedEditing,
  ChangeSet,
} from "@devexpress/dx-react-scheduler";
import {
  Scheduler,
  DayView,
  Appointments,
  AppointmentForm,
  AppointmentTooltip,
  ConfirmationDialog,
  Toolbar,
  DateNavigator,
  TodayButton,
} from "@devexpress/dx-react-scheduler-material-ui";

import { useCalendars, useGetAppos, usePostAppo } from "./schedulerApi";
import { PlaceHolder } from "../UtilityComponents/PlaceholderComponet";
import { CheckBoxRender } from "../UtilityComponents/CheckBoxListComponet";
import { ICalendar } from "../Support/Models";
import { BasicLayout } from "../UtilityComponents/BasicLayoutComponent";
import { Box } from "@mui/material";

export function DevScheduler() {
  const [currentDate, setCurrentDate] = React.useState(new Date());

  const [selectedCalendars, setSelectedCalendars] = React.useState<string[]>(
    []
  );

  const {
    isLoading: CalIsLoading,
    error: CalError,
    data: CalData,
    isPreviousData: calsNotChanging,
  } = useCalendars();

  const { isLoading, error, data, isFetching } = useGetAppos(
    currentDate,
    selectedCalendars,
    !CalIsLoading
    // changesCommited
  );

  //Post/change/delete Appos
  const postAppoinment = usePostAppo();

  const handleSelectedCalendars = (calendar: ICalendar, checked: boolean) => {
    if (checked) {
      setSelectedCalendars([...selectedCalendars, calendar.calId]);
    } else {
      setSelectedCalendars(
        selectedCalendars.filter((cal) => cal !== calendar.calId)
      );
    }
  };

  function commitChanges(changes: ChangeSet) {
    let appoTmp: AppointmentModel = {
      startDate: "",
    };
    if (changes.added) {
      postAppoinment.mutate({
        startDate: currentDate,
        ...changes.added,
      });
    }
    if (changes.changed) {
      console.log(changes);
      data!.forEach((appo) => {
        if (changes!.changed![appo.id!]) {
          appoTmp.id = appo.id;
          appoTmp.title = changes!.changed![appo.id!].title;
          appoTmp.startDate = changes!.changed![appo.id!].startDate;
          appoTmp.calId = appo.calId;
        }
      });
      postAppoinment.mutate(appoTmp);
    }
    if (changes.deleted !== undefined) {
      appoTmp.id = changes.deleted;
      appoTmp.title = "deleteIt";
      postAppoinment.mutate(appoTmp);
    }
  }

  if (CalIsLoading) return <PlaceHolder />;
  if (error) return <div>An error has occurred: + {error.message}</div>;

  return (
    <Paper>
      <Box>
        <Box sx={{ display: "flex" }}>
          {CalData?.map((calendar) => {
            return (
              <CheckBoxRender
                key={calendar.calId}
                calendar={calendar}
                handleChanges={handleSelectedCalendars}
              />
            );
          })}
        </Box>
        {isLoading || isFetching ? (
          <PlaceHolder />
        ) : (
          <Scheduler data={data}>
            <ViewState
              currentDate={currentDate}
              onCurrentDateChange={(currentDate) => setCurrentDate(currentDate)}
            />
            <EditingState
              onCommitChanges={(changes) => commitChanges(changes)}
            />
            <IntegratedEditing />
            <DayView startDayHour={9} endDayHour={19} />
            <ConfirmationDialog />
            <Toolbar />
            <DateNavigator />
            <TodayButton />
            <Appointments />
            <AppointmentTooltip showOpenButton showDeleteButton />
            <AppointmentForm
              basicLayoutComponent={(props) => (
                <BasicLayout {...props} calData={CalData} />
              )}
            />
          </Scheduler>
        )}
      </Box>
    </Paper>
  );
}
