import axios from 'axios'

const API_URL = `${process.env.REACT_APP_API_URL}/purchase`;

//Create purchase
const create = async(purchaseData, token) => {
    const response = await axios.post(API_URL, purchaseData, getHeader(token))
    return response.data
}

//Create purchase La Ruche
const createLaRuche = async(purchaseData, token) => {
    const response = await axios.post(`${API_URL}/laruche`, purchaseData, getHeader(token))
    return response.data
}

//Get all purchases
const getpurchases = async(token) => {
    const response = await axios.get(API_URL, getHeader(token))
    return response.data
}

//Get all purchases by date
const getpurchasesbydate = async(dates, token) => {
    const response = await axios.get(`${API_URL}/${dates.startDate}/${dates.endDate}`, getHeader(token))
    return response.data
}

//Get CO2 emissions between two dates
const getCO2bydate = async(dates, token) => {
    const response = await axios.get(`${API_URL}/CO2/${dates.startDate}/${dates.endDate}`, getHeader(token))
    return response.data
}

//Get a purchase
const getpurchase = async(id, token) => {
    const response = await axios.get(`${API_URL}/${id}`, getHeader(token))
    return response.data
}

//Update a purchase
const updatepurchase = async(purchase, token) => {
    const response = await axios.put(API_URL, purchase, getHeader(token))
    return response.data
}

//Delete a purchase
const deletepurchase = async(purchaseId, token) => {
    const response = await axios.delete(`${API_URL}/${purchaseId}`, getHeader(token))
    return response.data
}

//Delete a purchase
const deletelinepurchase = async(linePurchaseId, token) => {
    const response = await axios.delete(`${API_URL}/line/${linePurchaseId}`, getHeader(token))
    return response.data
}

const getHeader = (token) => {
    return { headers: { 
            Authorization: `Bearer ${token}`
        } 
    }
};

const purchaseService = {create, createLaRuche, getpurchases, getpurchasesbydate, getCO2bydate, getpurchase, updatepurchase, deletepurchase, deletelinepurchase}

export default purchaseService