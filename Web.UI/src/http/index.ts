import axios from 'axios'
import { useAuthStore } from '@/stores/auth'

const instance = axios.create({
    baseURL: 'https://localhost:7010/',
    withCredentials: true
});

instance.interceptors.request.use(async (config: any) => {
    const { getUser } = useAuthStore()
    const user = await getUser()

    config.headers.Authorization = 'Bearer ' + user?.access_token;

    return config;
})

export default instance;