import { useNavigation } from "@react-navigation/native"
import { useState } from "react";
import {
    View, Text, Button, StyleSheet, TextInput, Touchable, TouchableOpacity, Keyboard,
    KeyboardAvoidingView,
    TouchableWithoutFeedback,
    ActivityIndicator,
} from "react-native"
import { SafeAreaView } from "react-native-safe-area-context";
import Icon from "react-native-vector-icons/Feather";
import { Color } from "../../../domain/models/config";
import Apis, { authApis, endpoints } from "../../../data/api/clients";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { useAppDispatch, useAppSelector } from "../../redux/hooks";
import { getCurrentUser } from "../../redux/features/user/userSlice";

interface AuthenticatedResult {
    accountId: string,
    token: string,
    refreshToken: string
}

export const LoginScreen = () => {
    const navigation: any = useNavigation();
    const [secure, setSecure] = useState(true);
    const [userName, setUserName] = useState('');
    const [password, setPassword] = useState('');
    const [isValid, setIsValid] = useState(true);
    const [errorMessage, setErrorMsg] = useState('');
    const [loading, setLoading] = useState(false);

    const dispatch = useAppDispatch();

    const validate = (): boolean => {
        if (!userName || !password) {
            setIsValid(false);
            setErrorMsg("Username và password không được để trống");
            return false;
        }

        if (userName.includes(" ")) {
            setIsValid(false);
            setErrorMsg("Username không chứa khoảng trắng");
            return false;
        }


        if (password.includes(" ")) {
            setIsValid(false);
            setErrorMsg("Password không chứa khoảng trắng");
            return false;
        }

        setIsValid(true);
        setErrorMsg('');
        return true;
    }

    const login = async () => {
        // validate the username and password
        if (validate()) {
            try {
                console.log("logininnnnn")
                setLoading(true);
                let res = await Apis.post<AuthenticatedResult>(endpoints.login, { username: userName, password: password });
                console.log(res);
                if (res.status === 200) {
                    // Store token in async storage
                    const token = await AsyncStorage.setItem('token', res.data.token);
                    const refreshToken = await AsyncStorage.setItem('refreshToken', res.data.refreshToken);
                    const accountId: string = res.data.accountId;
                    // Dispatch action to update user slice state
                    const data = dispatch(getCurrentUser({ accountId }));
                    console.log("data current user: ", data);
                    
                    navigation.replace("inapp");
                }
            }
            catch (err) {
                if (err.response) {
                    // Server responded with a status code outside 2xx
                    console.log("Error status:", err.response.status);
                    console.log("Error data:", err.response.data);
                    setErrorMsg("*Đăng nhập thất bại vui lòng thử lại!");
                    setIsValid(false);
                } else {
                    // Network error / request not sent
                    console.log("Network Error:", err.message);
                }
            }
            finally {
                setLoading(false);
            }
        }
    }

    return (
        <KeyboardAvoidingView style={{ flex: 1 }}>
            <TouchableWithoutFeedback onPress={Keyboard.dismiss}>

                <SafeAreaView style={styles.container}>
                    <View style={styles.main}>
                        <View style={styles.header}>
                            <Text style={styles.headerText}>Đăng nhập</Text>
                        </View>
                        <View style={styles.body}>
                            <TextInput onChangeText={(userName) => setUserName(userName)} style={styles.textInput} placeholder="Username">{userName}</TextInput>
                            <View style={styles.passwordContainer}>
                                <TextInput style={{ flex: 1, paddingVertical: 15 }} onChangeText={(text) => setPassword(text)} secureTextEntry={secure} placeholder="Password">{password}</TextInput>
                                <TouchableOpacity onPress={() => setSecure(!secure)}>
                                    <Icon name={secure ? "eye-off" : "eye"} size={15} color="#333" />
                                </TouchableOpacity>
                            </View>
                            {
                                !isValid && <Text style={{ color: "red" }}>{errorMessage}</Text>
                            }
                            <TouchableOpacity style={styles.btnSignIn} onPress={() => login()}>
                                {
                                    loading ? (
                                        <ActivityIndicator color="#007AFF"></ActivityIndicator>
                                    ) : (
                                        <Text style={{ color: 'white', fontSize: 14 }}>Đăng nhập</Text>     
                                    )
                                }
                            </TouchableOpacity>
                            <View style={styles.registerContainer}>
                                <Text>Bạn chưa có tài khoản?</Text>
                                <TouchableOpacity onPress={() => { navigation.navigate('register') }}>
                                    <Text style={{ marginLeft: 8, color: Color.mainColor }}>Đăng ký ngay</Text>
                                </TouchableOpacity>
                            </View>
                        </View>
                    </View>
                </SafeAreaView>
            </TouchableWithoutFeedback>
        </KeyboardAvoidingView>)
}


const styles = StyleSheet.create({
    container: {
        flex: 1
    },
    main: {
        marginHorizontal: 25,
        flex: 1,
    },
    header: {
        flex: 1,
        justifyContent: "center",
        alignItems: "center",
        paddingTop: 20
    },
    headerText: {
        fontSize: 25,
        fontWeight: "bold"
    },
    body: {
        flex: 3,
    },
    textInput: {
        padding: 15,
        borderWidth: 1,
        borderColor: "#b3b1b1ff",
        borderRadius: 10,
        paddingHorizontal: 12,
        marginBottom: 16,
    },
    passwordContainer: {
        flexDirection: "row",
        justifyContent: "space-between",
        alignItems: "center",
        borderWidth: 1,
        borderColor: "#b3b1b1ff",
        borderRadius: 10,
        paddingHorizontal: 12,
        marginBottom: 16,
    },
    btnSignIn: {
        backgroundColor: Color.mainColor,
        height: 50,
        justifyContent: "center",
        alignItems: "center",
        borderRadius: 10,
        marginTop: 25,
    },
    registerContainer: {
        flexDirection: "row",
        justifyContent: "center",
        alignItems: "center",
        marginTop: 25
    }
});

// Tomorrow: register page