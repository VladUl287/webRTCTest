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
    min-width: 50%;
    max-width: 70%;
    margin: 0 auto;
}

.message-new #message-content {
    width: 100%;
    height: 5em;
    resize: none;
}

.message-new button {
    /* display: none; */
}
</style>