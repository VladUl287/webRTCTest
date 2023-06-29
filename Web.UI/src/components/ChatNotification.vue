<template>
    <div class="chat-notification">
        <p>Call #1</p>
        <div class="notify-controls">
            <button class="success" @click="$emit('success')">
                <span class="material-symbols-outlined">call</span>
            </button>
            <button class="danger" @click="$emit('reject')">
                <span class="material-symbols-outlined">call_end</span>
            </button>
        </div>
    </div>
</template>

<script setup lang="ts">

defineEmits<{
    (e: 'accept'): void,
    (e: 'reject'): void
}>()

</script>

<style scoped>
.chat-notification {
    width: 100%;
    display: flex;
    padding: 0 .5em;
    align-items: center;
    background-color: var(--color-active-light);
}

.chat-notification .notify-controls {
    margin-left: auto;
}

.notify-controls button {
    border: none;
    padding: .5em;
    cursor: pointer;
    margin-right: 1em;
    border-radius: 50%;
    display: inline-flex;
    color: var(--color-text)
}

.success {
    background-color: var(--color-success)
}

.danger {
    background-color: var(--color-danger);
}
</style>