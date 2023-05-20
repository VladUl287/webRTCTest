import axios from 'axios'

const instance = axios.create({
    baseURL: 'https://localhost:7242/',
    withCredentials: true
})

export default instance