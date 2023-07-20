<template>
    <div v-if="chat" class="chat-head">
        <section class="img-wrap">
            <AvatarImg :src="chat.image" alter="https://html.com/wp-content/uploads/flamingo4x.jpg" />
        </section>
        <section>
            <p class="chat-name">{{ chat.name }}</p>
        </section>
        <section class="chat-actions">
            <button @click="emits('startCall')" class="base-btn">
                <span class="material-symbols-outlined">call</span>
            </button>
        </section>
    </div>
    <div v-else class="chat-head">
        loading...
    </div>
</template>
  
<script setup lang="ts">
import type { PropType } from 'vue'
import type { Chat } from '@/types/chat'
import AvatarImg from './controls/AvatarImg.vue'

defineProps({
    chat: Object as PropType<Chat>
})

const emits = defineEmits<{
    (e: 'startCall'): void
}>()
</script>
  
<style scoped>
.chat-head {
    padding: .5em;
    display: flex;
    column-gap: 1em;
    user-select: none;
    align-items: center;
    background-color: var(--color-background-dark);
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
</style>