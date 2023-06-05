<template>
  <main>
    <RouterView />
  </main>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { RouterView } from 'vue-router'
import { useHubsStore } from './stores/hubs'

onMounted(async () => {
  // const chatHubStore = useHubsStore()

  // chatHubStore.connect();
})
</script>

<style>
main {
  height: 100%;
}
</style>