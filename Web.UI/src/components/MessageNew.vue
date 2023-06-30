<template>
    <div class="message-new">
        <div class="textarea" contenteditable data-placeholder="message" ref="textarea"></div>
        <button @click="sendMessage">
            <span class="material-symbols-outlined">
                send
            </span>
        </button>
    </div>
</template>
  
<script setup lang="ts">
import { ref } from 'vue'

defineProps({
    disabled: Boolean
})

const emits = defineEmits<{
    (e: 'send', content: string): void
}>()

const textarea = ref<HTMLElement>()

const sendMessage = () => {
    if (textarea.value && textarea.value.innerText) {
        emits('send', textarea.value.innerText)
    }
}
</script>
  
<style scoped>
.message-new {
    width: 100%;
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

button {
    display: flex;
    padding: .7em;
    border-radius: 50%;
    background-color: transparent;
    color: var(--color-placeholder);
    border: 1px solid var(--color-border-dark);
}
</style>