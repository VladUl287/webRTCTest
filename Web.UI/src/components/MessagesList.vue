<template>
    <section class="messages-wrap">
        <div v-if="loading" class="loading">
            <LoadingRing />
        </div>
        <div v-else class="messages-list" ref="list">
            <MessageItem v-for="message of messagesReverse" :key="message.id" :message="message"
                :right="userId == message.userId" />
        </div>
    </section>
</template>
  
<script setup lang="ts">
import { computed, onUpdated, ref, type PropType } from 'vue'
import type { MessageAction, Message } from '@/types/chat'
import MessageItem from '@/components/MessageItem.vue'
import LoadingRing from '@/components/controls/LoadingRing.vue'

const props = defineProps({
    messages: {
        type: Object as PropType<Message[]>,
        required: true
    },
    userId: String,
    loading: Boolean
})

defineEmits<{
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