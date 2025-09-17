import { View, StyleSheet, Text, ScrollView, TouchableOpacity, Touchable, Pressable } from 'react-native';
import React, { useEffect, useRef, useState } from 'react';
import MapView from 'react-native-maps';
import { Marker } from 'react-native-maps';
import { CssConfig } from '../../../domain/models/config';
import MapApi from '../../../data/api/goong/mapApi';
import { departLocationGetted, Location } from '../../redux/features/booking/bookingRequirementSlice';
import { Feather } from "@expo/vector-icons";
import Icon from "react-native-vector-icons/FontAwesome";
import { mapError } from '../../../data/api/mapData';
import { UseDispatch } from 'react-redux';
import { useAppDispatch } from '../../redux/hooks';
import { useNavigation } from '@react-navigation/native';
import { userLocationPickingUpdated } from '../../redux/features/pickingLocation/userLocationPickingSlice';

type LatLng = {
  latitude: number,
  longitude: number,
}

type Region = {
  latitude: number,
  longitude: number,
  latitudeDelta: number,
  longitudeDelta: number,
}

interface Marker {
  latlng: LatLng,
  description: string
}

let data: Location[] = [
  {
    locationPoint: {
      lat: 3939,
      long: 39393
    },
    locationAddress: "abc"
  },
  {
    locationPoint: {
      lat: 3939,
      long: 39393
    },
    locationAddress: "abc"
  }, {
    locationPoint: {
      lat: 3939,
      long: 39393
    },
    locationAddress: "abc"
  },
  {
    locationPoint: {
      lat: 3939,
      long: 39393
    },
    locationAddress: "abc"
  },
]

const LocationItem = ({ item, index, isSelected, handleSelected }: { item: Location, index: number, isSelected: boolean, handleSelected: (index: number) => void }) => {

  const navigation:any = useNavigation();
  const dispatch = useAppDispatch();
  
  // Người dùng muốn thay đổi địa chỉ bằng tay ở trong màn hình getCurrentLocation
  const handleOnEditPressed = () => {
    dispatch(userLocationPickingUpdated(item.locationAddress));
    navigation.replace('getCurrentLocation');
  }
  
  return (
    <View>
      <TouchableOpacity style={[locationStyles.container, { backgroundColor: isSelected ? "rgba(238, 231, 165, 0.3)" : "white" }]} onPress={() => handleSelected(index)}>
        <View style={locationStyles.iconContainer}>
          <Feather name="map-pin" size={20} color="#a09e9eff" />
        </View>
        <View style={{ gap: 8, flex: 1 }}>
          <Text>
            {item.locationAddress}
          </Text>
        </View>
        {
          isSelected ? (<Pressable style={locationStyles.editIconContainer} onPress={() => handleOnEditPressed()}>
            <Icon name="pencil" size={18} color="#666" />
          </Pressable>) : <></>
        }

      </TouchableOpacity>
    </View>
  )
}

const locationStyles = StyleSheet.create({
  container: {
    flexDirection: "row",
    alignItems: "center",
    paddingVertical: 15,
    gap: 20,
    flex: 1
  },
  iconContainer: {
    width: 36,
    height: 36,
    borderRadius: "50%",
    backgroundColor: "rgba(165, 163, 163, 0.3)",
    alignItems: "center",
    justifyContent: "center",
  },
  editIconContainer: {
    width: 36,
    height: 36,
    alignItems: "center",
    justifyContent: "center",
  }
});

const mapDataToLocation = (res: any): Location[] => {
  let data: Location[] = [];
  if (res.status == "OK") {
    data = res.results.map((item) => {
      return {
        locationPoint: {
          lat: item.geometry.location.lat,
          long: item.geometry.location.lng
        },
        locationAddress: item.formatted_address
      }
    })
  }

  return data;
}

const PickLocationScreen = () => {
  const [region, setRegion] = useState<Region>({
    latitude: 10.785412207,
    longitude: 106.664921,
    latitudeDelta: 0.009999945759787465,
    longitudeDelta: 0.009999945759787465,
  });

  const [selectedIndex, setSelectedIndex] = useState(0);
  const [addressesFetched, setAddressesFetched] = useState<Location[]>([]);
  const dispatch = useAppDispatch();
  const navigation:any = useNavigation();
  const timerId = useRef<ReturnType<typeof setTimeout> | null>(null);

  const handleSelected = (index: number) => {
    setSelectedIndex(index);
  }

  const handleConfirm = () => {
      if (addressesFetched.length == 0)
        return;
      
      dispatch(departLocationGetted(addressesFetched[selectedIndex]));
      navigation.replace('home');
  }

  const fetchDataAddress = async (location: { lat: number, long: number }) => {
    const data: Location[] = await MapApi.getReverseGeocoding({ description: encodeURIComponent(`${location.lat},${location.long}`) })
                                         .then(mapDataToLocation).catch(mapError);
    setAddressesFetched(data);
  }
  // Get address tips
  useEffect(() => {
    timerId.current = setTimeout(() => fetchDataAddress({ lat: region.latitude, long: region.longitude }), 1000);

    return () => {
      if (timerId.current) {
        clearTimeout(timerId.current);
      }
    }

  }, [region])

  return (
    <View style={styles.container}>
      <Pressable style={styles.iconContainer} onPress={() => {navigation.goBack();}}>
          <Icon name="arrow-left" size={20} color={CssConfig.textSuitYellow} />
      </Pressable>
      <View style={{ flex: 2 }}>
        <MapView style={styles.map}
          provider="google"
          initialRegion={region}
          onRegionChange={(region) => {
            setRegion({
              ...region,
              latitudeDelta: 0.009999945759787465,
              longitudeDelta: 0.009999945759787465
            })
          }}
        >
          <Marker
            coordinate={region}
          />
        </MapView>
      </View>

      <View style={styles.bottomContainer}>
        <ScrollView>
          {
            addressesFetched.map((item, index) => (<LocationItem key={index} item={item} index={index} isSelected={index == selectedIndex} handleSelected={handleSelected}></LocationItem>))
          }
        </ScrollView>
        <TouchableOpacity style={styles.btnConfirm} onPress={() => handleConfirm()}>
          <Text style={{ color: 'white', fontSize: 14 }}>Xác nhận</Text>
        </TouchableOpacity>
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  map: {
    width: "100%",
    height: "100%"
  },
  bottomContainer: {
    flex: 1,
    borderRadius: 16,
    backgroundColor: "white",
    gap: 10
  },
  btnConfirm: {
    backgroundColor: CssConfig.mainColor,
    height: 50,
    justifyContent: "center",
    alignItems: "center",
    borderRadius: 10,
    marginBottom: 25,
    marginHorizontal: CssConfig.paddingHorizontal
  },
  iconContainer: {
    width: 40,
    height: 40,
    borderRadius: "50%",
    backgroundColor: "white",
    alignItems: "center",
    justifyContent: "center",
    position: "absolute",
    top: 55,
    left: CssConfig.paddingHorizontal,
    zIndex: 20,
    elevation: 5
  }
});

export default PickLocationScreen;


