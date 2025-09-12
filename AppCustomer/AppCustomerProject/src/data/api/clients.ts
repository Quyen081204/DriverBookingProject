import axios from "axios";

export const endpoints = {
    login: '/auth/login/',
    getCurrentUser: '/Customer/get_customer_profile/',
    registerUser: '/Customer/register'
}

const host = "https://968c7afa9075.ngrok-free.app";
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
