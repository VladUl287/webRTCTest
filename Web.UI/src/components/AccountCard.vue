<template>
    <div class="user-card">
        <AvatarImg :src="''" alter="https://html.com/wp-content/uploads/flamingo4x.jpg" />

        <section>
            <p>{{ user?.profile.name }}</p>
        </section>
    </div>
</template>
<script setup lang="ts">
import type { User } from 'oidc-client';
import type { PropType } from 'vue';
import AvatarImg from '@/components/controls/AvatarImg.vue'

defineProps({
    user: Object as PropType<User | null>
})
</script>
<style scoped>
.user-card {
    min-height: 50px;
    padding: 1em;
    display: flex;
    column-gap: .5em;
    user-select: none;
    align-items: center;
    border-bottom: 1px solid var(--color-border-dark);
}
</style>