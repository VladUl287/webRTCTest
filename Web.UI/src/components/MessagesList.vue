<template>
    <section class="messages-wrap">
        <div v-if="loading">loading...</div>
        <div v-else class="messages-list">
            <MessageItem v-for="message of messages" :key="message.id" :message="message" />
        </div>
    </section>
</template>
  
<script setup lang="ts">
import type { PropType } from 'vue'
import type { Message } from '@/types/chat'
import MessageItem from '@/components/MessageItem.vue'
import type { User } from 'oidc-client'

defineProps({
    messages: {
        type: Object as PropType<Message[]>,
        required: true
    },
    user: {
        type: Object as PropType<User>,
        // required: true
    },
    loading: Boolean
})
</script>
  
<style>
.messages-wrap {
    overflow-y: hidden;
    padding: 0 5px;
}

.messages-list {
    max-height: 100%;
    overflow-y: auto;
}
</style>