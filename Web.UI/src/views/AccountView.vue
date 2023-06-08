<template>
    <section>
    </section>
</template>
  
<script setup lang="ts">
import { useAuthStore } from '@/stores/auth';

var auth = useAuthStore()

if (await auth.getUser() === null) {
    await auth.login()
}
else {
    window.location.href = '../chat'
}

</script>
  
<style scoped></style>