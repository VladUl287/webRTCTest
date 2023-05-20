import instance from '@/http/hub'
import { ref } from 'vue'
import { defineStore } from 'pinia'
import type { Dialog } from '@/types/dialogs'

export const useDialogStore = defineStore('dialog', () => {
    const dialogs = ref<Dialog[]>([])

    //   const doubleCount = computed(() => count.value * 2)

    const loading = async (dialog: string): Promise<string | void> => {
        const params = new URLSearchParams(dialog)

        try {
            const result = await instance.get<Dialog[]>('/dialogs', { params });
            dialogs.value = result.data
        } catch (error) {
            // const typedError = (error as Error)
            console.log(error);
        }
    }

    return { dialogs, loading }
})
