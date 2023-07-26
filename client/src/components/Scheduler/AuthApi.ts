import axios from "axios";
import { useQuery } from "react-query";
import { API_BASE_URL } from "../Support/Models";

let axiosConfig = {
  headers: {    
    'Authorization': "Bearer "+sessionStorage.getItem('accessToken')
  }
};

export const RegUser = async(
        Log:string,
        Pass:string
      )=>{
        const response = await axios.post(`${API_BASE_URL}/RegUser`, [Log,Pass]);
        console.log(response.statusText)
        if(response.status==200)
        {
          sessionStorage.setItem('accessToken',response.data.access_token)
        }
        else
          return response.statusText;
      }
export const LogUser = async()=>{
        axios.defaults.headers.post['Authorization'] = "Bearer "+sessionStorage.getItem('accessToken');
        const response = await axios.post(`${API_BASE_URL}/LogUser`,{her:"her"});        
        if(response.status==200)
        {
          console.log("CONGRATULATIONS")
        }
        else
          return response.statusText;
      }