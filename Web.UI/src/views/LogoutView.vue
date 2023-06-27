<template>
    <div>
        Logout...
    </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'

useAuthStore().login()

</script>