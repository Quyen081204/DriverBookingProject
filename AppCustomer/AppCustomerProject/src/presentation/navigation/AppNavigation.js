import { NavigationContainer } from "@react-navigation/native";
import { createNativeStackNavigator } from "@react-navigation/native-stack";
import { createBottomTabNavigator } from "@react-navigation/bottom-tabs";
import { LoginScreen } from "../pages/auth/Login";
import { RegisterScreen } from "../pages/auth/Register";
import { HomeScreen } from "../pages/Home";
import { AccountScreen } from "../pages/Account";
import { HistoryScreen } from "../pages/History";
import { InitBookingScreen } from "../pages/booking/InitBooking";
import { ProcessBookingScreen } from "../pages/booking/ProcessBooking";
import { TrackingScreen } from "../pages/booking/Tracking";
import Feather from "react-native-vector-icons/Feather";


const Stack = createNativeStackNavigator();
const Tab = createBottomTabNavigator();

export const AppNavigation = () => {
    return (
        <NavigationContainer>
            <Stack.Navigator screenOptions={{ headerShown: false }}>
                <Stack.Screen name="auth" component={AuthNavigation}></Stack.Screen>
                <Stack.Screen name="inapp" component={InAppNavigation}></Stack.Screen>
            </Stack.Navigator>
        </NavigationContainer>
    );
}


const AuthNavigation = () => {
    return (
        <Stack.Navigator screenOptions={{ headerShown: false }}>
            <Stack.Screen name="login" component={LoginScreen}></Stack.Screen>
            <Stack.Screen name="register" component={RegisterScreen}></Stack.Screen>
        </Stack.Navigator>
    )
}

const InAppNavigation = () => {
    return (
        <Tab.Navigator screenOptions={({route}) => ({
            tabBarIcon : ({color, size}) => {
                let iconName = "";
                if (route.name == "homeBooking") {
                    iconName = "home";
                } else if (route.name == "history") {
                    iconName = "clock";
                } else if (route.name == "account") {
                    iconName = "user";
                }

                return <Feather name={iconName} color={color} size={20}/> 
            },
            tabBarActiveTintColor: "#ffc107",
            tabBarInactiveTintColor: "gray",
            headerShown: false
        })}>
            <Tab.Screen name="homeBooking" component={HomeNavigation} options={{title:"Trang chủ"}}></Tab.Screen>
            <Tab.Screen name="history" component={HistoryScreen} options={{title:"Hoạt động"}}></Tab.Screen>
            <Tab.Screen name="account" component={AccountScreen} options={{title:"Tài khoản"}}></Tab.Screen>
        </Tab.Navigator >
    )
}

const HomeNavigation = () => {
    return (
        <Stack.Navigator screenOptions={{ headerShown: false }}>
            <Stack.Screen name="home" component={HomeScreen}></Stack.Screen>
            <Stack.Screen name="initBooking" component={InitBookingScreen}></Stack.Screen>
            <Stack.Screen name="processBooking" component={ProcessBookingScreen}></Stack.Screen>
            <Stack.Screen name="tracking" component={TrackingScreen}></Stack.Screen>
        </Stack.Navigator>
    )
}