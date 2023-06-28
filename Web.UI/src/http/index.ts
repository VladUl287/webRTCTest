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

instance.interceptors.response.use(
    (config: any) => config,
    (error: any) => {
        if (error.response.status === 401) {
            console.log('401');
            
            return useAuthStore().silentRenew()
        }
        return Promise.reject(error);
    })

export default instance;