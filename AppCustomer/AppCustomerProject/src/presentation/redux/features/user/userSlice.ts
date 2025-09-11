import { PayloadAction } from '@reduxjs/toolkit'
import { createSlice } from '@reduxjs/toolkit'
import type { RootState } from '../../store'
import { createAppAsyncThunk } from '../../hooks';
import { authApis, endpoints } from '../../../../data/api/clients';
import { ApiResponse } from '../../../../domain/models/ApiResponse';
import AsyncStorage from '@react-native-async-storage/async-storage';


export interface User {
    id: string | null,
    userName: string | null,
    email: string | null,
    phoneNumber: string | null,
    firstName: string | null,
    lastName: string | null,
    profileAvatarUrl: string | null
}

interface UserState {
    user: User,
    result: boolean,
    status: 'idle' | 'pending' | 'succeeded' | 'failed',
    error: string | null
}

// Create an initial state value for the reducer, with that type

const initialState: UserState = {
    user: {
        id: null,
        userName: null,
        email: null,
        phoneNumber: null,
        firstName: null,
        lastName: null,
        profileAvatarUrl: null
    },
    status: 'idle',
    error: null,
    result: false
}

// Create the slice and pass in the initial state
const userSlice = createSlice({
    name: 'user',
    initialState,
    reducers: {
        userLoggedIn: (state, action: PayloadAction<User>) => {
            state.user = action.payload;
        },
        userLoggedOut: (state) => {
            state.user.userName = null;
            state.user.email = null;
            state.user.firstName = null;
            state.user.lastName = null;
            state.user.id = null;
            state.user.phoneNumber = null;
            state.user.profileAvatarUrl = null;
            state.error = null;
            state.status ='idle';
        }
    },
    extraReducers: (builder) => {
        builder.addCase(getCurrentUser.pending, (state, action) => {
            state.status = 'pending'
        }).addCase(getCurrentUser.fulfilled, (state, action) => {
            state.status = 'succeeded';
            if (action.payload.success) {
                const currentUser : User = action.payload.data;
                state.user.userName = currentUser.userName;
                state.user.email = currentUser.email;
                state.user.firstName = currentUser.firstName;
                state.user.lastName = currentUser.lastName;
                state.user.id = currentUser.id;
                state.user.phoneNumber = currentUser.phoneNumber;
                state.user.profileAvatarUrl = currentUser.profileAvatarUrl;
                state.result = true
            } else {
                state.result = false;
                state.error = action.payload.message;
            }
        }).addCase(getCurrentUser.rejected, (state, action) => {
            state.status = 'failed';
            state.error = action.error.message ?? 'Unknown Error'
        })
    }
})

// thunk
export const getCurrentUser = createAppAsyncThunk("user/getUser", async(requestBody: {accountId: string}) => {
    const token = await AsyncStorage.getItem('token')
    var response = await authApis(token).post<ApiResponse<User>>(endpoints.getCurrentUser, requestBody);
    return response.data;
} )

// Export the auto-generated action creator with the same name
export const { userLoggedIn, userLoggedOut } = userSlice.actions;

// Useful selector, selector an accept other arugment as well
export const selectUser = (state: RootState) => state.userState.user;
export const selectCurrentUserName = (state: RootState) => state.userState.user.userName;
export const selectUserStatus = (state:RootState) => state.userState.status;
export const selectUserError = (state:RootState) => state.userState.error;

// Export the generated reducer function
export default userSlice.reducer