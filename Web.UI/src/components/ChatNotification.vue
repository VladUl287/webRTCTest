<template>
    <div class="chat-notification">
        <p>Call #1</p>
        <div class="notify-controls">
            <button class="success" @click="$emit('success')">
                <span class="material-symbols-outlined">call</span>
            </button>
            <button class="danger">
                <span class="material-symbols-outlined">call_end</span>
            </button>
        </div>
    </div>
</template>

<script setup lang="ts">

defineEmits<{
    (e: 'success'): void
}>()

</script>

<style scoped>
.chat-notification {
    display: flex;
    user-select: none;
    align-items: center;
    position: absolute;
    width: 100%;
    z-index: 999;
    padding: 0 1em;
    background-color: var(--color-active-light);
}

.chat-notification .notify-controls {
    margin-left: auto;
}

.notify-controls button {
    border: none;
    padding: .5em;
    cursor: pointer;
    margin-right: .5em;
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