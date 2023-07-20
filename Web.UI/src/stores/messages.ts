import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import instance from '@/http'
import type { Message } from '@/types/message'

export const useMessageStore = defineStore('messages', () => {
    const _messages = ref<Message[]>([])
    const _messagesLoading = ref<boolean>()

    const messages = computed(() => [..._messages.value])
    const messagesLoading = computed(() => _messagesLoading.value)

    const addMessage = (message: Message) => _messages.value.push(message)

    const getMessages = async (chatId: string): Promise<void> => {
        try {
            _messagesLoading.value = true

            const messages = await instance.get<Message[]>('/api/messages/getmessages', { params: { chatId } });

            _messages.value = messages.data
        } catch (error) {
            console.log(error);

            _messages.value = []
        } finally {
            _messagesLoading.value = false
        }
    }

    return { messages, messagesLoading, getMessages, addMessage }
})
