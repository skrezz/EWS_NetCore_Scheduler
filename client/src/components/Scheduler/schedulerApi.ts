import { AppointmentModel } from "@devexpress/dx-react-scheduler";
import axios from "axios";
import { useMutation, useQuery, useQueryClient } from "react-query";
import Checkbox from '@mui/material/Checkbox';


//get Calendars
const getCalendars = () =>
axios
.get(
  `http://localhost:5152/EWSApiScheduler/GetCalendars`      
 )
.then((res) => res.data)

export function useCalendars() 
{
  return useQuery<[],Error>
    (["availableCalendars"],()=> getCalendars())
}


// Get Appointments
/*const devAppos = (currentDate:Date,calIds:string[]) =>
  axios
  .post(
    `http://localhost:5152/EWSApiScheduler/GetAppos`,{startD:currentDate.toISODate(),CalendarIds:calIds}      
   )
  .then((res) => res.data)
  //Custom Hooks for each API call in a service 
export function useAppos(currentDate:Date,calIds:string[])
{  
    return useQuery<AppointmentModel[],Error>
    (["appointmentData",currentDate],()=> getAppos(currentDate,calIds),{enabled: !!calIds})

}
*/
 
  const getAppos =(currentDate:Date,calIds:string[])=>{
    //console.log('appo: ')
    //console.log(Appo)
    //return axios.post(`http://localhost:5152/EWSApiScheduler/GetAppos?startD=${currentDate.toISODate()}`,{calIds})
    return axios.post(`http://localhost:5152/EWSApiScheduler/GetAppos?startD=${currentDate.toISODate()}`,calIds)
    .then((res) => res.data)
  }
  export const useGetAppos=(currentDate:Date,calIds:string[],CalIsLoading:boolean)=>
{  
  return useQuery<AppointmentModel[],Error>
    (["appointmentData",currentDate,calIds],()=> getAppos(currentDate,calIds),{enabled: CalIsLoading})
}




//Post appointments
const postAppo =(Appo:AppointmentModel)=>{
  //console.log('appo: ')  
  /*if(Appo.id!="")
  {
    Appo.id="id:"+Appo.id
  }*/
  
  //console.log(Appo)
  return axios.post('http://localhost:5152/EWSApiScheduler/PostAppos',Appo)
}
export const usePostAppo=()=>
{
  const queryClient = useQueryClient()
  return useMutation(postAppo,
    {
      onSuccess:()=>
      {
        queryClient.invalidateQueries('appointmentData')
      }
    })
}





