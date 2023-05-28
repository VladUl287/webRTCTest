import instance from '@/http/hub'
import { ref } from 'vue'
import { defineStore } from 'pinia'
import type { Chat } from '@/types/chat'

export const useChatStore = defineStore('chat', () => {
    const chats = ref<Chat[]>([])

    const loading = async (chat: string): Promise<string | void> => {
        const params = new URLSearchParams(chat)

        try {
            const result = await instance.get<Chat[]>('/chat', { params });
            chats.value = result.data
        } catch (error) {
            // const typedError = (error as Error)
            console.log(error);
        }
    }

    return { dialogs: chats, loading }
})
