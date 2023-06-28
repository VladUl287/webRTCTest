import { ref } from 'vue'
import { defineStore } from 'pinia'
import instance from '@/http'
import type { Chat } from '@/types/chat'

export const useChatStore = defineStore('chat', () => {
    const chats = ref<Chat[]>([])

    const getChat = async (chatId: string): Promise<Chat | undefined> => {
        try {
            const result = await instance.get<Chat>('/api/chats/getchat', { params: { chatId } })

            console.time()

            const index = chats.value.findIndex(chat => chat.id === result.data.id)

            if (index > -1) {
                chats.value[index] = result.data
            }

            console.timeEnd()

            return result.data
        } catch (error) {
            console.log(error);
            
            throw new Error("Error getting chat")
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

    const create = async (chat: { name: string, image: string, userId: number, type: number, users: any[] }): Promise<string | undefined> => {
        try {
            const result = await instance.post<string>('/api/chats/create', chat);

            return result.data
        } catch (error) {
            console.log(error);
        }
    }

    return { chats, getChats, getChat, create }
})
