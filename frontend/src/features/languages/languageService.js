import axios from 'axios'

const API_URL = `${process.env.REACT_APP_API_URL}/language`;

//Create language
const create = async(languageData, token) => {
    const response = await axios.post(API_URL, languageData, getHeader(token))
    return response.data
}

//Get all languages
const getlanguages = async(token) => {
    const response = await axios.get(API_URL, getHeader(token))
    return response.data
}

//Get a language
const getlanguageById = async(id, token) => {
    const response = await axios.get(`${API_URL}/${id}`, getHeader(token))
    return response.data
}

//Get a language
const getlanguagebyname = async(name, language, token) => {
    const response = await axios.get(`${API_URL}/name/${language}/${name}`, getHeader(token))
    return response.data
}

//Update a language
const closelanguage = async(id, token) => {
    const response = await axios.put(`${API_URL}/${id}`, {status: 'close'}, getHeader(token))
    return response.data
}

const getHeader = (token) => {
    return { headers: { 
            Authorization: `Bearer ${token}`
        } 
    }
};

const languageService = {create, getlanguages, getlanguageById, getlanguagebyname, closelanguage}

export default languageService