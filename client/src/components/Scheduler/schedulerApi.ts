import { AppointmentModel } from "@devexpress/dx-react-scheduler";
import axios from "axios";
import { useMutation, useQuery, useQueryClient } from "react-query";



// API call in a service file and using that API call in the component
const devAppos = (currentDate:Date) =>
axios
.get(
  `http://localhost:5152/EWSApiScheduler/GetAppos?CalendarId=AQMkADAwATNiZmYAZC0zNThjLWYyNzItMDACLTAwCgAuAAADVsPWUeK5pEqLP2nAwws2hwEAru%2F256NrUUC1fGm0RnQ7hQAAAgENAAAA&startD=${currentDate.toISODate()}`      
 )
.then((res) => res.data)
//Custom Hooks for each API call in a service 
export function useAppos(currentDate:Date) 
{
  return useQuery<AppointmentModel[],Error>
    (["appointmentData",currentDate],()=> devAppos(currentDate))
}

const postAppo =(Appo:AppointmentModel)=>{
  console.log('appo: ')
  console.log(Appo)
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





