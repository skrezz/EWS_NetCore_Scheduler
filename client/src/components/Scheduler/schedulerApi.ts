import { AppointmentModel } from "@devexpress/dx-react-scheduler";
import axios from "axios";
import { useMutation, useQuery, useQueryClient } from "react-query";
import { API_BASE_URL, ICalendar } from "../Support/Models";


//get appos
export const useGetAppos = (
  currentDate: Date,
  calIds: string[] | undefined,
  currentViewState:string,
  CalStatus: string
) => {
  localStorage.setItem('SelectedBaseCals',JSON.stringify(calIds)) 
  return useQuery<AppointmentModel[], Error>(
    ['appointmentData', calIds,currentDate,currentViewState],
    () => getAppos(currentDate, calIds!,currentViewState),
    { enabled: (CalStatus=="success"),refetchOnWindowFocus: false}
  );
};
const getAppos = async (currentDate: Date, calIds: string[],currentViewState:string) => {
  axios.defaults.headers.post['Authorization'] = "Bearer "+localStorage.getItem('accessToken');
  const response = await axios.post(
    `${API_BASE_URL}/GetAppos?startD=${currentDate.toISODate()}&currentViewState=${currentViewState}`,
    [calIds,localStorage.getItem('userLogin')]
  );
  console.log("0-")
  return response.data;
};

//Post appointments
const postAppo = async (Appo: AppointmentModel) => {
  const response = await axios.post(`${API_BASE_URL}/PostAppos`, Appo);
  return response.data;
};

//get Calendars
const getCalendars = async (
  isLogError:boolean
) : Promise<ICalendar[]> => {  
  axios.defaults.headers.post['Authorization'] = "Bearer "+localStorage.getItem('accessToken');
  const response = await axios.post(`${API_BASE_URL}/GetCalendars`,[localStorage.getItem('userLogin'),localStorage.getItem('refreshToken')]);
 
  if(response.data==null)
  {        
    localStorage.setItem('accessToken',"")
  }
  return response.data;
};

export const useCalendars = (
  isLogError:boolean,
  LogIsLoading:boolean
) => {
  return useQuery<ICalendar[], Error>(["availableCalendars"], () => getCalendars(isLogError), {enabled: (LogIsLoading&&isLogError),refetchOnWindowFocus: false});
}

export const usePostAppo = () => {
  const queryClient = useQueryClient();
  return useMutation(postAppo, {
    onSuccess: () => {
      queryClient.invalidateQueries("appointmentData");
    },
  });
};

//auth
export const authStart=async(
  Log:string,
  Pass:string
)=>{
  const response = await axios.post(`${API_BASE_URL}/PostAppos`, [Log,Pass]);
  return response.data;
}
