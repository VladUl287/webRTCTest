import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import instance from '@/http'
import type { Message } from '@/types/chat'

export const useMessageStore = defineStore('messages', () => {
    const _messages = ref<Message[]>([])

    const messages = computed(() => [..._messages.value])

    const addMessage = (message: Message) => _messages.value.push(message)

    const getMessages = async (chatId: string): Promise<void> => {
        try {
            _messages.value = []

            const result = await instance.get<Message[]>('/api/messages/getmessages', { params: { chatId } });

            _messages.value = result.data
        } catch (error) {
            console.log(error);
        }
    }

    return { messages, getMessages, addMessage }
})
