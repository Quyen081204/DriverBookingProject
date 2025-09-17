/**
 * Define collection of reducer functions and action to handle update booking requirement
 * 
 */
import { createSlice, PayloadAction } from "@reduxjs/toolkit"
import { RootState } from "../../store"

export interface Point {
    lat: number,
    long: number
}


interface BookingRequirementType {
    depart: Point | null,
    dest: Point | null,
    departAddress: string | null,
    destAddress: string | null,
    requestVehicleCapacity: number | null,
    // 0 is SAME for motorbike, 1 is Luxury, 2 is Normal
    requestVehicleType: number | null,
    customerNote: string | null,
    price: number | null,
    distance: number | null,
    paymentMethod: string | null
}


const initialState: BookingRequirementType = {
    depart: null,
    dest: null,
    departAddress: null,
    destAddress: null,
    requestVehicleCapacity: null,
    requestVehicleType: 0,
    customerNote: null,
    price: null,
    distance: null,
    paymentMethod: null
}

export interface Location {
    locationPoint: Point,
    locationAddress: string
}

interface VehiclePaymentMethodPayload {
    requestVehicleCapacity: number,
    requestVehicleType: number,
    paymentMethod: string,
    price: number
}

const bookintRequireSlice = createSlice({
    name: 'bookingRequirement',
    initialState,
    reducers: {
        // cập nhật nơi đón cho khách hàng (xin được quyền truy cập, dùng tự động lấy vị trí, con trỏ, người dùng nhập)
        departLocationGetted: (state, action: PayloadAction<Location>) => {
            state.depart = action.payload.locationPoint;
            state.departAddress = action.payload.locationAddress;
        },
        // cập nhật nơi trả khách
        destLocationGetted: (state, action: PayloadAction<Location>) => {
            state.dest = action.payload.locationPoint;
            state.destAddress = action.payload.locationAddress;
        },
        // cập nhật xe khách muốn đặt
        vehicleCapacityGetted: (state, action:PayloadAction<number>) => {
            state.requestVehicleCapacity = action.payload;
        },
        // cập nhật ghi chú của khách
        customerNoteGetted: (state, action: PayloadAction<string>) => {
            state.customerNote = action.payload;
        },
        // hoàn thành chọn loại xe phù hợp và phương thức thanh toán, giá tiền
        vehiclePricePaymentMethodeCompleted: (state, action: PayloadAction<VehiclePaymentMethodPayload>) => {
            state.requestVehicleCapacity = action.payload.requestVehicleCapacity;
            state.requestVehicleType = action.payload.requestVehicleType;
            state.paymentMethod = action.payload.paymentMethod;
            state.price = action.payload.price
        },
        // reset bookingRequirement
        bookingRequirementReseted: (state) => {
            return initialState;
        }
    }
})

// export action creator
export const { departLocationGetted, destLocationGetted, customerNoteGetted, vehiclePricePaymentMethodeCompleted } = bookintRequireSlice.actions;
// export reducer
export default bookintRequireSlice.reducer;
// export selector
export const selectDepartAddress = (state: RootState) => state.bookingRequirement.departAddress;
export const selectDestAddress = (state: RootState) => state.bookingRequirement.destAddress;
export const selectRequestVehicleCapacity = (state:RootState) => state.bookingRequirement.requestVehicleCapacity
