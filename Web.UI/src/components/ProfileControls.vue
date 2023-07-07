<template>
    <div class="account-controls">
        <button @click="logout">
            <span class="material-symbols-outlined">move_item</span>
            Logout
        </button>
    </div>
</template>
<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()

const logout = () => authStore.logout()
</script>

<style scoped>
.account-controls {
    width: 100%;
    padding: .5em;
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