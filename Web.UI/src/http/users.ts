import axios from 'axios'
import { useAuthStore } from '@/stores/auth'
import type { UserDto } from '@/types/user'

const instance = axios.create({
    baseURL: 'https://localhost:7250/'
})

instance.interceptors.request.use(async (config: any) => {
    const user = await useAuthStore().getUser()

    config.headers.Authorization = 'Bearer ' + user?.access_token;

    return config;
})

export const getUsers = async (name: string): Promise<UserDto[]> => {
    try {        
        const result = await instance.get<UserDto[]>('/api/users/getusers', { params: { name } });

        return result.data
    } catch (error) {
        console.log(error)
        return []
    }
}

export default instance