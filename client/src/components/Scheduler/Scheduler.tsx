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

import {useCalendars,useGetAppos,usePostAppo} from "./schedulerApi";
import { CalModel } from "../Support/Models";
import {CheckBoxRender,BasicLayout,SelectedCal} from "./UtilityComponents"

export let calTitles:string[]=[]
export let calIds:string[]=['','']

export function DevScheduler() { 
 
  const [currentDate, setCurrentDate] = React.useState(new Date());


  const { isLoading:CalIsLoading, error:CalError, data:CalData }  = useCalendars()
  
  
  if(!CalIsLoading)
  {
    calTitles=CalData!.map((cal:CalModel)=>{
      return cal.title
    }) 
  }
//CheckBoxes Controller
  let [checkBoxState, setCheckBoxesState] = React.useState(new Array(200).fill(true))
  const handleOnChange = (position:number) => {
  const updatedCheckedState = checkBoxState.map((item, index) =>
      index === position ? !item : item
      );
        setCheckBoxesState(updatedCheckedState);          
      }
//Get Appos 
    
    if(!CalIsLoading)
    {
    calIds=CalData!.map((cal:CalModel,index)=>{    
      if(checkBoxState[index])
      {        
      return cal.calId
      }
      return ''
    })  
    }    
   
    const { isLoading, error, data, isFetching }= useGetAppos(currentDate,calIds,!CalIsLoading) 

//Post/change/delete Appos 
  const { mutate}=usePostAppo() 

  function commitChanges(changes:ChangeSet){  
    let appoTmp:AppointmentModel={
        startDate:""
      }  
    if (changes.added) {
             mutate({ startDate: currentDate, calId:calIds[SelectedCal], ...changes.added})       
    }
    if (changes.changed) {      
          
       data!.forEach(appo=>{
        if(changes!.changed![appo.id!])
        {
         appoTmp.id=appo.id       
         appoTmp.title=changes!.changed![appo.id!].title
         appoTmp.startDate=changes!.changed![appo.id!].startDate
         appoTmp.calId=appo.calId
        }        
      })      
      mutate(appoTmp)
      }
      if (changes.deleted !== undefined) {
        appoTmp.id=changes.deleted
        appoTmp.title="deleteIt"
        mutate(appoTmp)
      }
  }

    if (isLoading) return <div>Loading...</div>;
    if (CalIsLoading) return <div>Loading...</div>;
    if (error) return <div>An error has occurred: + {error.message}</div>; 

    
  return (
    <div>
       <div className="CheckBoxesPanel">
        {CheckBoxRender(calTitles,checkBoxState,handleOnChange)}
        </div>
    <Paper>      
      <Scheduler data={data}>
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
          <AppointmentForm 
          basicLayoutComponent={BasicLayout}          
          />

      </Scheduler>
    </Paper>  
    </div>
  );
}