import { Action, ThunkAction, configureStore } from '@reduxjs/toolkit'
import userReducer from "./features/user/userSlice"
import bookingRequirementReducer from "./features/booking/bookingRequirementSlice"
import tripReducer from "./features/booking/tripSlice"
import userLocationPickingReducer from "./features/pickingLocation/userLocationPickingSlice"
//import connectionReducer from "./features/signalRConnection/connectionSlice"
export const store = configureStore({
    reducer: {
        // user state will be updated by userReducer function when actions are dispatched
        userState: userReducer,
        bookingRequirement: bookingRequirementReducer,
        currentTrip: tripReducer,
        userLocationPicking: userLocationPickingReducer
    }
})

// Infer the type of `store`
export type AppStore = typeof store
// Infer the `AppDispatch` type from the store itself
export type AppDispatch = typeof store.dispatch
// Same for the `RootState` type
export type RootState = ReturnType<typeof store.getState>
// Export a reusable type for handwritten thunks
export type AppThunk = ThunkAction<void, RootState, unknown, Action>