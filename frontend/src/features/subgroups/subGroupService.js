import axios from 'axios'

const API_URL = `${process.env.REACT_APP_API_URL}/subGroup`;

//Create group
const create = async(subGroupData, token) => {
    const response = await axios.post(API_URL, subGroupData, getHeader(token))
    return response.data
}

//Get all groups
const getsubgroups = async(token) => {
    const response = await axios.get(API_URL, getHeader(token))
    return response.data
}

//Get a product
const getsubgroupsbyname = async(name, language, token) => {
    const response = await axios.get(`${API_URL}/name/${language}/${name}`, getHeader(token))
    return response.data
}

//Get a group
const getsubgroup = async(id, token) => {
    const response = await axios.get(`${API_URL}/${id}`, getHeader(token))
    return response.data
}

//Update a group
const updatesubgroup = async(subGroup, token) => {
    const response = await axios.put(API_URL, subGroup, getHeader(token))
    return response.data
}

//Delete a group
const deletesubgroup = async(subGroupId, token) => {
    const response = await axios.delete(`${API_URL}/${subGroupId}`, getHeader(token))
    return response.data
}

const getHeader = (token) => {
    return { headers: { 
            Authorization: `Bearer ${token}`
        } 
    }
};

const groupService = {create, getsubgroups, getsubgroup, getsubgroupsbyname, updatesubgroup, deletesubgroup}

export default groupService