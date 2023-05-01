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
import { BasicLayout, } from "../UtilityComponents/BasicLayoutComponent";
import { Box, Button, Modal } from "@mui/material";
import { json } from "stream/consumers";

const styleFavsWindow = {
  position: 'absolute' as 'absolute',
  top: '50%',
  left: '50%',
  transform: 'translate(-50%, -50%)',
  width: 400,
  bgcolor: 'background.paper',
  border: '2px solid #000',
  boxShadow: 24,
  p: 4,
};

export function DevScheduler() {
  const [currentDate, setCurrentDate] = React.useState(new Date());

  const [selectedCalendars, setSelectedCalendars] = React.useState<string[]>(() => {
    const stickyValue = localStorage.getItem('SelectedBaseCals');
    return stickyValue !== null
      ? JSON.parse(stickyValue)
      : [];
  })

  const handleSelectedCalendars = (calendar: ICalendar, checked: boolean) => {
    if (checked) {
      setSelectedCalendars([...selectedCalendars, calendar.calId]);      
    }
    else {
      setSelectedCalendars(
        selectedCalendars.filter((cal) => cal !== calendar.calId)        
      );      
    }   
    
  };

  const [selectedFavCalendars, setSelectedFavCalendars] = React.useState<string[]>(() => {
    const stickyValue = localStorage.getItem('SelectedFavCals');
    return stickyValue !== null
      ? JSON.parse(stickyValue)
      : [];
  })

  const handleSelectedFavCalendars = (calendar: ICalendar, checked: boolean) => {
    if (checked) {
      setSelectedFavCalendars([...selectedFavCalendars, calendar.calId]);      
    } else {
      setSelectedFavCalendars(
        selectedFavCalendars.filter((cal) => cal !== calendar.calId)        
      );
      setSelectedCalendars(
        selectedCalendars.filter((cal) => cal !== calendar.calId)        
      );    
    } 

    
  };

  const [favsWinOpen, setFavsWinOpen] = React.useState(false);
  const handleFavsWinOpen = () => {
    setFavsWinOpen(true);   
  };
  const handleFavsWinClose = () => {
    localStorage.setItem('SelectedFavCals',JSON.stringify(selectedFavCalendars))
    setFavsWinOpen(false);
  };


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
  );

  //Post/change/delete Appos
  const postAppoinment = usePostAppo();

  
  
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
      data!.forEach((appo) => {
        if (changes!.changed![appo.id!]) {
          appoTmp.id = appo.id;
          appoTmp.title = changes!.changed![appo.id!].title;
          appoTmp.startDate = changes!.changed![appo.id!].startDate;
          if(changes!.changed![appo.id!].calId !==undefined)
            appoTmp.calId=changes!.changed![appo.id!].calId
          else
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
  if (error || CalError)
    return (
      <div>
        An error has occurred: +{" "}
        {error === null ? CalError!.message : error!.message}
      </div>
    );
const CalDataBase =CalData?.filter((calendar)=>{
return selectedFavCalendars.indexOf(calendar.calId)>-1
})
  return (
    <React.Fragment>
      <Paper className="header">
        {CalDataBase?.map((calendar) => {  
          selectedCalendars.indexOf(calendar.calId)>-1?calendar.checkedBase=true:calendar.checkedBase=false                 
          return (
            <CheckBoxRender
              key={calendar.calId}
              calendar={calendar}   
              checked={calendar.checkedBase}          
              handleChanges={handleSelectedCalendars}
            />
          );
        })}
        
      </Paper>
      <Paper className="FavButton">
      {      
        <Button 
        variant="outlined"
        onClick={handleFavsWinOpen}
        >
          Избранные календари   
        </Button>
      }      
      </Paper>
      <Paper className="FavsWindow">
            <Modal
            open={favsWinOpen}
            onClose={handleFavsWinClose}
            aria-labelledby="parent-modal-title"
            aria-describedby="parent-modal-description"
            >            
                <Box sx={{ ...styleFavsWindow, width: 400 }}>
                    {CalData?.map((calendar:any) => {
                        selectedFavCalendars.indexOf(calendar.calId)>-1?calendar.checkedFav=true:calendar.checkedFav=false
                    return (
                        <CheckBoxRender
                        key={calendar.calId}
                        calendar={calendar}   
                        checked={calendar.checkedFav}                                         
                        handleChanges={handleSelectedFavCalendars}                        
                        />
                    );
                    })}
                </Box>
            </Modal>
         </Paper>
      
      <Paper className="content">
        {isLoading || isFetching ? (
          <PlaceHolder />
        ) : (
          <Scheduler data={data} height={660}>
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
      </Paper>
    </React.Fragment>
  );
}
