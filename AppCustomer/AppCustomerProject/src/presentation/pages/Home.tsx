import React, { useCallback } from "react";
import { View, Text, StyleSheet, ScrollView, ImageBackground, Dimensions, TouchableOpacity } from "react-native";
import Modal from 'react-native-modal'
import { HomeHeader } from "../components/home/header";
import { CssConfig } from "../../domain/models/config";
import { HomeBody } from "../components/home/body";
import { useState, useEffect } from 'react';
import * as Location from 'expo-location';
import { useAppDispatch, useAppSelector } from "../redux/hooks";
import { departLocationGetted, selectDepartAddress } from "../redux/features/booking/bookingRequirementSlice";
import { Location as LocationType } from "../redux/features/booking/bookingRequirementSlice";
import MapAPI from '../../data/api/goong/mapApi'
import { mapDataToTextAddress } from "../../data/api/mapData";
import { useFocusEffect, useNavigation } from "@react-navigation/native";

export function HomeScreen() {

  const userCurrentDepartAddress = useAppSelector(selectDepartAddress); 
  const [isModalVisible, setModalVisible] = useState(userCurrentDepartAddress == null);
  const screenHeight = Dimensions.get("window").height;
  const navigation:any = useNavigation();
  const dispatch = useAppDispatch();

  async function getCurrentLocation() {
    try {
      let { status } = await Location.requestForegroundPermissionsAsync();
          if (status !== 'granted') {
              //setErrorMsg('Permission to access location was denied');
              return;
          }

          let location = await Location.getCurrentPositionAsync({
              accuracy: Location.Accuracy.BestForNavigation, // or Location.Accuracy.BestForNavigation
          });
          ///setLocation(location);
          // cập nhật điểm đón cho user dựa trên vị trsi hiện tại
          const currentLocation:LocationType = {
              locationPoint: {
                lat: location.coords.latitude,
                long: location.coords.longitude
              },
              // Reverse geocoding dựa trên thông tin vị trí người dùng hiện tại và kèm theo địa giới hành chính cũ
              locationAddress: await MapAPI.getReverseGeocoding({
                description: encodeURIComponent(`${location.coords.latitude},${location.coords.longitude}`),
                queryParam: {
                  limit: 3,
                  has_deprecated_administrative_unit: true
                }
              }).then(mapDataToTextAddress)
          }
          // cập nhật vị trí hiện tại của nguwoif dùng
          dispatch(departLocationGetted(currentLocation));     
    }
    catch(err) {
      console.log(err);
    }
          
  }

  useEffect(() => {
    if (userCurrentDepartAddress != null) {
      setModalVisible(false);
    }
  },[userCurrentDepartAddress])

  useFocusEffect(
  useCallback(() => {
    setModalVisible(userCurrentDepartAddress == null);
    return () => {
      setModalVisible(false);
    }
  }, [userCurrentDepartAddress])
);

   

  return (
    <View style={{ flex: 1 }}>
      <ImageBackground
        source={require('../../../assets/backgroundImg.webp')}
        style={styles.headerImage}
      >
      </ImageBackground>

      <ScrollView contentContainerStyle={{ paddingTop: 130 }}>
        <View>
            <HomeHeader></HomeHeader>
            <HomeBody></HomeBody>
        </View>
      </ScrollView>

      
      <Modal
        isVisible={isModalVisible}
        style={{ margin: 0, justifyContent: "flex-end" }}
        backdropOpacity={0.5}
      >
        <View style={[styles.modalContent, { height: screenHeight / 1.5 }]}>
        <View style={{flex: 1}}>
          <Text style={styles.modalText}>Chào bạn!</Text>
          <Text style={styles.modalText}>Cho phép ứng dụng truy cập vị trí hiện tại để có trải nghiệm tốt hơn.</Text>
        </View>
          <View style={{flex: 1}}>
            <Text style={{fontSize: 14, color: CssConfig.textSuitYellow, opacity: 0.8}}>Để bỏ qua bước này hãy cho phép ứng dụng truy cập vào vị trí của bạn.</Text>
            <TouchableOpacity style={styles.btnSignIn} onPress={() => getCurrentLocation()}>                           
              <Text style={{ color: 'white', fontSize: 16, fontWeight: "600" }}>Cho phép truy cập vị trí</Text>                              
            </TouchableOpacity>
            <TouchableOpacity style={styles.btnGetLocationManually} onPress={() => {navigation.navigate('getCurrentLocation')}}>                           
              <Text style={{ color: CssConfig.mainColor, fontSize: 16, fontWeight: "600" }}>Nhập vị trí hiện tại</Text>                              
            </TouchableOpacity>
          </View>
        </View>
      </Modal>
    </View>
  );
}

const styles = StyleSheet.create({
  headerImage: {
    position: "absolute",
    top: 0,
    left: 0,
    right: 0,
    height: 120,
    justifyContent: "center",
    alignItems: "center",
  },
  modal: {
    justifyContent: "flex-end",
    backgroundColor: "blue",
    margin: 0,
  },
   modalContent: {
    backgroundColor: "white",
    borderTopLeftRadius: 20,
    borderTopRightRadius: 20,
    padding: 20,
  },
  modalText: {
    fontSize: 20,
    fontWeight: "bold",
    marginBottom: 8,
    color: CssConfig.textSuitYellow
  },
  btnSignIn: {
    backgroundColor: CssConfig.mainColor,
    height: 50,
    justifyContent: "center",
    alignItems: "center",
    borderRadius: 10,
    marginTop: 25,
    },
  btnGetLocationManually: {
    backgroundColor: CssConfig.backgroundColorSlight,
    height: 50,
    justifyContent: "center",
    alignItems: "center",
    borderRadius: 10,
    marginTop: 25,
    borderColor: CssConfig.mainColor,
    borderWidth: 1
  }
});


                 
    // let text = "waiting....";                    
    // if (errorMsg) {                    
    //     text = errorMsg;
    // } else if (location) {
    //     text = JSON.stringify(location);
    // }