import { ref } from 'vue'
import { defineStore } from 'pinia'
import instance from '@/http'
import type { Message } from '@/types/chat'

export const useMessagesStore = defineStore('messages', () => {
    const messages = ref<Message[]>([])

    const getMessages = async (chatId: string): Promise<void> => {
        try {           
            messages.value = []

            const result = await instance.get<Message[]>('/api/messages/getmessages', { params: { chatId } });
            
            messages.value = result.data
        } catch (error) {
            console.log(error);
        }
    }

    return { messages, getMessages }
})
