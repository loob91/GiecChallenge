import axios from 'axios'

const API_URL = `${process.env.REACT_APP_API_URL}/user`;

//Register user
const register = async(userData) => {
    await axios.post(`${API_URL}/register`, userData);
}

const logout = () => {
    localStorage.removeItem('user')
}

//Login user
const login = async(userData) => {
    const response = await axios.post(`${API_URL}/login`, userData)
    if (response.data) {
        localStorage.setItem('user', JSON.stringify(response.data))
    }
    return response.data
}

const authService = {register, logout, login}

export default authService