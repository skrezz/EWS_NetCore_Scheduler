import * as React from "react";
import "../../utils/date-extension"
import Paper from "@mui/material/Paper";
import { useMutation, useQuery } from "react-query";
import axios from "axios";
import {
  AppointmentModel,
  ViewState,
  EditingState, 
  IntegratedEditing,
  ChangeSet
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

import {useAppos,usePostAppo} from "./schedulerApi";

export function DevScheduler() { 
  console.log('start')
  const [currentDate, setCurrentDate] = React.useState(new Date()); 
  const [dataFront, dataFrontChanges] = React.useState(Array<AppointmentModel>);

  const { mutate}=usePostAppo() 

    function commitChanges(changes:ChangeSet){  
      if (changes.added) {
         mutate({ startDate: currentDate, ...changes.added})
      }
    }
  const { isLoading, error, data, isFetching } = useAppos(currentDate)
  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>An error has occurred: + {error.message}</div>;

  
        /*const { mutate, isLoading, error } = useMutation<AppointmentModel[],Error>(
          (data)=>axios.post('https://localhost:7151/EWSApiScheduler/PostAppos', 
          {
          startDate: currentDate,
          title: 'FrontTest10'   
          },
            {headers: 
              {
                'accept': 'text/plain',
                'Content-Type': 'application/json'                
              }
            }
          ),
          {
            onSuccess: (data:AppointmentModel[])=>{useAppos(currentDate)}
          ,
          onError: (error) => {console.log(error);}
          }
          )  
        //dataFrontChanges([...dataFront, { startDate: currentDate, ...changes.added }])*/
    
    

  return (
    <Paper>      
      <Scheduler data={[...data!,...dataFront]}>
        <ViewState
          currentDate={currentDate}
          onCurrentDateChange={(currentDate) => setCurrentDate(currentDate)}          
        />
        <EditingState          
            onCommitChanges={(changes)=>commitChanges(changes)}            
          />
        <IntegratedEditing />
        <DayView startDayHour={9} endDayHour={19} />
        <ConfirmationDialog />
        <Toolbar />
        <DateNavigator />
        <TodayButton />
        <Appointments />
        <AppointmentTooltip
            showOpenButton
            showDeleteButton
          />
          <AppointmentForm />
      </Scheduler>
    </Paper>
  );
}