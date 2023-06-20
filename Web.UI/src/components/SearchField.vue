<template>
    <div class="search-field" :class="{ 'active': active }">
        <section class="search-wrap">
            <input type="text" id="search" placeholder="search" @input="search" autocomplete="off">
        </section>
        <section class="items-list">
            <button v-for="item of items" :key="item.key" @click="select(item)">{{ item.label }}</button>
        </section>
        <section v-if="loading" class="loading">
            <LoadingRing />
        </section>
        <section v-else class="empty-list">
            empty
        </section>
    </div>
</template>

<script setup lang="ts">
import type { PropType } from 'vue'
import type { SearchItem } from '@/types/components'
import LoadingRing from '@/components/helpers/LoadingRing.vue'

defineProps({
    items: Object as PropType<SearchItem[]>,
    active: Boolean,
    loading: Boolean
})

const emits = defineEmits<{
    (e: 'input', value: string): void,
    (e: 'select', value: SearchItem): void
}>()

const select = (item: SearchItem) => emits('select', item)

let timeout: number
const search = (event: Event) => {
    clearTimeout(timeout)

    timeout = setTimeout(() => {
        const input = (event.target as HTMLInputElement)
        emits('input', input.value)
    }, 700)
}
</script>

<style scoped>
.search-field {
    padding: .5em;
}

.search-field.active {
    height: 100%;
}

.search-wrap {
    position: relative;
}

#search {
    width: 100%;
    padding: .8em 1em;
    font-size: medium;
    color: #fff;
    border-radius: 5em;
    background-color: transparent;
    border: 1px solid var(--color-border);
}

#search::placeholder {
    font-style: italic;
    color: var(--color-placeholder)
}

.items-list,
.empty-list,
.loading {
    display: none;
}

.empty-list {
    user-select: none;
    width: fit-content;
    margin: 50% auto 0 auto;
}

.search-field.active .items-list,
.search-field.active .loading {
    display: block;
}

.search-field.active .items-list:empty+.empty-list {
    display: block;
}

.items-list>button {
    width: 100%;
    padding: 1em;
    margin: .5em 0;
    display: block;
    cursor: pointer;
    text-align: left;
    border-radius: .5em;
    color: var(--color-text);
    background-color: transparent;
    border: 1px solid var(--color-border);
}

.loading {
    width: 4em;
    height: 4em;
    margin: 50% auto 0 auto;
}
</style>