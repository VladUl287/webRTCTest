<template>
    <section class="messages-wrap">
        <div v-if="loading" class="loading">
            <LoadingRing />
        </div>
        <div v-else class="messages-list" ref="messagesList">
            <MessageItem v-for="message of messagesReverse" :key="message.id" :id="normalizeNodeId(message.id)"
                :message="message" :right="userId === message.userId" />
            <!-- :class="{ 'active': active(message.userId, message.date) }" /> -->
        </div>
    </section>
</template>
  
<script setup lang="ts">
import { computed, ref, type PropType, watch } from 'vue'
import type { Chat } from '@/types/chat'
import type { Message } from '@/types/message'
import MessageItem from '@/components/MessageItem.vue'
import LoadingRing from '@/components/controls/LoadingRing.vue'
import { normalizeNodeId } from '@/helpers/nodes'

// const messageVisibleRatio = .5

// const options = {
//     root: document.querySelector('.messages-list'),
//     threshold: messageVisibleRatio
// }

const messagesList = ref<HTMLElement>()

const props = defineProps({
    messages: {
        type: Object as PropType<Message[]>,
        required: true
    },
    userId: {
        type: Number,
        required: true
    },
    chat: {
        type: Object as PropType<Chat>,
        required: true
    },
    loading: Boolean
})

// const emit = defineEmits<{
//     (e: 'messageCheck', date: string): void
// }>()

watch(
    () => messagesList.value?.children,
    () => {
        scrollToFirstUnreadMessage()
        // observeLastMessage()
    },
    { flush: 'post' })

watch(
    () => props.messages,
    (messages) => {
        if (messages.length > 0) {
            const lastMessage = messages[messages.length - 1]

            if (lastMessage.userId == props.userId) {
                scrollToBottom()
            }
        }
    })

// watch(
//     () => props.chat?.lastRead,
//     () => observeLastMessage())

const messagesReverse = computed(() => [...props.messages].reverse())

// const active = (userId: number, date: string) => {
//     return props.userId && props.userId != userId && props.chat && props.chat.lastRead < date
// }

const scrollToFirstUnreadMessage = () => {
    const message = getFirstUnreadMessage()

    if (message) {
        const element = getMessageElement(message.id)
        element?.scrollIntoView({ block: "end" })
    }
}

// const observeLastMessage = (): void => {
//     const message = getFirstUnreadMessage()

//     if (message) {
//         const element = getMessageElement(message.id)

//         const observer = new IntersectionObserver((entries) => {
//             entries.forEach(entry => {
//                 if (entry.intersectionRatio >= messageVisibleRatio) {
//                     emit('messageCheck', message.date)
//                     observer.disconnect()
//                 }
//             })
//         }, options);

//         element && observer.observe(element)
//     }
// }

const getMessageElement = (id: string) => messagesList.value?.querySelector('#' + normalizeNodeId(id))

const getFirstUnreadMessage = () => {
    const chatLastReadDate = props.chat.lastRead

    return props.messages.find(
        (message) => message.userId != props.userId && message.date > chatLastReadDate)
}

const scrollToBottom = () => {
    messagesList.value && (messagesList.value.scrollTop = 0)
}
</script>
  
<style scoped>
/* .active {
    background-color: rgba(233, 106, 233, 0.2);
} */

.messages-wrap {
    height: 100%;
    padding: 0 5px;
    position: relative;
    overflow-y: hidden;
}

.messages-list {
    height: 100%;
    min-height: 600px;
    display: flex;
    overflow-y: auto;
    row-gap: var(--section-gap);
    flex-direction: column-reverse;
    padding: var(--section-gap) var(--section-gap) 0 0;
}

.loading {
    width: 4em;
    height: 4em;
    top: 50%;
    left: 50%;
    position: absolute;
    transform: translate(-50%, -50%);
}
</style>