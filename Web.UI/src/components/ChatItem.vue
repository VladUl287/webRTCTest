<template>
    <div class="chat-item" :data-time="getTime(chat.message.date)">
        <img :src="chat.image" width="100" height="100" />
        <div class="chat-info">
            <p class="chat-name">{{ chat.name }}</p>
            <p class="chat-message">{{ chat.message.content }}</p>
        </div>
        <span class="chat-unread">{{ chat.unread }}</span>
    </div>
</template>
  
<script setup lang="ts">
import type { PropType } from 'vue';
import type { Chat } from '@/types/chat';

defineProps({
    chat: {
        type: Object as PropType<Chat>,
        required: true
    }
})

const getTime = (dateString: string) => {
    const date = new Date(dateString)

    const isToday = (date: Date) => {
        return new Date().toDateString() === date.toDateString()
    }

    if (isToday(date)) {
        return new Intl.DateTimeFormat('ru', { hour: '2-digit', minute: '2-digit', second: '2-digit' } as Intl.DateTimeFormatOptions).format(date)
    }

    return new Intl.DateTimeFormat('ru').format(date)
}


</script>
  
<style>
.chat-item {
    position: relative;
    display: grid;
    cursor: pointer;
    overflow: hidden;
    column-gap: .5em;
    align-items: center;
    border-radius: .5em;
    grid-template-columns: min-content auto min-content;
    background-color: rgba(255, 0, 0, 0.1);
}

.chat-item::after {
    content: attr(data-time);
    position: absolute;
    right: 10px;
    top: 5px;
    color: black;
    font-size: .6em;
}

.chat-item:hover,
.chat-item:focus {}

.chat-item img {
    max-width: 50px;
    max-height: 50px;
}

.chat-item .chat-info {
    overflow: hidden;
}

.chat-item p {
    margin: 0;
    overflow: hidden;
    user-select: none;
    white-space: nowrap;
    text-overflow: ellipsis;
}

.chat-item .chat-name {}

.chat-item .chat-message {
    color: black;
    font-size: .7em;
}

.chat-item .chat-unread {
    margin: 0 .5em 0 0;
}
</style>