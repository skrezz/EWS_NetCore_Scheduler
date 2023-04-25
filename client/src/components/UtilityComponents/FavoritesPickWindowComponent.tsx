import { Box, Modal, Paper } from "@mui/material";
import { CheckBoxRender } from "./CheckBoxListComponet";


  
const style = {
    position: 'absolute' as 'absolute',
    top: '50%',
    left: '50%',
    transform: 'translate(-50%, -50%)',
    width: 400,
    bgcolor: 'background.paper',
    border: '2px solid #000',
    boxShadow: 24,
    p: 4,
  };

export function OpenFavsWindow(CalData:any,selectedFavCalendars:string[],favsWinOpen:boolean,handleFavsWinClose:any,handleSelectedFavCalendars:any)
{     
    return(
        <Paper className="FavsWindow">
            <Modal
            open={favsWinOpen}
            onClose={handleFavsWinClose}
            aria-labelledby="parent-modal-title"
            aria-describedby="parent-modal-description"
            >            
                <Box sx={{ ...style, width: 400 }}>
                    {CalData?.map((calendar:any) => {
                        selectedFavCalendars.indexOf(calendar.calId)>-1?calendar.checkedFav=true:calendar.checkedFav=false
                    return (
                        <CheckBoxRender
                        key={calendar.calId}
                        calendar={calendar}
                        fromFavsWindow={true}                        
                        handleChanges={handleSelectedFavCalendars}                        
                        />
                    );
                    })}
                </Box>
            </Modal>
         </Paper>
    )
}