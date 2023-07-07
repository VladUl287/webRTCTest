import { ref } from 'vue'
import { defineStore } from 'pinia'
import instance from '@/http'

export const useCallStore = defineStore('call', () => {
    const call = ref<any>()

    const getCall = async (callId: string): Promise<any> => {
        try {
            const result = await instance.get<any>('/api/calls/getcall', { params: { callId } })
            return result.data
        } catch (error) {
            console.log(error)
        }
    }

    return { call, getCall }
})
