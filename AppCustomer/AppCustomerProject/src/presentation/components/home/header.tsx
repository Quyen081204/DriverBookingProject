import { View, Text, StyleSheet, Pressable } from "react-native"
import { CssConfig } from "../../../domain/models/config";
import { Feather } from "@expo/vector-icons";
import { useAppSelector } from "../../redux/hooks";
import { selectUserFullName } from "../../redux/features/user/userSlice";
import { useNavigation } from "@react-navigation/native";

export const HomeHeader = () => {
    const userFullName = useAppSelector(selectUserFullName);
    const navigation:any = useNavigation();

    return (
       <View style={{paddingHorizontal: CssConfig.paddingHorizontal}}>
            <Text style={styles.headerText}>Chào {userFullName}</Text>
            <View style={{marginTop: 20}}>
                <Pressable style={styles.searchBar} onPress={() => navigation.navigate('searchLocation')}>
                    <Feather name="map-pin" size={24} color="#28bdbf" />
                    <Text style={styles.text}>Bạn muốn đi đâu?</Text>
                </Pressable>
            </View>
       </View>
    );
}

const styles = StyleSheet.create({
    headerText: {
        fontSize: 16,
        fontWeight: "700",
        color: CssConfig.textSuitYellow
    },
    searchBar: {
        flexDirection: "row",
        alignItems: "center",
        gap: 10,
        borderRadius: 50,
        borderColor: CssConfig.mainColor,
        borderWidth: 2,
        padding: 12
    },
    text: {
        fontSize: 16,
        fontWeight: "700",
        color: CssConfig.textSuitYellow
    }

});