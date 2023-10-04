import { AppointmentModel } from "@devexpress/dx-react-scheduler";
import axios from "axios";
import { useMutation, useQuery, useQueryClient } from "react-query";
import { ICalendar, API_BASE_URL } from "../Support/Models";




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



//get Calendars
const getCalendars = async () : Promise<ICalendar[]> => {  
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
  return useQuery<ICalendar[], Error>(["availableCalendars",isLogError], () => getCalendars(), {enabled: (isLogError&&LogIsLoading),refetchOnWindowFocus: false});
}
//Post appointments

export const usePostAppo = () => {
  const queryClient = useQueryClient();
  return useMutation(postAppo, {
    onSuccess: () => {
      sessionStorage.removeItem('AppoToPostTmp')
      queryClient.invalidateQueries("appointmentData");
    },
    onError:(err: Error) => { 
      if(err.message.includes("401")) 
      {
        queryClient.invalidateQueries("logUser");
      }
    },
  });
};
const postAppo = async (Appo: AppointmentModel) => {  
  axios.defaults.headers.post['Authorization'] = "Bearer "+localStorage.getItem('accessToken');
  const response = await axios.post(`${API_BASE_URL}/PostAppos`, [Appo,localStorage.getItem('userLogin')]);

  return response.data;
};

