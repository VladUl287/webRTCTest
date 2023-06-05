import { ref } from 'vue'
import { defineStore } from 'pinia'
import instance from '@/http/hub'
import type { Message } from '@/types/chat'

export const useMessagesStore = defineStore('messages', () => {
    const messages = ref<Message[]>([
        {
            id: '1',
            content: 'test',
            date: new Date().toDateString(),
            edit: false,
            userId: 1
        }
    ])

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
