<template>
  <section class="chats-main">
    <ChatsList :chats="chatsStore.chats" :loading="false" @select="chatSelect" />
    <div class="messages-main" v-if="chatId">
      <!-- <ChatHead :chat=""/> -->
      <div class="messges-header"></div>
      <MessagesList :messages="messagesStore.messages" :loading="false" />
      <MessageNew :disabled="false" @send="sendMessage" />
    </div>
  </section>
</template>

<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import { useChatsStore } from '@/stores/chats';
import ChatsList from '@/components/ChatsList.vue'
import MessagesList from '@/components/MessagesList.vue'
import MessageNew from '@/components/MessageNew.vue'
import { useMessagesStore } from '@/stores/messages';
import { useHubsStore } from '@/stores/hubs';
import { useRoute, useRouter } from 'vue-router';

const chatId = ref<string>()

const chatsStore = useChatsStore()
const messagesStore = useMessagesStore()

// chatsStore.getChats()

const route = useRoute()
const router = useRouter()

onMounted(() => {
  let message = messagesStore.messages[0]

  for (let i = 0; i < 100; i++) {
    messagesStore.messages.push(message)
  }

  const chatIdParam = route.query.id
  chatId.value = chatIdParam as string
  // chatId.value && messagesStore.getMessages(chatId.value)
})

watch(
  () => route.params,
  () => {
    const chatIdParam = route.query.id
    chatId.value = chatIdParam as string
    chatId.value && messagesStore.getMessages(chatId.value)
  }
)

const hubsStore = useHubsStore()
hubsStore.connection.start();

const sendMessage = (content: string) => {
  hubsStore.connection.send('sendMessage', { chatId, content })
}

hubsStore.connection.on('receiveMessage', (message: any) => {
  messagesStore.messages.push(message)
})

const chatSelect = (chat: any) => {
  router.push({ path: '/chat', query: { id: chat } })
}

</script>

<style scoped>
.chats-main {
  height: 100%;
  display: grid;
  grid-template-columns: auto 1fr;
}

.messages-main {
  display: grid;
  overflow-y: hidden;
  grid-template-rows: 1fr 11fr auto;
}
</style>