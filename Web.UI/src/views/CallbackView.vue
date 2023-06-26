<template>
    <div>
        Loading...
    </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'

useAuthStore()
    .signingRedirectCallback(() => window.location.href = '../')

</script>