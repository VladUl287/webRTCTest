import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import instance from '@/http'
import type { Call } from '@/types/call'

export const useCallStore = defineStore('call', () => {
    const _call = ref<Call | undefined>()

    const call = computed(() => _call.value)

    const getCall = async (callId: string): Promise<void> => {
        try {
            if (callId) {
                const result = await instance.get<Call>('/api/calls/getcall', { params: { callId } })

                _call.value = result.data
            }
            else {
                _call.value = undefined
            }
        } catch (error) {
            console.log(error)
        }
    }

    return { call, getCall }
})
