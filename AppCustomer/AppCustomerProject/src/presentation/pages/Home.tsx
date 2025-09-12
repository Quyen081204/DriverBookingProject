import { View, Text } from "react-native"
import { SafeAreaView } from "react-native-safe-area-context";
import { useEffect } from "react";
import { useAppSelector } from "../redux/hooks";
import { selectUser } from "../redux/features/user/userSlice";
import AsyncStorage from "@react-native-async-storage/async-storage";

export const HomeScreen = () => {

    return (
        <SafeAreaView>
            <View>
                <Text>Home screen</Text>
            </View>
        </SafeAreaView>
    );
}