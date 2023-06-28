<template>
    <input type="text" id="search" placeholder="search" autocomplete="off" @input="search">
</template>

<script setup lang="ts">

const emit = defineEmits<{
    (e: 'input', value: string): void
}>()

let timeout: number | undefined
const search = (event: Event) => {
    clearTimeout(timeout)

    timeout = setTimeout(() => {
        const input = (event.target as HTMLInputElement)
        emit('input', input.value)
    }, 600)
}
</script>

<style scoped>
#search {
    width: 100%;
    padding: .8em 1em;
    font-size: medium;
    border-radius: 5em;
    color: var(--color-text);
    background-color: transparent;
    border: 1px solid var(--color-border-dark);
}

#search::placeholder {
    user-select: none;
    font-style: italic;
    color: var(--color-placeholder)
}
</style>