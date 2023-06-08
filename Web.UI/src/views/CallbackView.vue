<template>
    <div>
        Loading...
    </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()

authStore.signingRedirectCallback(() => window.location.href = '../')

</script>