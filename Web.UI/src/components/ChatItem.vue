<template>
    <div class="chat-item" :data-time="getTime(chat.message?.date)" :class="{ 'active': active }">
        <div class="chat-img-wrap">
            <img :src="chat.image" @error="(event: Event) => imgError(event)" alt="Avatar" />
        </div>
        <div class="chat-info">
            <p class="chat-name">{{ chat.name }}</p>
            <p class="chat-message" v-if="chat.message">{{ chat.message.content }}</p>
        </div>
        <span class="chat-unread" v-if="chat.unread > 0">{{ chat.unread }}</span>
    </div>
</template>
  
<script setup lang="ts">
import type { PropType } from 'vue';
import type { Chat } from '@/types/chat';

defineProps({
    chat: {
        type: Object as PropType<Chat>,
        required: true
    },
    active: Boolean
})

const getTime = (dateString: string) => {
    if (!dateString) return

    const date = new Date(dateString)

    const isToday = (date: Date) => {
        return new Date().toDateString() === date.toDateString()
    }

    if (isToday(date)) {
        return new Intl.DateTimeFormat('ru', { hour: '2-digit', minute: '2-digit', second: '2-digit' } as Intl.DateTimeFormatOptions).format(date)
    }

    return new Intl.DateTimeFormat('ru').format(date)
}

const imgError = (event: Event) => {
    const image = (event.target as HTMLImageElement)

    if (image) {
        image.src = 'https://html.com/wp-content/uploads/flamingo4x.jpg'
        image.onerror = null
    }
}
</script>
  
<style>
.chat-item {
    display: flex;
    padding: .5em;
    cursor: pointer;
    column-gap: .5em;
    overflow: hidden;
    position: relative;
    align-items: center;
    border-radius: .3em;
    border: 1px solid #fff;

    --color: #fff;
}

.chat-item.active {
    background-color: #ffffff36;
}

.chat-item::after {
    content: attr(data-time);
    position: absolute;
    right: 10px;
    top: 5px;
    color: var(--color);
    font-size: .6em;
}

.chat-item .chat-img-wrap {
    width: 50px;
    height: 50px;
    max-width: 50px;
    max-height: 50px;
}

.chat-item .chat-img-wrap img {
    border-radius: 50%;
    width: 100%;
    height: 100%;
}

.chat-item .chat-info p {
    margin: 0;
    overflow: hidden;
    user-select: none;
    color: var(--color);
    white-space: nowrap;
    text-overflow: ellipsis;
}

.chat-info .chat-name {}

.chat-info .chat-message {
    font-size: .7em;
}

.chat-item .chat-unread {
    margin: 0 .5em 0 0;
}
</style>