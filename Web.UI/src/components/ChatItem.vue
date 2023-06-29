<template>
    <div class="chat-item" :class="{ 'active': active }" :data-time="getTime(chat.message?.date || chat.date)">
        <div class="img-wrap">
            <AvatarImg :src="chat.image" alter="https://html.com/wp-content/uploads/flamingo4x.jpg" />
        </div>
        <div class="chat-info">
            <p>{{ chat.name }}</p>
            <p class="chat-message" v-if="chat.message">{{ chat.message.content }}</p>
        </div>
        <span class="chat-unread" v-if="chat.unread > 0">{{ chat.unread }}</span>
    </div>
</template>
  
<script setup lang="ts">
import type { PropType } from 'vue';
import type { Chat } from '@/types/chat';
import AvatarImg from '@/components/controls/AvatarImg.vue'

defineProps({
    chat: {
        type: Object as PropType<Chat>,
        required: true
    },
    active: Boolean
})

const dateFormat = new Intl.DateTimeFormat('ru')
const timeFormat = new Intl.DateTimeFormat('ru', { hour: '2-digit', minute: '2-digit', second: '2-digit' })

const getTime = (dateString: string): string | undefined => {
    if (!dateString) return

    const date = new Date(dateString)

    if (isToday(date)) {
        return timeFormat.format(date)
    }

    return dateFormat.format(date)
}

const isToday = (date: Date) => {
    return new Date().toDateString() === date.toDateString()
}
</script>
  
<style scoped>
.chat-item {
    position: relative;
    display: flex;
    padding: .5em;
    cursor: pointer;
    column-gap: .5em;
    align-items: center;
    border-radius: .3em;
    border: 1px solid var(--color-border-light);
}

.chat-item.active {
    background-color: var(--color-active-light);
}

.chat-item::after {
    content: attr(data-time);
    position: absolute;
    font-size: .6em;
    right: 10px;
    top: 5px;
    color: var(--color);
}

.chat-item .img-wrap {
    width: 50px;
    height: 50px;
    user-select: none;
}

.img-wrap img {
    width: 100%;
    height: 100%;
    border-radius: 50%;
}

.chat-info {
    max-width: 75%;
}

.chat-info p {
    margin: 0 0 .2em 0;
    color: var(--color-text);
    overflow: hidden;
    white-space: nowrap;
    text-overflow: ellipsis;
}

.chat-info p:last-child {
    margin: 0
}

.chat-info .chat-message {
    font-size: .7em;
}

.chat-unread {
    display: block;
    margin-left: auto;
}
</style>