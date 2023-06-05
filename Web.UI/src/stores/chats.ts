import instance from '@/http/hub'
import { ref } from 'vue'
import { defineStore } from 'pinia'
import type { Chat } from '@/types/chat'

export const useChatsStore = defineStore('chat', () => {
    const chats = ref<Chat[]>([
        {
            id: '3219e2ec-a92b-4d04-8cd4-b0b0bbc01a93',
            name: 'Тестовое название чата',
            image: 'https://html.com/wp-content/uploads/flamingo4x.jpg',
            message: {
                date: new Date().toString(),
                content: 'yes'
            },
            unread: 5
        },
        {
            id: 'db528990-2eae-4c2d-b0f0-eeb7b7ccbd27',
            name: 'Длинное тестовое название чата для вылезаня',
            image: 'https://html.com/wp-content/uploads/flamingo4x.jpg',
            message: {
                date: new Date(2011, 0, 1, 2, 3, 4, 567).toString(),
                content: 'yes'
            },
            unread: 5
        }
    ])

    const getChats = async (): Promise<void> => {
        try {
            const result = await instance.get<Chat[]>('/api/chats/getchats');
            chats.value = result.data
        } catch (error) {
            console.log(error);
        }
    }

    return { chats, getChats }
})
