import { View, Text, StyleSheet, Dimensions, Pressable } from 'react-native'
import React from 'react'
import { CssConfig } from '../../../domain/models/config';
import Icon from "react-native-vector-icons/Ionicons";
import { SafeAreaView } from 'react-native-safe-area-context';

type Props = {
    screenTitle: string,
    goBackFunc: () => void;
}

const HeaderScreen = ({ screenTitle, goBackFunc }: Props) => {
    const screenHeight = Dimensions.get("window").height;
    return (
        <View style={[styles.container, {height: screenHeight * 0.1}]}>
                <Pressable style={styles.btn} onPress={() => goBackFunc()}>
                    <Icon name="arrow-back" size={24} color="black"/>
                 </Pressable>
                <Text style={styles.screenTitle}>{screenTitle}</Text>
        </View>
    )
}

const styles = StyleSheet.create({
    container: {
        position: "absolute",
        top: 0,
        left: 0,
        right: 0,
        paddingHorizontal: CssConfig.paddingHorizontal,
        borderColor: "#d3d3d3",
        borderWidth: 1,
    },
    screenTitle: {
        position: "absolute",
        bottom: 0,
        //paddingHorizontal: CssConfig.paddingHorizontal,
        paddingTop: 6,
        alignSelf: "center",
        fontSize: 18,
        fontWeight: "500",
        paddingVertical: 6
    },
    btn: {
        position: "absolute",
        bottom: 0,
        paddingHorizontal: CssConfig.paddingHorizontal,
        paddingVertical: 6,
    }
});

export default HeaderScreen