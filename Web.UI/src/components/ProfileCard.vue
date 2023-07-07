<template>
    <div class="user-card">
        <AvatarImg :src="authStore.profile?.image" alter="https://html.com/wp-content/uploads/flamingo4x.jpg" />

        <section>
            <p>{{ authStore.profile?.name }}</p>
        </section>
    </div>
</template>
<script setup lang="ts">
import AvatarImg from '@/components/controls/AvatarImg.vue'
import { useAuthStore } from '@/stores/auth';

const authStore = useAuthStore()

</script>

<style scoped>
.user-card {
    padding: 1em;
    display: flex;
    min-height: 50px;
    column-gap: .5em;
    user-select: none;
    align-items: center;
    border-bottom: 1px solid var(--color-border-dark);
}
</style>