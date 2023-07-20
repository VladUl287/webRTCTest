import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import usersInstance from '@/http/users'
import type { UserDto } from '@/types/user'
import type { SearchItem } from '@/types/components'

export const useSearchStore = defineStore('search', () => {
    const _items = ref<SearchItem[]>([])
    const _searching = ref<boolean>()

    const items = computed(() => [..._items.value])
    const searching = computed(() => _searching.value)

    const setItems = (items: SearchItem[]) => {
        _items.value = items
    }

    const loadUsers = async (name: string): Promise<void> => {
        try {
            if (name && name.length > 2) {
                _searching.value = true

                const response = await usersInstance.get<UserDto[]>('/api/users/getusers', { params: { name: name } });

                _items.value = response.data.map(user => ({
                    key: user.id,
                    image: user.image,
                    value: user.chatId,
                    label: user.userName
                }))
            }
            else {
                _items.value = []
            }
        } catch (error) {
            console.log(error);

            _items.value = []
        } finally {
            _searching.value = false
        }
    }

    return { items, searching, loadUsers, setItems }
})
