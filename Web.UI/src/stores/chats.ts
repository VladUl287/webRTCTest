import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { type Chat, ChatType } from '@/types/chat'
import instance from '@/http'

export const useChatStore = defineStore('chat', () => {
    const _chat = ref<Chat | undefined>()
    const _chats = ref<Chat[]>([])
    const _chatsLoading = ref<boolean>()

    const chat = computed(() => _chat.value)
    const chats = computed(() => _chats.value)
    const chatsLoading = computed(() => _chatsLoading.value)

    const setLastRead = (date: string) => {
        if (_chat.value) {
            _chat.value.lastRead = date
        }
    }

    const getChat = async (chatId: string): Promise<Chat | undefined> => {
        try {
            const result = await instance.get<Chat>('/api/chats/getchat', { params: { chatId } })

            _chat.value = result.data

            const index = _chats.value.findIndex(chat => chat.id === result.data.id)

            if (index > -1) {
                _chats.value[index] = result.data
            }
            else {
                _chats.value.push(result.data)
            }

            return result.data
        } catch (error) {
            console.log(error)
        }
    }

    const getChats = async (): Promise<void> => {
        try {
            _chatsLoading.value = true

            const result = await instance.get<Chat[]>('/api/chats/getchats')

            _chats.value = result.data
        } catch (error) {
            console.log(error)

            _chats.value = []
        } finally {
            _chatsLoading.value = false
        }
    }

    const createDialog = async (firstUser: number, secondUser: number): Promise<string | undefined> => {
        try {
            const result = await instance.post<string>('/api/chats/create', {
                userId: firstUser,
                type: ChatType.dialog,
                users: [
                    { id: firstUser },
                    { id: secondUser },
                ]
            })

            return result.data
        } catch (error) {
            console.log(error)
        }
    }

    const create = async (chat: { name: string, image: string, userId: number, type: ChatType, users: { id: number }[] }): Promise<string | undefined> => {
        try {
            const result = await instance.post<string>('/api/chats/create', chat)

            return result.data
        } catch (error) {
            console.log(error)
        }
    }

    return { chat, chats, chatsLoading, createDialog, setLastRead, getChats, getChat, create }
})
