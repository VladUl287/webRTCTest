<template>
    <section class="list-wrap">
        <div v-if="loading" class="loading-wrap">
            <div v-for="i in 6" :key="i">
                <SkeletonLoad />
            </div>
        </div>
        <div v-else>
            <ChatItem v-for="chat of chats" :key="chat.id" :chat="chat" :active="chat.id === select"
                @click="select(chat.id)" class="item" />
        </div>
    </section>
</template>
  
<script setup lang="ts">
import type { PropType } from 'vue'
import type { Chat } from '@/types/chat'
import ChatItem from '@/components/ChatItem.vue'
import SkeletonLoad from '@/components/controls/SkeletonLoad.vue'

defineProps({
    chats: {
        type: Object as PropType<Chat[]>,
        required: true
    },
    loading: Boolean,
    select: String
})

const emits = defineEmits<{
    (e: 'select', chatId: string): void
}>()

const select = (chatId: string) => emits('select', chatId)
</script>
  
<style>
.list-wrap {
    max-width: 100%;
}

.item {
    margin: .5em 0 0 0;
}

.loading-wrap>div {
    height: 60px;
    margin: 0 0 .5em 0;
}
</style>