<template>
    <section class="message-new">
        <div class="textarea" contenteditable data-placeholder="message" ref="textarea"></div>
        <button @click="sendMessage" class="base-btn">
            <span class="material-symbols-outlined">
                send
            </span>
        </button>
    </section>
</template>
  
<script setup lang="ts">
import { ref } from 'vue'
import type { CreateMessage } from '@/types/message'

const props = defineProps({
    chatId: {
        type: String,
        required: true
    },
    disabled: Boolean
})

const emits = defineEmits<{
    (e: 'sendMessage', body: CreateMessage): void
}>()

const textarea = ref<HTMLElement>()

const sendMessage = () => {
    const content = textarea.value?.innerText

    if (props.chatId && content) {
        emits('sendMessage', { chatId: props.chatId, content })
    }
}
</script>
  
<style scoped>
.message-new {
    display: grid;
    align-items: center;
    padding: var(--section-gap) 0;
    column-gap: var(--section-gap);
    grid-template-columns: 1fr auto;
}

.textarea {
    resize: none;
    padding: .7em;
    overflow-y: auto;
    max-height: 20rem;
    border-radius: .5em;
    border: 1px solid var(--color-border-light);
}

.textarea:empty::before {
    content: attr(data-placeholder);
    color: var(--color-placeholder);
}
</style>