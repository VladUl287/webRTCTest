<template>
  <main>
    <RouterView />
  </main>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { RouterView } from 'vue-router'
import { useAuthStore } from './stores/auth'
import HubClient from './hubs/chat'

onMounted(async () => {
  const authStore = useAuthStore()
  const user = await authStore.getUser()
  const access_token = user?.access_token

  if (!access_token) {
    return authStore.login()
  }

  HubClient.build(access_token)
  HubClient.Instance?.connect()
})
</script>

<style>
main {
  height: 100%;
}
</style>