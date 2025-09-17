import axios, { AxiosInstance } from "axios";
import _ from 'lodash'
import { GOONG_HOST } from "./constant";
import { mapData, mapDataToTextAddress, mapError } from "../mapData";

// Nhiệm vụ: Gửi api lên goong với url tương ứng

const addParams = (url:string, params: {}) => {
    // add params for url
    url += ``;
  _.keys(params).forEach(value => {
    url += `&${value}=${params[value]}`;
  });

  return url;
}

class Request {
    private apiClient:AxiosInstance;

    constructor() {
    this.apiClient = axios.create({
      baseURL: GOONG_HOST,
      headers: {
        'Content-Type': 'application/json',
        'X-Requested-With': 'XMLHttpRequest',
      }
    });
  }

    get(url:string, queryParam:{} = {}) {
        // addQuery param to url
        //console.log(addParams(url,queryParam))
        const requestData = this.apiClient
              .get(addParams(url,queryParam))
              .then(mapData)
              .catch(mapError);
        return requestData;
    }
}


export default new Request();
