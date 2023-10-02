import axios from "axios";
import { useQuery, useQueryClient } from "react-query";
import { API_BASE_URL } from "../Support/Models";
import { DevScheduler} from "./Scheduler";

//const queryClient = useQueryClient();
let axiosConfig = {
  headers: {    
    'Authorization': "Bearer "+sessionStorage.getItem('accessToken')
  }
};
/*export const useRegUser = (
  Log:string,
  Pass:string
  ) => {
  return useQuery<Promise<string | undefined>, Error>(["regUser",Log,Pass], () => RegUser(Log,Pass), {refetchOnWindowFocus: false});
}*/
export const RegUser = async (               
        Log:string,
        Pass:string             
        )=>{   
        const response = await axios.post(`${API_BASE_URL}/RegUser`, [Log,Pass,localStorage.getItem('refreshToken')]);
       
        if(response.data!="WrongCreds")
        { 
          localStorage.setItem('accessToken',response.data.value.access_token)   
          localStorage.setItem('refreshToken',response.data.value.refresh_token) 
          localStorage.setItem('userLogin',Log)
          localStorage.setItem('regDone',"regDone")       
       
        }
        else
        {
     
          localStorage.setItem('regDone',"regFail")
          return response.statusText;          
        }
       
      
        return response.data;
      }
export const useLogUser = (
  queryName:string,
  backAdress:string,
  isLogError:boolean,
  authWinOpen:boolean
  ) => {
        return useQuery<string, Error>([queryName,authWinOpen,isLogError], () => LogUser(backAdress), {enabled: isLogError,refetchOnWindowFocus: false, retry: 1});
      }
const LogUser = async(
  backAdress:string,  
)=>{
        axios.defaults.headers.post['Authorization'] = "Bearer "+localStorage.getItem('accessToken');
        const response = await axios.post(`${API_BASE_URL}/${backAdress}`,[localStorage.getItem('userLogin'),localStorage.getItem('refreshToken')]); 
        if(response.status==200)
        {         
          if(backAdress=="RefToken")
          {
          localStorage.setItem('accessToken',response.data)
          } 
          else
          {
          console.log("Logged") 
          }
        }
        else
        {
          console.log("Refresh Token error") 
    
          return response.statusText;
          
        }

        return response.data;
      }