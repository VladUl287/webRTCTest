<template>
    <div class="search-field" :class="{ 'active': modelValue }">
        <section class="search-wrap">
            <button @click="disable" class="disable">
                <span class="material-symbols-outlined">arrow_back</span>
            </button>
            <input type="text" id="search" placeholder="search" @input="search" ref="search" autocomplete="off">
        </section>
        <section v-if="items && items.length > 0" class="items-list">
            <button v-for="item of items" :key="item.key" @click="select(item)">{{ item.label }}</button>
        </section>
        <section v-else-if="loading" class="loading">
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
    modelValue: Boolean,
    items: Object as PropType<SearchItem[]>,
    loading: Boolean
})

const emits = defineEmits<{
    (e: 'update:modelValue', value: Boolean): void,
    (e: 'input', value: string): void,
    (e: 'select', value: SearchItem): void
}>()

const select = (item: SearchItem) => {
    emits('select', item)

    const searchInput = document.querySelector('#search') as HTMLInputElement
    if (searchInput) {
        searchInput.value = ''
    }
}

const disable = () => emits('update:modelValue', false)

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
.search-field {
    padding: .5em;
}

.search-field.active {
    height: 100%;
}

.search-wrap {
    display: flex;
    column-gap: .5em;
    align-items: center;
}

.search-wrap .disable {
    display: none;
    padding: .5em;
    cursor: pointer;
    user-select: none;
    border-radius: 50%;
    background-color: transparent;
    color: var(--color-placeholder);
    border: 1px solid var(--color-border-dark);
}

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

.items-list {
    display: none;
}

.items-list>button {
    width: 100%;
    padding: 1em;
    cursor: pointer;
    text-align: left;
    user-select: none;
    margin: .5em 0 0 0;
    border-radius: .5em;
    color: var(--color-text);
    background-color: transparent;
    border: 1px solid var(--color-border-dark);
}

.empty-list {
    display: none;
    user-select: none;
    width: fit-content;
    margin: 50% auto 0 auto;
}

.loading {
    width: 4em;
    height: 4em;
    margin: 50% auto 0 auto;
}

.search-field.active .items-list,
.search-field.active .empty-list {
    display: block
}

.search-field.active .search-wrap .disable {
    display: flex
}
</style>