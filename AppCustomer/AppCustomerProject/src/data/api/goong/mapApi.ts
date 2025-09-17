import goongClient from "./goongClient";
import API_LIST from "./apiList";
import { GOONG_API_KEY } from "./constant";
import { mapDataToTextAddress } from "../mapData";

/**
 * This class used for call API and get response
 * 
 */

interface queryParamGeocoding {
    limit: number,
    has_deprecated_administrative_unit: boolean
}

interface queryParamAutocomplete{
    radius: number,
    location: string
}


class MapAPI {
    getAutocomplete = (bodyRequest: {search: string, queryParam:queryParamAutocomplete}) => {
        return goongClient.get(API_LIST.PlacesAutocomplete + GOONG_API_KEY + "&input=" + bodyRequest.search,bodyRequest.queryParam)
    }
    getPlaceDetail  = (bodyRequest: {place_id:string}) => {
        return goongClient.get(API_LIST.PlaceDetail + bodyRequest.place_id + '&api_key=' + GOONG_API_KEY)
    }
    getForwardGeocoding = (bodyRequest: {description:string}) => {
        return goongClient.get(API_LIST.ForwardGeocoding + bodyRequest.description + '&api_key=' + GOONG_API_KEY)
    }
    getReverseGeocoding = (bodyRequest: {description:string})=> {
        return goongClient.get(API_LIST.ReverseGeocoding + bodyRequest.description + '&api_key=' + GOONG_API_KEY)
    }
}

export default new MapAPI();


// Test MapAPI và sử dụng nó để lấy kết quả auto complete đáng lẽ viết cái reverse mới đúng
// test xong lấy kết quả vị trí nguwoif dùng và set vào điểm đón cho nguwoif dùng

