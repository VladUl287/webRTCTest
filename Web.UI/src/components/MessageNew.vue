<template>
    <div class="message-new">
        <div class="textarea-wrap">
            <div class="textarea" contenteditable data-placeholder="message" ref="textarea"></div>
        </div>
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
  
<style>
.message-new {
    width: 100%;
    display: grid;
    align-items: center;
    grid-template-columns: 1fr auto;
}

.textarea-wrap {
    padding: .7em 1em;
    border-radius: .5em;
    display: inline-block;
    border: 1px solid var(--color-border-light);
}

.textarea {
    resize: none;
    outline: none;
    max-width: 20rem;
    overflow-y: auto;
    max-height: 20rem;
}

.textarea[contenteditable]:empty::before {
    content: attr(data-placeholder);
    color: var(--color-placeholder);
}

.message-new button {
    display: inline-flex;
    margin: .5em;
    padding: .7em;
    cursor: pointer;
    border-radius: 5em;
    background-color: transparent;
    color: var(--color-placeholder);
    border: 1px solid var(--color-border-dark);
}
</style>