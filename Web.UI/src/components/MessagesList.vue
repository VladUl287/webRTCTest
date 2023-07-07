<template>
    <section class="messages-wrap">
        <div v-if="loading" class="loading">
            <LoadingRing />
        </div>
        <div v-else class="messages-list" ref="messagesNode">
            <MessageItem v-for="message of messagesReverse" :key="message.id" :id="normalizeNodeId(message.id)"
                :message="message" :right="userId == message.userId"
                :class="{ 'active': active(message.userId, message.date) }" />
        </div>
    </section>
</template>
  
<script setup lang="ts">
import { computed, ref, type PropType, watch } from 'vue'
import type { Chat } from '@/types/chat'
import type { Message } from '@/types/message'
import { debounce } from '@/helpers/debounce'
import MessageItem from '@/components/MessageItem.vue'
import LoadingRing from '@/components/controls/LoadingRing.vue'
import { normalizeNodeId } from '@/helpers/nodes'

const messagesNode = ref<HTMLElement>()

const props = defineProps({
    messages: {
        type: Object as PropType<Message[]>,
        required: true
    },
    userId: {
        type: Number,
        required: true
    },
    chat: Object as PropType<Chat>,
    loading: Boolean
})

const emit = defineEmits<{
    (e: 'messageCheck', date: string): void
}>()

watch(
    () => messagesNode.value?.childNodes,
    () => {
        scrollToLastReaded()
    })

watch(
    () => props.messages,
    (messages: Message[]) => {
        if (messages.length > 0) {
            const lastMessage = messages[messages.length - 1]

            if (lastMessage.userId == props.userId) {
                scrollToBottom()
            }
        }
    })

watch(
    () => props.chat?.lastRead,
    () => {
        observeLastMessage()
    })

const messagesReverse = computed(() => [...props.messages].reverse())

const active = (userId: number, date: string) => {
    return props.userId && props.userId != userId && props.chat && props.chat.lastRead < date
}

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
            message && messageCheck(message.date)
            observer.disconnect()
        }, options);

        element && observer.observe(element)
    }
}

const messageCheck = debounce((value: string) => emit('messageCheck', value), 5000)

const getMessageElement = (id: string) => {
    return messagesNode.value?.querySelector('#' + normalizeNodeId(id))
}

const getUnreadMessage = () => {
    if (props.chat) {
        const chatLastReadDate = new Date(props.chat.lastRead)

        return props.messages.find(
            (message) => message.userId != props.userId && new Date(message.date) > chatLastReadDate)
    }
}

const scrollToBottom = () => {
    messagesNode.value && (messagesNode.value.scrollTop = 0)
}
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