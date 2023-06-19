<template>
    <div class="search-field" :class="{ 'active': active }">
        <input type="text" name="search" id="search" placeholder="search" @input="search">
        <section class="items-list">
            <div v-for="item of items" :key="item.key">{{ item.value }}</div>
        </section>
    </div>
</template>

<script setup lang="ts">
import type { SearchItem } from '@/types/components'
import type { PropType } from 'vue'

defineProps({
    items: Object as PropType<SearchItem[]>,
    active: Boolean
})

const emits = defineEmits<{
    (e: 'search', value: string): void
}>()

let timeout: number
const search = (event: Event) => {
    clearTimeout(timeout)

    timeout = setTimeout(() => {
        const input = (event.target as HTMLInputElement)
        searchEmit(input.value)
    }, 800)
}

const searchEmit = async (value: string) => emits('search', value)
</script>

<style scoped>
.search-field.active {
    height: 100%;
    background-color: black;
}

#search {
    width: 100%;
    color: #fff;
    padding: 1em 1em;
    border-radius: 5em;
    background-color: transparent;
}

#search::placeholder {
    font-style: italic;
}

.items-list {
    display: none;
    width: 100%;
    background-color: transparent;
}

.search-field.active .items-list {
    display: block;
}

.items-list>div {
    padding: 1em;
    cursor: pointer;
    border-radius: .5em;
    margin: .5em .5em 0 .5em;
    border: 1px solid gray;
}
</style>