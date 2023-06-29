<template>
    <section class="messages-wrap">
        <div v-if="loading" class="loading">
            <LoadingRing />
        </div>
        <div v-else class="messages-list" ref="messagesList">
            <MessageItem v-for="message of messagesReverse" :key="message.id" :id="normalizeId(message.id)"
                :message="message" :right="userId == message.userId"
                :class="{ 'active': message.userId != userId && message.date > chat.lastRead }" />
        </div>
    </section>
</template>
  
<script setup lang="ts">
import { computed, ref, type PropType, watch, onMounted } from 'vue'
import type { Message, Chat } from '@/types/chat'
import MessageItem from '@/components/MessageItem.vue'
import LoadingRing from '@/components/controls/LoadingRing.vue'

const messagesList = ref<HTMLElement>()

const props = defineProps({
    messages: {
        type: Object as PropType<Message[]>,
        required: true
    },
    chat: {
        type: Object as PropType<Chat>,
        required: true
    },
    userId: {
        type: String,
        required: true
    },
    loading: Boolean
})

const emit = defineEmits<{
    (e: 'messageCheck', date: string): void
}>()

watch(
    () => messagesList.value?.childNodes,
    () => {
        scrollToLastReaded()
    })

watch(
    () => props.messages,
    (messages: Message[]) => {
        const lastMessage = messages[messages.length - 1]

        if (lastMessage.userId == props.userId) {
            scrollToBottom()
        }
    })

watch(
    () => props.chat?.lastRead,
    () => {
        observeLastMessage()
    })

const messagesReverse = computed(() => [...props.messages].reverse())

const options = {
    root: document.querySelector(".messages-list"),
    threshold: .5
}

const scrollToLastReaded = () => {
    const message = getUnreadMessage()
    if (message) {
        const element = getMessageElement(message.id)

        element?.scrollIntoView({ block: "end" })

        observeLastMessage()
    }
}

const observeLastMessage = (): void => {
    const message = getUnreadMessage()

    if (message) {
        const element = getMessageElement(message.id)

        const observer = new IntersectionObserver(() => {
            emit('messageCheck', message?.date)
            observer.disconnect()
        }, options);

        element && observer.observe(element)
    }
}

const getMessageElement = (id: string) => {
    return messagesList.value?.querySelector('#' + normalizeId(id))
}

const getUnreadMessage = () => {
    if (props.chat) {
        const chatLastReadDate = new Date(props.chat.lastRead)

        return props.messages.find(
            (message) => message.userId != props.userId && new Date(message.date) > chatLastReadDate)
    }
}

const scrollToBottom = () => {
    messagesList.value && (messagesList.value.scrollTop = 0)
}

const normalizeId = (value: string) => `a${value}`
</script>
  
<style scoped>
.active {
    background-color: violet;
}

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
    padding: .5em .5em 0 0;
    flex-direction: column-reverse;
}

.loading {
    width: 4em;
    height: 4em;
    margin: 50% auto 0 auto;
}
</style>