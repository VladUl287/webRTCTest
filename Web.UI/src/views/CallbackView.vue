<template>
    <div class="content">
        <div>
            <LoadingRing />
        </div>
    </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'
import LoadingRing from '@/components/controls/LoadingRing.vue'

useAuthStore()
    .signingCallback(() => window.location.href = '../')

</script>

<style scoped>
.content {
    height: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
}

.content>div {
    width: 70px;
    height: 70px;
}
</style>