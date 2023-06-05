<template>
    <section class="chats-wrap">
        <div v-if="loading">loading...</div>
        <div v-else>
            <ChatItem v-for="chat of chats" :key="chat.id" :chat="chat" @click="selectChat(chat.id)" />
        </div>
    </section>
</template>
  
<script setup lang="ts">
import type { PropType } from 'vue'
import type { Chat } from '@/types/chat'
import ChatItem from '@/components/ChatItem.vue'

defineProps({
    chats: {
        type: Object as PropType<Chat[]>,
        required: true
    },
    loading: Boolean
})

const emits = defineEmits<{
    (e: 'select', chatId: string): void
}>()

const selectChat = (chatId: string) => emits('select', chatId)
</script>
  
<style>
.chats-wrap {
    max-width: 400px;
    min-width: 400px;
    
    height: 100%;
    padding: .5em;
    background-color: gray;
}

.chats-wrap>div {
    display: flex;
    row-gap: .5em;
    flex-direction: column;
}
</style>