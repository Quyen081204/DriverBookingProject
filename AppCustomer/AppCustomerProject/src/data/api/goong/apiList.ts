// API của goong
const API_LIST = {  
  Find_Place_from_text: '/place/findplacefromtext?api_key=',
  PlacesAutocomplete:'/place/autoComplete?api_key=',
  ForwardGeocoding:'/geocode?address=',
  ReverseGeocoding: '/geocode?latlng=',
  PlaceDetail:'/place/detail?place_id=',
  Directions:'/Direction?vehicle=',
};
export default API_LIST;

// /Place/AutoComplete?api_key=fKXvD2uoiAeN3QyXOP4kTnnSDDwpE2EuFyRzi7Xm&input= + body.search