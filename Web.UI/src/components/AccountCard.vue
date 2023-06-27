<template>
    <div v-if="user" class="user-card">
        <AvatarImg :src="''" alter="https://html.com/wp-content/uploads/flamingo4x.jpg" />

        <p>{{ user.profile.name }}</p>
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
    display: flex;
    column-gap: .5em;
    user-select: none;
    align-items: center;
    padding: 1em .5em;
}
</style>