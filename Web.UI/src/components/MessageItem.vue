<template>
    <div class="message-item" :class="{ 'right': proper }">
        <p>{{ message.content }}</p>
        <p class="message-date">{{ message.date }}</p>
    </div>
</template>
  
<script setup lang="ts">
import type { PropType } from 'vue';
import type { Message } from '@/types/chat';

defineProps({
    message: {
        type: Object as PropType<Message>,
        required: true
    },
    proper: Boolean
})
</script>
  
<style>
.message-item {
    max-width: 70%;
    padding: .5em 1em;
    border-radius: 1em;
    width: fit-content;
    background-color: rgba(137, 43, 226, 0.322);
}

.message-item p {
    margin: 0;
}

.message-item .message-date {
    font-size: .5em;
}

.message-item.right {
    margin-left: auto;
    background-color: rgb(124, 14, 14);
}
</style>