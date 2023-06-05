import axios from 'axios'

const instance = axios.create({
    baseURL: 'https://localhost:7010/',
    withCredentials: true
})

export default instance