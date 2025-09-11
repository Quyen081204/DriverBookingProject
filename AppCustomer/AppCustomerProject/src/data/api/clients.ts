import axios from "axios";
import AsyncStorage from "@react-native-async-storage/async-storage";

export const endpoints = {
    login: '/auth/login/',
    getCurrentUser: '/customer/get_customer_profile/' 
}

const host = "https://1d1ec872acb7.ngrok-free.app";
const BASE_URL = `${host}/api`

export const authApis = (token) => {
  return axios.create({
    baseURL: BASE_URL,
    headers: {
      'Authorization': `Bearer ${token}`
    }
  })};

export default axios.create({
    baseURL: BASE_URL
});
