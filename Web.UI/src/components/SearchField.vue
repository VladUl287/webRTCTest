<template>
    <input type="text" id="search" placeholder="search" autocomplete="off" :value="modelValue" @input="search">
</template>

<script setup lang="ts">
import { debounce } from '@/helpers/debounce'

defineProps({
    modelValue: String
})

const emit = defineEmits<{
    (e: 'update:modelValue', value: string): void,
    (e: 'input', value: string): void
}>()

const search = (event: Event) => {
    const input = (event.target as HTMLInputElement)

    emit('update:modelValue', input.value)
    searchDebounce(input.value)
}

const searchDebounce = debounce((value: string) => emit('input', value), 500)
</script>

<style scoped>
#search {
    width: 100%;
    padding: .8em;
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