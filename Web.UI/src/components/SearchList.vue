<template>
    <div>
        <section v-if="loading" class="list-body loading">
            <LoadingRing />
        </section>
        <section v-else-if="items.length === 0" class="list-body">
            <span>¯\_(ツ)_/¯</span>
        </section>
        <section v-else class="items-list">
            <button v-for="item of items" :key="item.key" @click="$emit('select', item)">
                <AvatarImg :src="item.image" alter="https://html.com/wp-content/uploads/flamingo4x.jpg" />
                {{ item.label }}
            </button>
        </section>
    </div>
</template>
  
<script setup lang="ts">
import type { PropType } from 'vue'
import type { SearchItem } from '@/types/components'
import AvatarImg from '@/components/controls/AvatarImg.vue'
import LoadingRing from '@/components/controls/LoadingRing.vue'

defineProps({
    items: {
        type: Object as PropType<SearchItem[]>,
        required: true
    },
    loading: Boolean,
    visible: Boolean
})

defineEmits<{
    (e: 'select', value: SearchItem): void
}>()
</script>
  
<style scoped>
.list-body {
    margin: 50% auto;
    user-select: none;
    font-size: x-large;
    width: fit-content;
}

.loading {
    width: 70px;
    height: 70px;
}

.items-list>button {
    width: 100%;
    display: flex;
    padding: .5em;
    column-gap: .5em;
    font-size: medium;
    margin: .5em 0 0 0;
    align-items: center;
    border-radius: .5em;
    overflow-y: auto;
    color: var(--color-text);
    background-color: transparent;
    border: 1px solid var(--color-border-dark);
}
</style>