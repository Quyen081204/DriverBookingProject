/**
 * This screen used for getting current location of user if they dont allow to turn on the GPS
 * 
 */

import {
    View, Text, Dimensions, TouchableWithoutFeedback, KeyboardAvoidingView,
    Keyboard, TouchableOpacity, StyleSheet,
    Platform,
    FlatList, ScrollView,
    Pressable
} from 'react-native'
import React, { useState, useEffect, useRef } from 'react'
import HeaderScreen from '../../components/common/headerScreen'
import { useNavigation } from '@react-navigation/native'
import { Feather } from "@expo/vector-icons";
import { TextInput } from 'react-native-paper';
import { SafeAreaView } from 'react-native-safe-area-context';
import { CssConfig } from '../../../domain/models/config';
import MapApi from '../../../data/api/goong/mapApi';
import { useAppDispatch, useAppSelector } from '../../redux/hooks';
import { departLocationGetted, Location } from '../../redux/features/booking/bookingRequirementSlice';
import { mapError } from '../../../data/api/mapData';
import { selectUserLocationPicking } from '../../redux/features/pickingLocation/userLocationPickingSlice';

interface AutocompleteResponse {
    mainText: string,
    mainTextMatch: MatchSubstrings[] | [],
    secondaryText: string | null,
    sendaryTextMatch: MatchSubstrings[] | [],
    distance?: number | null,
    place_id: string
}

interface MatchSubstrings {
    length: number,
    offset: number
}

interface HightlightText {
    text: string,
    isHighLight: boolean
}

const predictAutocompleteData: AutocompleteResponse[] = [
    {
        mainText: "87/5 Tây Sơn",
        secondaryText: "Phường Tân Qúy, Quận Tân Phú, TP HCM",
        mainTextMatch: [
            {
                "length": 3,
                "offset": 5
            },
        ],
        sendaryTextMatch: [],
        place_id: "abc"
    },
    {
        mainText: "Tân Quý, Tân Phú, Hồ Chí Minh",
        secondaryText: "Phường Tân Qúy, Quận Tân Phú, TP HCM",
        mainTextMatch: [
            {
                "length": 3,
                "offset": 4
            },
            {
                "length": 3,
                "offset": 0
            },
            {
                "length": 3,
                "offset": 9
            },
            {
                "length": 3,
                "offset": 13
            }
        ],
        sendaryTextMatch: [],
        place_id: "def"
    },
]

type HighlightMatchingParams = {
    originalText: string,
    matchSubstrings: MatchSubstrings[],
}

const hightlightMatchingTexts = ({ originalText, matchSubstrings }: HighlightMatchingParams) => {
    const arrayOfText: HightlightText[] = [];
    let lastNormalTextIndex = 0;
    let i = 0;
    if (matchSubstrings.length == 0) {
        // no hight light
        arrayOfText.push({
            text: originalText,
            isHighLight: false
        });
    }
    else {
        // sort asec matching array base on the offset
        const matchSubstringsSorted = matchSubstrings.sort((a, b) => {
            return a.offset - b.offset;
        });

        matchSubstringsSorted.forEach((item, index) => {
            if (lastNormalTextIndex < item.offset) {

                // No highlight [lastNormalTextIndex, offset)
                arrayOfText.push({
                    text: originalText.substring(lastNormalTextIndex, item.offset),
                    isHighLight: false
                });
            }
            // hightlight [item.offset, offset + length]
            arrayOfText.push({
                text: originalText.substring(item.offset, item.offset + item.length),
                isHighLight: true
            });
            lastNormalTextIndex = item.offset + item.length;
        })

        // add the last elem
        arrayOfText.push({
            text: originalText.substring(lastNormalTextIndex),
            isHighLight: false
        })
    }

    return arrayOfText;
}

const mapDataToAutocompleteSimp = (res: any): AutocompleteResponse[] => {
    const predictions = res.predictions;
    const mapAutocompleSimp: AutocompleteResponse[] = predictions.map(item => ({
        place_id: item.place_id,
        mainText: item.structured_formatting.main_text,
        mainTextMatch: item.structured_formatting.main_text_matched_substrings,
        secondaryText: item.structured_formatting.secondary_text,
        sendaryTextMatch: item.structured_formatting.secondary_text_matched_substrings
    }))

    return mapAutocompleSimp;
}

const AutocompleteItem = ({ item }: { item: AutocompleteResponse }) => {

    const arrayOfMainText = hightlightMatchingTexts({ originalText: item.mainText, matchSubstrings: item.mainTextMatch });
    const arrayOfSecondText = hightlightMatchingTexts({ originalText: item.secondaryText, matchSubstrings: item.sendaryTextMatch });
    const navigation: any = useNavigation();
    const dispatch = useAppDispatch();

    const getCurrentLocation = async (place_id: string) => {
        const currentLocationDetail = await MapApi.getPlaceDetail({ place_id }).then((res) => {
            if (res.status === "OK") {
                return {
                    locationPoint: {
                        lat: res.result.geometry.location.lat,
                        long: res.result.geometry.location.lng
                    },
                    locationAddress: res.result.formatted_address
                }
            }
        }).catch(mapError);
        // cap nhat vi tri hien tai
        dispatch(departLocationGetted(currentLocationDetail));
        // quay ve bo
        navigation.replace('home');
    }

    const handleLocationSelected = () => {
        getCurrentLocation(item.place_id);
    }

    return (
        <View>
            <TouchableOpacity style={autocompleteItemStyle.container} onPress={() => handleLocationSelected()}>
                <View style={autocompleteItemStyle.iconContainer}>
                    <Feather name="map-pin" size={20} color="#a09e9eff" />
                </View>
                <View style={{ gap: 8 }}>
                    <Text>
                        {
                            arrayOfMainText.map((item, index) => (
                                <Text key={index} style={item.isHighLight ? autocompleteItemStyle.hightlightText : autocompleteItemStyle.normalText}>{item.text}</Text>
                            ))
                        }
                    </Text>
                    <Text>
                        {
                            arrayOfSecondText.map((item, index) => (
                                <Text key={index} style={item.isHighLight ? autocompleteItemStyle.hightlightText : autocompleteItemStyle.normalText}>{item.text}</Text>
                            ))
                        }
                    </Text>
                </View>
            </TouchableOpacity>
        </View>
    )
}

const autocompleteItemStyle = StyleSheet.create({
    container: {
        flexDirection: "row",
        alignItems: "center",
        paddingVertical: 15,
        gap: 20
    },
    iconContainer: {
        width: 36,
        height: 36,
        borderRadius: "50%",
        backgroundColor: "rgba(165, 163, 163, 0.3)",
        alignItems: "center",
        justifyContent: "center",
    },
    normalText: {
        fontSize: 16,
        color: CssConfig.textSuitYellow
    },
    hightlightText: {
        fontSize: 16,
        color: CssConfig.mainColor
    }
})

const GetCurrentLocationScreen = () => {

    const navigation: any = useNavigation();
    const [userLocationInput, setUserLocationInput] = useState<string>("");
    // dữ liệu từ màn hình picking location
    const userLocationPicking = useAppSelector(selectUserLocationPicking);

    const [autocompleteData, setAutocompleteData] = useState<AutocompleteResponse[]>([]);
    const timerId = useRef<ReturnType<typeof setTimeout> | null>(null);

    const goBack = () => {
        navigation.goBack();
    }


    // getting data from auto complete -> extract it to simple form 
    const getAutocompleteData = async (textInput: string) => {
        let data = await MapApi.getAutocomplete({
            search: encodeURIComponent(textInput),
            queryParam: {
                location: "21.0278,105.8342",
                radius: 50
            }
        }).then(mapDataToAutocompleteSimp);

        // update state
        setAutocompleteData(data);
    };

    useEffect(() => {
        // Nếu có dữ liệu từ việc người dùng chọn trên bản đồ thì update giá trị của text input
        if (userLocationPicking != "") {
            setUserLocationInput(userLocationPicking);
        } 
    },[userLocationPicking]);

    useEffect(() => {
        if (userLocationInput != "") {
            timerId.current = setTimeout(() => {
                getAutocompleteData(userLocationInput);
            }, 100);
        }

        return () => {
            if (timerId.current) {
                clearTimeout(timerId.current);
            }
        }
    }, [userLocationInput])

    return (
        <KeyboardAvoidingView style={{ flex: 1 }}
            behavior={Platform.OS === "ios" ? "padding" : "height"}
            keyboardVerticalOffset={0}>

            <TouchableWithoutFeedback onPress={Keyboard.dismiss}>
                <SafeAreaView style={{ flex: 1 }}>
                    <HeaderScreen screenTitle='Vị trí của bạn' goBackFunc={goBack} />
                    <View style={{ justifyContent: "space-between", flex: 1 }}>
                        <View style={{ paddingTop: 50, paddingHorizontal: CssConfig.paddingHorizontal }}>
                            <TextInput
                                value={userLocationInput}
                                onChangeText={(text) => { setUserLocationInput(text) }}
                                style={{ backgroundColor: "#f2f2f2" }}
                                placeholder="Nhập địa chỉ hiện tại"
                                activeUnderlineColor={CssConfig.mainColor}
                                autoFocus={true}   
                                left={
                                    <TextInput.Icon
                                        icon={() => <Feather name="map-pin" size={24} color="#28bdbf" />}
                                    />
                                }
                            />
                            <FlatList data={autocompleteData}
                                keyExtractor={(item) => item.place_id}
                                renderItem={({ item, index }) => <AutocompleteItem key={index} item={item} />}
                                showsVerticalScrollIndicator={false}
                            ></FlatList>
                        </View>
                        <View>
                            <TouchableOpacity style={styles.btnPickFromMap} onPress={() => { navigation.navigate("pickLocation") }}>
                                <Feather name="map" size={24} color="black" />
                                <Text style={styles.textPickFromMap}>
                                    Chọn từ bản đồ
                                </Text>
                            </TouchableOpacity>
                        </View>
                    </View>
                </SafeAreaView>
            </TouchableWithoutFeedback>
        </KeyboardAvoidingView>

    )
}

const styles = StyleSheet.create({
    btnPickFromMap: {
        flexDirection: "row",
        justifyContent: "center",
        borderTopColor: "#d3d3d3",
        borderTopWidth: 1,
        borderBottomColor: "#d3d3d3",
        borderBottomWidth: 1,
        gap: 10,
        padding: 20
    },
    textPickFromMap: {
        fontSize: CssConfig.labelFontSize,
        fontWeight: "400"
    }
});

export default GetCurrentLocationScreen


// Hanlde auto comple khi nhap dia chi va dispactch action