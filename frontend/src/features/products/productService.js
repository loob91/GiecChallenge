import axios from 'axios'

const API_URL = `${process.env.REACT_APP_API_URL}/product`;

//Create product
const create = async(productData, token) => {
    const response = await axios.post(API_URL, productData, getHeader(token))
    return response.data
}

//Get all products
const getproducts = async(token) => {
    const response = await axios.get(API_URL, getHeader(token))
    return response.data
}

//Get a product
const getproduct = async(id, token) => {
    const response = await axios.get(`${API_URL}/${id}`, getHeader(token))
    return response.data
}

//Get a product
const getproductbyname = async(name, language, token) => {
    const response = await axios.get(`${API_URL}/name/${language}/${name}`, getHeader(token))
    return response.data
}

//Update a product
const closeproduct = async(id, token) => {
    const response = await axios.put(`${API_URL}/${id}`, {status: 'close'}, getHeader(token))
    return response.data
}

const getHeader = (token) => {
    return { headers: { 
            Authorization: `Bearer ${token}`
        } 
    }
};

const productService = {create, getproducts, getproduct, getproductbyname, closeproduct}

export default productService