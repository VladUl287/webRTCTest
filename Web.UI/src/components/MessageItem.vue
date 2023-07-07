<template>
    <div v-if="message" class="message-item" :class="{ 'right': right }">
        <p>{{ message.content }}</p>
        <p class="message-date">{{ getTime(message.date) }}</p>
    </div>
</template>
  
<script setup lang="ts">
import type { PropType } from 'vue'
import type { Message } from '@/types/message'

defineProps({
    message: {
        type: Object as PropType<Message>,
        required: true
    },
    right: Boolean
})

const formatOptions: Intl.DateTimeFormatOptions = { hour: '2-digit', minute: '2-digit', second: '2-digit' }
const timeFormat = new Intl.DateTimeFormat('ru', formatOptions)

const getTime = (dateString: string) => dateString && timeFormat.format(new Date(dateString))
</script>
  
<style>
.message-item {
    max-width: 60%;
    min-width: 15%;
    padding: .5em 1em;
    border-radius: 1em;
    width: fit-content;
    scroll-margin-top: .5em;
    background-color: var(--indigo);
}

.message-item p {
    margin: 0;
    word-break: break-all;
}

.message-item .message-date {
    font-size: .5em;
    user-select: none;
    text-align: right;
}

.right {
    margin-left: auto;
    background-color: var(--color-main-dark);
}
</style>