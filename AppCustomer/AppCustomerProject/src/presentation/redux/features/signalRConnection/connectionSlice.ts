// import { HubConnection } from "@microsoft/signalr";
// import { createSlice } from "@reduxjs/toolkit";
// import { userLoggedIn } from "../user/userSlice";
// import AsyncStorage from "@react-native-async-storage/async-storage";
// import { createAppAsyncThunk } from "../../hooks";
// import type { RootState } from '../../store'

// const initialState: HubConnection = null;

// const connectionSlice = createSlice({
//     name:"connection",
//     initialState,
//     reducers: {},
//     extraReducers: (builder) => {
//         builder.addCase(initSignalRConnection.fulfilled, (state, action) => {
//             // return the connection object has been create in the initSignalRConnection thunk
//             return action.payload;
//         })
//     }
// })

// export const initSignalRConnection = createAppAsyncThunk("connection/init", async() => {
//     try {
//         const token = await AsyncStorage.getItem('token');
//         const connection = createConnetion(token);
//         await connection.start();
//         console.log("SignalR Connected.");
//         await connection.send("TestConnectedBySendMessage", "This from Quyen app ", "Server can hear that ?");
//         return connection;
//     }
//     catch(err) {
//         console.log(err);
//     }
// })

// export default connectionSlice.reducer;

// export const selectConnection = (state:RootState) => state.connection;
// // useSelector handle pass store.getState() to our selector function