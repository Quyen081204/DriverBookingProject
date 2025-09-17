/**
 * Define collection of reducer functions and action to handle update current trip user is taking
 * 
 */

import { createSlice, PayloadAction } from "@reduxjs/toolkit"
import { Point } from "./bookingRequirementSlice"
import { RootState } from "../../store"

interface Vehicle {
    licensePlate: string | null,
    model: string | null,
    vehicleCapacity: number | null,
    vehicleType: string | null
}

interface Driver {
    id: number | null,
    fullName: string | null,
    phoneNumber: string | null,
    profileAvatarUrl: string | null,
    vehicle: Vehicle
}

interface Trip {
    id: string | null,
    startTime: Date |null,
    status: string | null,
    driver: Driver | null,
    currentLocation: Point | null
    departAddress: string | null,
    destAddress: string | null,
    price: number | null,
    paymentMethod: string | null,
    distance: number | null,
}

const initialState:Trip = {
    id: null,
    startTime: null,
    status: null,
    driver: null,
    currentLocation: null,
    departAddress: null,
    destAddress: null,
    price: null,
    paymentMethod: null,
    distance: null,
}

const tripSlice = createSlice({
    name:"trip",
    initialState,
    reducers: {
        // Chuyến xe tìm được tài xế
        driverFound: (state, action:PayloadAction<Trip>) => {
            return action.payload;
        },
        // Tài xế cập nhật vị trí
        driverLocationUpdated: (state, action:PayloadAction<Point>) => {
            state.currentLocation = action.payload;
        },
        // Taì xế cập nhật trạng thái chuyến đi
        tripStatusUpdated: (state, action:PayloadAction<string>) => {
            state.status = action.payload;
        },
        // Kết thúc chyến thì reset
        tripEnded: (state) => {
            return initialState;
        }
    }
})

// export action createtor 
export const {driverFound, driverLocationUpdated, tripStatusUpdated} = tripSlice.actions;
// export reducer
export default tripSlice.reducer;
// export selector
export const selectTripDepartAddress = (state: RootState) => state.currentTrip.departAddress;
export const selectTripDestAddress = (state: RootState) => state.currentTrip.destAddress;
export const selectTrip = (state:RootState) => state.currentTrip;
