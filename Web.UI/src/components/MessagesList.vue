<template>
    <section class="messages-wrap">
        <div v-if="loading">loading...</div>
        <div v-else class="messages-list" ref="list">
            <MessageItem v-for="message of messagesRev" :key="message.id" :message="message"
                :proper="user.profile.sub == message.userId" @click="action(message, MessageAction.Delete)" class="message-item"/>
        </div>
    </section>
</template>
  
<script setup lang="ts">
import { computed, onUpdated, ref, onMounted, type PropType } from 'vue'
import { MessageAction, type Message } from '@/types/chat'
import MessageItem from '@/components/MessageItem.vue'
import type { User } from 'oidc-client'

const props = defineProps({
    messages: {
        type: Object as PropType<Message[]>,
        required: true
    },
    user: {
        type: Object as PropType<User>,
        required: true
    },
    loading: Boolean
})

const emit = defineEmits<{
    (e: 'action', content: { message: Message, messageAction: MessageAction }): void
}>()

const messagesRev = computed(() => [...props.messages].reverse())

const list = ref<HTMLElement>()

onUpdated(() => {
    scrollToBottom()
})


const scrollToBottom = () => {
    if (list.value) {
        console.log(list.value.scrollTop, list.value.scrollHeight)
        list.value.scrollTop = -1
    }
}

const action = (message: Message, messageAction: MessageAction) => {

    const newMessage = { ...message, content: 'new-conte' }
    emit('action', { message: newMessage, messageAction: MessageAction.Update })

    // emit('action', { message, messageAction })
}
</script>
  
<style>
.messages-wrap {
    padding: 0 5px;
    overflow-y: hidden;
}

.messages-list {
    display: flex;
    flex-direction: column-reverse;
    overflow-y: auto;
    max-height: 100%;
    padding: 0 10px 0 0;
}

.messages-list>* {
    margin: 5px 0;
}
</style>