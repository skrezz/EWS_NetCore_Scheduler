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
  WeekView,
  MonthView,
  ViewSwitcher,
} from "@devexpress/dx-react-scheduler-material-ui";

import { authStart, useCalendars, useGetAppos, usePostAppo } from "./schedulerApi";
import { PlaceHolder } from "../UtilityComponents/PlaceholderComponet";
import { CheckBoxRender } from "../UtilityComponents/CheckBoxListComponet";
import { ICalendar } from "../Support/Models";
import { BasicLayout, } from "../UtilityComponents/BasicLayoutComponent";
import { Box, Button, Modal, TextField, formControlClasses } from "@mui/material";
import { json } from "stream/consumers";
import { useRef } from "react";
import { useLogUser, RegUser } from "./AuthApi";
import { useQueryClient } from "react-query";



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
  m:1,
};

export function DevScheduler() {
  //это для мануального обновления query на логин
const queryClient = useQueryClient()
const reLog = () => {
     queryClient.invalidateQueries("availableCalendars")
}
  //стейты логина и пароля
  const [lgn, setLgn] = React.useState("");
  const handleLgnChange = (lgnValue:string) => { 
    setLgn(lgnValue);      
    }
  const [pwd, setPwd] = React.useState("");
  const handlePwdChange = (pwdValue:string) => { 
    setPwd(pwdValue);      
      }
  //стейты для окна авторизации
  const [authWinOpen, setAuthWinOpen] = React.useState(true);
  const handleAuthWinOpen = () => {
    setAuthWinOpen(true);   
  };
  const handleAuthWinClose = () => {   
    setAuthWinOpen(false);
  }; 
  const handleRegdone = (LogIsDone:boolean) => {  
    RegUser(lgn,pwd)   
    reLog()
    if(!LogIsDone) setAuthWinOpen(false);
  }; 

  //дата
  const [currentDate, setCurrentDate] = React.useState(new Date());
  //вид
  const [currentViewState, setCurrentViewState] = React.useState("Month");
  //выбранные календари
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
  //для окна изброанных календарей
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
  //стейты для открытия окна избранных календарей
  const [favsWinOpen, setFavsWinOpen] = React.useState(false);
  const handleFavsWinOpen = () => {
    setFavsWinOpen(true);   
  };
  const handleFavsWinClose = () => {
    localStorage.setItem('SelectedFavCals',JSON.stringify(selectedFavCalendars))
    setFavsWinOpen(false);
  };
//Логинимся



//тут жу точно логинимся
  const {
    isLoading: LogIsLoading,
    error: LogError,
    data: LogData,
    status:LogStatus, 
    isError:isLogError,
    isFetching:LogIsFetching
  } = useLogUser("logUser","LogUser",true,authWinOpen);
  const {
    isLoading: CalIsLoading,
    error: CalError,
    data: CalData,
    status:CalStatus,    
    isPreviousData: calsNotChanging    
  } = useCalendars(isLogError);
  //чекаем рефреш токен
  const {
    isLoading: refTokenCheckIsLoading,
    error: refTokenCheckError,
    data: refTokenCheckData,
    status:refTokenCheckStatus, 
    isError:isrefTokenCheckError,
  } = useLogUser("refTokenCheck","RefToken",isLogError,false)
  
  
  const { isLoading, error, data, isFetching } = useGetAppos(
    currentDate,
    selectedCalendars,
    currentViewState,
    CalStatus    
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

  
  if (CalIsLoading||LogIsLoading||refTokenCheckIsLoading) return <PlaceHolder />;
  if (localStorage.getItem('regDone')?.toString()!="regDone")
  {
 
      if (refTokenCheckIsLoading) return <PlaceHolder />;
      if(refTokenCheckData=="refTokenFail")     
      return (
      <Paper className="AuthWindow">
          <Modal
          open={authWinOpen}   
          onClose={handleAuthWinClose}         
          aria-labelledby="parent-modal-title"
          aria-describedby="parent-modal-description"
          >
            <Box sx={{ ...styleFavsWindow, width: 400 }}>
            <div>
            
            <TextField  fullWidth                  
              id="standard-login"
              label="Логин"
              type="login"
              variant="standard"
              margin="dense"
              onChange={(e)=>handleLgnChange(e.target.value)}
            /> 
            <TextField fullWidth            
              id="standard-password-input"
              label="Пароль"
              type="password"                
              variant="standard"
              margin="dense"
              onChange={(e)=>handlePwdChange(e.target.value)}
            />
            </div>
            <Button 
            variant="outlined"
            //onClick={()=>console.log("log: "+loginRef.current)}
            onClick={(e)=>handleRegdone(isLogError)
              /*()=>{                
               RegUser(lgn,pwd)
               reLog() 
               }*/
            }
            >
            ok   
            </Button>
            <Button 
            variant="outlined"
            //onClick={()=>console.log("log: "+loginRef.current)}
            //onClick={()=>useLogUser()}
            >
            testToken   
            </Button>
            </Box>
          </Modal>
      </Paper>
      )
      


    return (
      <div>
        An error has occurred: +{" "}
        {error === null ? CalError!.message : error!.message}
        
      </div>
     
    );
  }
  
const CalDataBase =CalData?.filter((calendar)=>{
return selectedFavCalendars.indexOf(calendar.calId)>-1

})

  return (
   
    <React.Fragment>
      <Paper className="header">      
        {
          
        CalDataBase?.map((calendar) => {
          if(authWinOpen) setAuthWinOpen(false)
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
              onCurrentViewNameChange={(currentViewState) => setCurrentViewState(currentViewState)}
              currentViewName={currentViewState}              
            />
            <EditingState
              onCommitChanges={(changes) => commitChanges(changes)}
            />
            <IntegratedEditing />
            <DayView startDayHour={9} endDayHour={19} />
            <WeekView startDayHour={9} endDayHour={19}/>
            <MonthView />
            <ConfirmationDialog />
            <Toolbar />
            <DateNavigator />
            <TodayButton />
            <ViewSwitcher />
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
