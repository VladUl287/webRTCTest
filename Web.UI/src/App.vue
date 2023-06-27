<template>
  <main>
    <RouterView />
  </main>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { RouterView } from 'vue-router'
import connection from '@/hubs/chat'

onMounted(() => connection.start())

</script>

<style>
main {
  height: 100%;
}
</style>