<template>
    <input type="text" id="search" placeholder="search" ref="search" autocomplete="off" @input="search">
</template>

<script setup lang="ts">
import type { SearchItem } from '@/types/components'

const emits = defineEmits<{
    (e: 'input', value: string): void,
    (e: 'select', value: SearchItem): void
}>()

let timeout: number
const search = (event: Event) => {
    clearTimeout(timeout)

    timeout = setTimeout(() => {
        const input = (event.target as HTMLInputElement)
        emits('input', input.value)
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