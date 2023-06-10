<template>
    <div class="message-new">
        <textarea name="message-content" id="message-content" cols="30" rows="10"></textarea>
        <button @click="send">
            <span class="material-symbols-outlined">
                send
            </span>
        </button>
    </div>
</template>
  
<script setup lang="ts">

const props = defineProps({
    chatId: String,
    disabled: Boolean
})

const emits = defineEmits<{
    (e: 'send-message', message: any): void
}>()

const send = () => emits('send-message', { chatId: props.chatId, content: 'send-message' })

</script>
  
<style>
.message-new {
    display: flex;
    padding: .5em 0;
    column-gap: .5em;
    align-items: center;
    justify-content: center;
}

.message-new #message-content {
    height: 3em;
    resize: none;
    padding: 1em 1em;
    border-radius: 5em;
}

.message-new button {
    border: none;
    height: 100%;
    border-radius: 5em;
}
</style>