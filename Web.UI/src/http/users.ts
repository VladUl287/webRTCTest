import axios from 'axios'
import { useAuthStore } from '@/stores/auth'

const instance = axios.create({
    baseURL: 'https://localhost:7250/'
})

instance.interceptors.request.use(async (config: any) => {
    const user = await useAuthStore().getUser()

    config.headers.Authorization = 'Bearer ' + user?.access_token;

    return config;
})

export default instance