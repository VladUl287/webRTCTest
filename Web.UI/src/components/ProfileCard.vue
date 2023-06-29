<template>
    <div class="user-card">
        <AvatarImg :src="profile?.image" alter="https://html.com/wp-content/uploads/flamingo4x.jpg" />

        <section>
            <p>{{ profile?.name }}</p>
        </section>
    </div>
</template>
<script setup lang="ts">
import type { Profile } from 'oidc-client';
import type { PropType } from 'vue';
import AvatarImg from '@/components/controls/AvatarImg.vue'

defineProps({
    profile: Object as PropType<Profile  | null>
})
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