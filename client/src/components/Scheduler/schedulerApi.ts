import { AppointmentModel } from "@devexpress/dx-react-scheduler";
import axios from "axios";
import { useMutation, useQuery, useQueryClient } from "react-query";
import { API_BASE_URL, ICalendar } from "../Support/Models";


//get appos
export const useGetAppos = (
  currentDate: Date,
  calIds: string[] | undefined,
  currentViewState:string,
  CalIsLoading: boolean
) => {
  localStorage.setItem('SelectedBaseCals',JSON.stringify(calIds)) 
  return useQuery<AppointmentModel[], Error>(
    ['appointmentData', calIds,currentDate,currentViewState],
    () => getAppos(currentDate, calIds!,currentViewState),
    { enabled: CalIsLoading,refetchOnWindowFocus: false}
  );
};
const getAppos = async (currentDate: Date, calIds: string[],currentViewState:string) => {
  const response = await axios.post(
    `${API_BASE_URL}/GetAppos?startD=${currentDate.toISODate()}&currentViewState=${currentViewState}`,
    calIds
  );
  return response.data;
};

//Post appointments
const postAppo = async (Appo: AppointmentModel) => {
  const response = await axios.post(`${API_BASE_URL}/PostAppos`, Appo);
  return response.data;
};

//get Calendars
const getCalendars = async () : Promise<ICalendar[]> => {
  const test:string="123"
  axios.defaults.headers.post['Authorization'] = "Bearer "+sessionStorage.getItem('accessToken');
  const response = await axios.post(`${API_BASE_URL}/GetCalendars`,[sessionStorage.getItem('userLogin')]);
  
  return response.data;
};

export const useCalendars = (
  LogIsLoading:boolean,
) => {
  return useQuery<ICalendar[], Error>(["availableCalendars"], () => getCalendars(), {enabled: LogIsLoading,refetchOnWindowFocus: false});
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
