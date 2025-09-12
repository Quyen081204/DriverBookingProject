import { useNavigation } from "@react-navigation/native"
import { useState } from "react";
import {
    View, Text, Button, StyleSheet, Touchable, TouchableOpacity, Keyboard,
    TouchableWithoutFeedback,
    Image,
    ActivityIndicator
} from "react-native"
import { TextInput } from "react-native-paper";
import { SafeAreaView } from "react-native-safe-area-context";
import { CssConfig } from "../../../domain/models/config";
import Apis, { authApis, endpoints } from "../../../data/api/clients";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { getCurrentUser, User } from "../../redux/features/user/userSlice";
import { KeyboardAwareScrollView } from "react-native-keyboard-aware-scroll-view";
import { AuthenticatedResult } from "../../../domain/models/authenticatedResult";
import { ApiResponse } from "../../../domain/models/ApiResponse";
import { AsyncThunkAction } from "@reduxjs/toolkit";
import { RootState, AppDispatch } from "../../redux/store";
import { useAppDispatch } from "../../redux/hooks";

interface infoType {
    placeHolder: string,
    field: string,
    securityTextEntry: boolean,
    autoCapitalize: "none" | "sentences" | "words" | "characters",
    rIcon: string | null,
    underlineColor: string
}

export const RegisterScreen = () => {
    const info: infoType[] = [{
        placeHolder: "Họ và tên",
        field: "FullName",
        securityTextEntry: false,
        autoCapitalize: "none",
        rIcon: null,
        underlineColor: "transparent"
    },
    {
        placeHolder: "Số điện thoại",
        field: "PhoneNumber",
        securityTextEntry: false,
        autoCapitalize: "none",
        rIcon: null,
        underlineColor: "transparent"
    },
    {
        placeHolder: "Email",
        field: "Email",
        securityTextEntry: false,
        autoCapitalize: "none",
        rIcon: null,
        underlineColor: "transparent"
    },
    {
        placeHolder: "Username",
        field: "UserName",
        securityTextEntry: false,
        autoCapitalize: "none",
        rIcon: null,
        underlineColor: "transparent"
    },
    {
        placeHolder: "Mật khẩu",
        field: "PassWord",
        securityTextEntry: true,
        autoCapitalize: "none",
        rIcon: "eye",
        underlineColor: "transparent"
    },]

    const [user, setUser] = useState({});
    const [showPassword, setShowPassword] = useState(true);
    const dispatch = useAppDispatch();
    const navigation: any = useNavigation();
    const [loading, setLoading] = useState(false);
    const [isValid, setIsValid] = useState(true);
    const [errorMessage, setErrorMsg] = useState('');

    const setStateUser = (field, value) => {
        setUser(prev => ({
            ...prev, [field]: value
        }))
    }

    const validate = (): boolean => {
        for (let i of info) {
            if (!(i.field in user) || user[i.field] == "") {
                setIsValid(false);
                setErrorMsg(`*Vui lòng nhập ${i.placeHolder}`);
                return false;
            }
        }
        setIsValid(true);
        return true;
    }

    const register = async () => {
        if (validate()) {
            try {
                setLoading(true);
                // send request body user to the server
                let res = await Apis.post<ApiResponse<AuthenticatedResult>>(endpoints.registerUser, user);
                if (res.status === 200) {
                    // Store token in async storage
                    const token = await AsyncStorage.setItem('token', res.data.data.token);
                    const refreshToken = await AsyncStorage.setItem('refreshToken', res.data.data.refreshToken);
                    const accountId: string = res.data.data.accountId;
                    // Dispatch action to update user slice state
                    const data = dispatch(getCurrentUser({ accountId }));

                    navigation.replace("inapp");
                }
            } catch (err) {
                if (err.response) {
                    // Server responded with a status code outside 2xx
                    console.log("Error status:", err.response.status);
                    console.log("Error data:", err.response.data.message);
                    setIsValid(false);
                    setErrorMsg(err.response.data.message);
                } else {
                    // Network error / request not sent
                    console.log("Network Error:", err.message);
                    setIsValid(false);
                    setErrorMsg(err.response.data.message);
                }
            }
            finally {
                setLoading(false);
            }
        } 
    }

    return (
        <KeyboardAwareScrollView
            contentContainerStyle={{ flexGrow: 1 }}
            extraScrollHeight={20}
        >
            <TouchableWithoutFeedback onPress={Keyboard.dismiss}>
                <SafeAreaView style={styles.container}>
                    <View style={styles.header}>
                        <Image source={require('../../../../assets/logoBe.jpeg')} style={styles.img} />
                        <Text style={styles.headerText}>Đăng ký tài khoản</Text>
                    </View>
                    <View style={styles.body}>
                        {
                            info.map(i => (
                                <TextInput placeholder={i.placeHolder}
                                    key={i.field}
                                    secureTextEntry={i.securityTextEntry && showPassword}
                                    autoCapitalize={i.autoCapitalize}
                                    underlineColor={i.underlineColor}
                                    value={user[i.field]}
                                    onChangeText={(text) => setStateUser(i.field, text)}
                                    right={i.rIcon && <TextInput.Icon onPress={() => setShowPassword(!showPassword)} icon={showPassword ? "eye-off" : "eye"} size={20} />}
                                    style={styles.info} />))
                        }
                        {
                            !isValid && <Text style={{ color: "red" }}>{errorMessage}</Text>
                        }
                        <TouchableOpacity style={styles.btnSignIn} onPress={() => register()}>
                            {
                                loading ? (
                                    <ActivityIndicator color="#007AFF"></ActivityIndicator>
                                ) : (
                                    <Text style={{ color: 'white', fontSize: 14 }}>Đăng ký</Text>
                                )
                            }
                        </TouchableOpacity>
                    </View>
                </SafeAreaView>
            </TouchableWithoutFeedback>
        </KeyboardAwareScrollView>
    )
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        paddingHorizontal: 25
    },
    header: {
        flex: 1,
        borderColor: "black",
        borderWidth: 1,
        justifyContent: "center",
        alignItems: "center",
        gap: 14
    },
    body: {
        flex: 3,
        borderColor: "black",
        borderWidth: 1,
        marginTop: 12
    },
    img: {
        width: 120,
        height: 120,
        borderRadius: 20
    },
    headerText: {
        fontSize: 20,
        fontWeight: "600",
        color: CssConfig.textSuitYellow
    },
    info: {
        backgroundColor: "#d3d3d3",
        borderRadius: 10,
        paddingHorizontal: 12,
        marginBottom: 20,
    },
    btnSignIn: {
        backgroundColor: CssConfig.mainColor,
        height: 50,
        justifyContent: "center",
        alignItems: "center",
        borderRadius: 10,
        marginTop: 15,
    },
})