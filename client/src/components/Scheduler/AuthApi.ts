import axios from "axios";
import { useQuery } from "react-query";
import { API_BASE_URL } from "../Support/Models";
import { DevScheduler } from "./Scheduler";

let axiosConfig = {
  headers: {    
    'Authorization': "Bearer "+sessionStorage.getItem('accessToken')
  }
};
let refDone:boolean=false;
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
        console.log(response.statusText)
        if(response.status==200)
        {       
          localStorage.setItem('accessToken',response.data.value.access_token)   
          localStorage.setItem('refreshToken',response.data.value.refresh_token) 
          localStorage.setItem('userLogin',Log)       
        }
        else
          return response.statusText;
          
        return response.data;
      }
export const useLogUser = (
  queryName:string,
  backAdress:string,
  isLogError:boolean,  
  ) => {
        return useQuery<string, Error>([queryName,refDone,isLogError], () => LogUser(backAdress), {enabled: isLogError,refetchOnWindowFocus: false});
      }
const LogUser = async(
  backAdress:string,  
)=>{
        axios.defaults.headers.post['Authorization'] = "Bearer "+localStorage.getItem('accessToken');
        const response = await axios.post(`${API_BASE_URL}/${backAdress}`,[localStorage.getItem('userLogin'),localStorage.getItem('refreshToken')]);  
        console.log("3")      
        if(response.status==200)
        {         
          if(backAdress=="RefToken")
          {
          localStorage.setItem('accessToken',response.data)
          refDone =true;  
          console.log("Access Token Refreshed") 
          } 
          else
          console.log("Logged")  
        }
        else
        {
          console.log("Refresh Token error") 
          refDone=false
          return response.statusText;
          
        }

        return response.data;
      }