<template>
    <div class="message-new">
        <div class="textarea-wrap">
            <div class="textarea" role="textbox" contenteditable>ауцау</div>
        </div>
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
    align-items: flex-end;
    justify-content: center;
}

.textarea-wrap {
    border-radius: 1em;
    border: 1px solid #ccc;
    padding: .5em .5em .5em 1em;
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
    content: "Message";
    color: gray;
}

.message-new button {
    padding: 1em;
    border: none;
    border-radius: 5em;
    height: fit-content;
}
</style>