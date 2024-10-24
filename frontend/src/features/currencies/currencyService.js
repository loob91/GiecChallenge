import axios from 'axios'

const API_URL = `${process.env.REACT_APP_API_URL}/currency`;

//Create currency
const create = async(currencyData, token) => {
    const response = await axios.post(API_URL, currencyData, getHeader(token))
    return response.data
}

//Get all currencies
const getcurrencies = async(token) => {
    const response = await axios.get(API_URL, getHeader(token))
    return response.data
}

//Get a currency
const getcurrency = async(id, token) => {
    const response = await axios.get(`${API_URL}/${id}`, getHeader(token))
    return response.data
}

//Update a currency
const closecurrency = async(id, token) => {
    const response = await axios.put(`${API_URL}/${id}`, {status: 'close'}, getHeader(token))
    return response.data
}

const getHeader = (token) => {
    return { headers: { 
            Authorization: `Bearer ${token}`
        } 
    }
};

const currencyService = {create, getcurrencies, getcurrency, closecurrency}

export default currencyService