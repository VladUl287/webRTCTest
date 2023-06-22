<template>
    <div v-if="chat" class="chat-head">
        <section class="img-wrap">
            <img :src="chat.image" @error="(event: Event) => imgError(event)" alt="Avatar" />
        </section>
        <section>
            <p class="chat-name">{{ chat.name }}</p>
        </section>
        <section class="chat-actions">
            <button @click="emits('startCall')" class="chat-action">
                <span class="material-symbols-outlined">call</span>
            </button>
        </section>
    </div>
    <div v-else>
        loading...
    </div>
</template>
  
<script setup lang="ts">
import type { PropType } from 'vue';
import type { Chat } from '@/types/chat';

defineProps({
    chat: Object as PropType<Chat>
})

const emits = defineEmits<{
    (e: 'startCall'): void
}>()

const imgError = (event: Event) => {
    const image = (event.target as HTMLImageElement)

    if (image) {
        image.src = 'https://html.com/wp-content/uploads/flamingo4x.jpg'
        image.onerror = null
    }
}

</script>
  
<style scoped>
.chat-head {
    display: flex;
    padding: 0 1em;
    column-gap: 1em;
    user-select: none;
    align-items: center;
    background-color: var(--color-background);
}

.img-wrap {
    width: 50px;
    height: 50px;
    max-width: 50px;
    max-height: 50px;
}

.img-wrap img {
    border-radius: 50%;
    width: 100%;
    height: 100%;
}

.chat-actions {
    margin-left: auto;
}

.chat-action {
    display: flex;
    padding: .5em;
    cursor: pointer;
    border-radius: 50%;
    background-color: transparent;
    color: var(--color-placeholder);
    border: 1px solid var(--color-border-dark);
}
</style>