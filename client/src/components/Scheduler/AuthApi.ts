import axios from "axios";
import { useQuery } from "react-query";
import { API_BASE_URL } from "../Support/Models";



export const RegUser = async(
        Log:string,
        Pass:string
      )=>{
        const response = await axios.post(`${API_BASE_URL}/RegUser`, [Log,Pass]);
        return response.data;
      }