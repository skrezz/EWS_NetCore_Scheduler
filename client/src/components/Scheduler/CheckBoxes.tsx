import {Box, FormControlLabel, Checkbox} from '@mui/material'
import React from 'react'
import { CalModel } from '../Support/Models'



export function CheckBoxRender(calTitles:string[],checkBoxState:any[],handleOnChange: (position: number) => void)
{    


    
    return(          
        <Box>
            <Box>
                {calTitles.map((title:string,index)=>{
                    return (
                    <FormControlLabel 
                    key={index}
                    id={`${index}`}
                    label= {title}                    
                    control=<Checkbox />
                    checked={checkBoxState[index]}
                    onChange={() => handleOnChange(index)}
                    />)
                }
                )}   
            </Box>
        </Box>        
    )
}