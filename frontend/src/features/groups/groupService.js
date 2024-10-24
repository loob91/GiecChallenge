import axios from 'axios'

const API_URL = `${process.env.REACT_APP_API_URL}/group`;

//Create group
const create = async(groupData, token) => {
    const response = await axios.post(API_URL, groupData, getHeader(token))
    return response.data
}

//Get all groups
const getgroups = async(token) => {
    const response = await axios.get(API_URL, getHeader(token))
    return response.data
}

//Get a product
const getgroupsbyname = async(name, language, token) => {
    const response = await axios.get(`${API_URL}/name/${language}/${name}`, getHeader(token))
    return response.data
}

//Get a group
const getgroup = async(id, token) => {
    const response = await axios.get(`${API_URL}/${id}`, getHeader(token))
    return response.data
}

//Update a group
const updategroup = async(group, token) => {
    const response = await axios.put(API_URL, group, getHeader(token))
    return response.data
}

//Delete a group
const deletegroup = async(groupId, token) => {
    const response = await axios.delete(`${API_URL}/${groupId}`, getHeader(token))
    return response.data
}

const getHeader = (token) => {
    return { headers: { 
            Authorization: `Bearer ${token}`
        } 
    }
};

const groupService = {create, getgroups, getgroup, getgroupsbyname, updategroup, deletegroup}

export default groupService