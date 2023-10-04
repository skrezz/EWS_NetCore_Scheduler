import { AppointmentModel } from "@devexpress/dx-react-scheduler";
import axios from "axios";
import { useMutation, useQuery, useQueryClient } from "react-query";
import { ICalendar } from "../Support/Models";

const API_BASE_URL = "https://scheduler.demo.com/api/EWSApiScheduler";



//get appos
export const useGetAppos = (
  currentDate: Date,
  calIds: string[] | undefined,
  currentViewState:string,
  CalIsLoading: boolean
  //changesCommited: number
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
  const response = await axios.get(`${API_BASE_URL}/GetCalendars`);
  return response.data;
};

export const useCalendars = () => {
  return useQuery<ICalendar[], Error>(["availableCalendars"], () => getCalendars(), {refetchOnWindowFocus: false});
}



export const usePostAppo = () => {
  const queryClient = useQueryClient();
  return useMutation(postAppo, {
    onSuccess: () => {
      queryClient.invalidateQueries("appointmentData");
    },
  });
};
