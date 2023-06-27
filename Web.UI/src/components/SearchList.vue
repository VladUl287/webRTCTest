<template>
    <div class="content">
        <section v-if="items && items.length > 0" class="items-list">
            <button v-for="item of items" :key="item.key" @click="select(item)">{{ item.label }}</button>
        </section>
        <section v-else-if="loading" class="loading">
            <LoadingRing />
        </section>
        <section v-else class="empty-list">
            <span>empty</span>
        </section>
    </div>
</template>
  
<script setup lang="ts">
import type { PropType } from 'vue'
import type { SearchItem } from '@/types/components'
import LoadingRing from '@/components/controls/LoadingRing.vue'

defineProps({
    active: Boolean,
    items: Object as PropType<SearchItem[]>,
    loading: Boolean
})

const emits = defineEmits<{
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
</script>
  
<style scoped>
.content {
    height: 100%;
    overflow-y: auto;
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

.loading,
.empty-list {
    user-select: none;
    width: fit-content;
    margin: 50% auto 0 auto;
}

.loading {
    width: 4em;
    height: 4em;
}
</style>