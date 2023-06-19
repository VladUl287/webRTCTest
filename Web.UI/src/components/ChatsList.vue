<template>
    <section class="list-wrap">
        <div v-if="loading">loading...</div>
        <div v-else>
            <ChatItem v-for="chat of chats" :key="chat.id" :chat="chat" @click="selectChat(chat.id)"
                :active="selected === chat.id" class="chat-item" />
        </div>
    </section>
</template>
  
<script setup lang="ts">
import { type PropType, watch } from 'vue'
import type { Chat } from '@/types/chat'
import ChatItem from '@/components/ChatItem.vue'

const props = defineProps({
    chats: {
        type: Object as PropType<Chat[]>,
        required: true
    },
    selected: String,
    loading: Boolean
})

watch(
    () => props,
    () => {
        console.log('watch', props.selected)
    })

const emits = defineEmits<{
    (e: 'select', chatId: string): void
}>()

const selectChat = (chatId: string) => emits('select', chatId)
</script>
  
<style>
.list-wrap {
    padding: .5em;
}

.chat-item {
    margin: .5em 0;
}
</style>