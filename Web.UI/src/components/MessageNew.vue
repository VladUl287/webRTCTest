<template>
    <div class="message-new">
        <div class="textarea-wrap">
            <div class="textarea" contenteditable data-placeholder="message" ref="textarea"></div>
        </div>
        <button @click="send">
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

const send = () => {
    if (textarea.value && textarea.value.innerText) {
        emits('send', textarea.value.innerText)
    }
}

</script>
  
<style>
.message-new {
    margin: 0 auto;
    padding: .5em 0;
    width: fit-content;
}

.textarea-wrap {
    padding: .7em 1em;
    border-radius: 5em;
    display: inline-block;
    border: 1px solid var(--color-border-light);
}

.textarea {
    padding: 0 .5em 0 0;
    width: 20rem;
    resize: none;
    outline: none;
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