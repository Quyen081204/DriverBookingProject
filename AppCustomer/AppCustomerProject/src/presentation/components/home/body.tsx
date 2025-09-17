import { View, Text, ScrollView, StyleSheet, Pressable, Image, FlatList } from "react-native"
import { SafeAreaView } from "react-native-safe-area-context";
import { useEffect } from "react";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { CssConfig } from "../../../domain/models/config";
import { ImageSourcePropType } from "react-native";
import { useNavigation } from "@react-navigation/native";
import { useAppSelector } from "../../redux/hooks";
import { selectRequestVehicleCapacity } from "../../redux/features/booking/bookingRequirementSlice";

type VehicleType = {
    image: ImageSourcePropType,
    name: string,
    isCar: boolean | null
}

type Props = {
    item: VehicleType,
    index: number,
    requestVehicleCapacity: number
}

let vehicleData: VehicleType[] = [{
    image: require("../../../../assets/taxi.png"),
    name: "Ô tô",
    isCar: true
},
{
    image: require("../../../../assets/scooterBike.png"),
    name: "Xe máy",
    isCar: false
},
{
    image: require("../../../../assets/package.png"),
    name: "Giao hàng",
    isCar: null
}
]

const VehicleItem = ({ item, index, requestVehicleCapacity }: Props) => {
    const navigation: any = useNavigation();

    const handleOnpress = () => {
        // Nếu xe đang chọn là xe máy và item này là xe máy
        if (item.isCar != null) {
            if ((requestVehicleCapacity == 2 && !item.isCar) || (requestVehicleCapacity > 4 && item.isCar)) {
                navigation.navigate('searchLocation')
            } else if (requestVehicleCapacity == null) {
                navigation.navigate('searchLocation')
            }
        } else {
            console.log("Giao hang");
        }
    }
    return (
        <View style={vehicleStyle.vehicleContainer}>
            <Pressable style={vehicleStyle.btn} onPress={() => handleOnpress()}>
                <View style={{ flex: 2, justifyContent: "center" }}>
                    <Image source={item.image} style={vehicleStyle.img} />
                </View>
                <View style={{ flex: 1, justifyContent: "center", alignItems: "center" }}>
                    <Text style={vehicleStyle.vehicleName}>{item.name}</Text>
                </View>
            </Pressable>
        </View>
    )
}

export const HomeBody = () => {
    const requestVehicleCapacity = useAppSelector(selectRequestVehicleCapacity);
    return (
        <View style={{ marginTop: 10, paddingHorizontal: CssConfig.paddingHorizontal }}>
            <FlatList data={vehicleData}
                renderItem={({ item, index }) => (<VehicleItem key={index} item={item} index={index} requestVehicleCapacity={requestVehicleCapacity}/>)}
                horizontal
                showsHorizontalScrollIndicator={false}
            />
        </View>
    );
}


const styles = StyleSheet.create({
    container: {
        marginTop: 20
    }
})


const vehicleStyle = StyleSheet.create({
    vehicleContainer: {
        backgroundColor: "rgba(255, 250, 205, 0.3)",
        marginRight: 20,
        borderRadius: 10
    },
    img: {
        width: 70,
        height: 70
    },
    vehicleName: {
        fontWeight: "500"
    },
    btn: {
        padding: 20,
        justifyContent: "center",
        alignItems: "center",
        gap: 8
    }
});
