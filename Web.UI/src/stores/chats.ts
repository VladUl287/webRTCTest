import { ref } from 'vue'
import { defineStore } from 'pinia'
import instance from '@/http'
import type { Chat } from '@/types/chat'

export const useChatsStore = defineStore('chat', () => {
    const chats = ref<Chat[]>([])

    const getChat = async (chatId: string): Promise<Chat | undefined> => {
        try {
            const result = await instance.get<Chat>('/api/chats/getchat', { params: { chatId } })

            const index = chats.value.findIndex(chat => chat.id === result.data.id)

            if (index > -1) {
                chats.value[index] = result.data
            }

            return result.data
        } catch (error) {
            console.log(error);
        }
    }

    const getChats = async (): Promise<void> => {
        try {
            const result = await instance.get<Chat[]>('/api/chats/getchats');
            chats.value = result.data
        } catch (error) {
            console.log(error);
        }
    }

    return { chats, getChats, getChat }
})
