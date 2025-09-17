/**
 * Đây là slice dùng để xử lý sự kiện liên quan đến user input location trong màn hình pickingUp và
 * màn hình getCurrentLocation, xử lý dispatch từ nút bút chì
 * 
 */
import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import type { RootState } from '../../store'


const initialState:string = "";

const userLocationPickingSlice = createSlice({
    name:"userLocationInput",
    initialState,
    reducers: {
        userLocationPickingUpdated: (state, action:PayloadAction<string>) => {
           return action.payload;
        }
    }
})

export default userLocationPickingSlice.reducer;
export const {userLocationPickingUpdated} = userLocationPickingSlice.actions;
export const selectUserLocationPicking = (state:RootState) => state.userLocationPicking;
