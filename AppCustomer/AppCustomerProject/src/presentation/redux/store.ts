import { Action, ThunkAction, configureStore } from '@reduxjs/toolkit'
import userReducer from "./features/user/userSlice"

export const store = configureStore({
    reducer: {
        // user state will be updated by userReducer function when actions are dispatched
        userState: userReducer
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