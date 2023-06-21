<template>
    <section class="messages-wrap">
        <div v-if="loading" class="loading">
            <LoadingRing />
        </div>
        <div v-else class="messages-list" ref="list">
            <MessageItem v-for="message of messagesReverse" :key="message.id" :message="message"
                @click="action(message, MessageAction.Delete)" :right="user.profile.sub == message.userId" />
        </div>
    </section>
</template>
  
<script setup lang="ts">
import { computed, onUpdated, ref, type PropType } from 'vue'
import { MessageAction, type Message } from '@/types/chat'
import MessageItem from '@/components/MessageItem.vue'
import type { User } from 'oidc-client'
import LoadingRing from '@/components/helpers/LoadingRing.vue'

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

const messagesReverse = computed(() => [...props.messages].reverse())

const list = ref<HTMLElement>()

onUpdated(() => {
    scrollToBottom()
})

const scrollToBottom = () => {
    if (list.value) {
        list.value.scrollTop = -1
    }
}

const action = (message: Message, messageAction: MessageAction) => {
    // const newMessage = { ...message, content: 'new-content' }
    // emit('action', { message: newMessage, messageAction: MessageAction.Update })

    // emit('action', { message, messageAction })
}
</script>
  
<style scoped>
.messages-wrap {
    height: 100%;
    padding: 0 5px;
    position: relative;
    overflow-y: hidden;
}

.messages-list {
    height: 100%;
    display: flex;
    row-gap: .5em;
    overflow-y: auto;
    padding: 0 10px 0 0;
    flex-direction: column-reverse;
}

.loading {
    width: 4em;
    height: 4em;
    margin: 50% auto 0 auto;
}
</style>