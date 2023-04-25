import { AppointmentModel } from "@devexpress/dx-react-scheduler";
import axios from "axios";
import { useMutation, useQuery, useQueryClient } from "react-query";
import { ICalendar } from "../Support/Models";

const API_BASE_URL = "http://localhost:5152/EWSApiScheduler";



//get appos
const getAppos = async (currentDate: Date, calIds: string[]) => {
  const response = await axios.post(
    `${API_BASE_URL}/GetAppos?startD=${currentDate.toISODate()}`,
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

export const useGetAppos = (
  currentDate: Date,
  calIds: string[] | undefined,
  CalIsLoading: boolean
  //changesCommited: number
) => {
  return useQuery<AppointmentModel[], Error>(
    ['appointmentData', calIds,currentDate],
    () => getAppos(currentDate, calIds!),
    { enabled: CalIsLoading,refetchOnWindowFocus: false}
  );
};

export const usePostAppo = () => {
  const queryClient = useQueryClient();
  return useMutation(postAppo, {
    onSuccess: () => {
      queryClient.invalidateQueries("appointmentData");
    },
  });
};
