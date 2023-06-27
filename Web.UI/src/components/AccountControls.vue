<template>
    <div class="account-controls">
        <button>
            <span class="material-symbols-outlined">add_circle</span>
            Create chat
        </button>
        <button>
            <span class="material-symbols-outlined">manage_accounts</span>
            Settings
        </button>
        <button @click="$emit('logout')">
            <span class="material-symbols-outlined">move_item</span>
            Logout
        </button>
    </div>
</template>
<script setup lang="ts">

defineEmits<{
    (e: 'logout'): void
}>()

</script>

<style scoped>
.account-controls {
    width: 100%;
    padding: 0 .5em;
}

.account-controls button {
    width: 100%;
    padding: 1em;
    display: flex;
    color: inherit;
    column-gap: 1em;
    align-items: center;
    margin-bottom: .5em;
    border-radius: .5em;
    background-color: transparent;
    border: 1px solid var(--color-border-dark);
}
</style>