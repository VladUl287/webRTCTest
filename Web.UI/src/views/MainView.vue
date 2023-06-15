<template>
  <section class="chats-main">
    <ChatsList :chats="chatsStore.chats" :loading="chatsLoading" :selected="chatId" @select="chatSelect" />
    <div class="messages-main" v-if="chatId">
      <!-- <ChatHead :chat=""/> -->
      <div class="messges-header"></div>
      <MessagesList :messages="messagesStore.messages" :user="user" :loading="messagesLoading" @action="action" />
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
import { useAuthStore } from '@/stores/auth';
import type { User } from 'oidc-client';
import { MessageAction } from '@/types/chat';

const chatId = ref<string>()
const user = ref<User | null>()
const chatsLoading = ref<boolean>()
const messagesLoading = ref<boolean>()

const authStore = useAuthStore()
const hubsStore = useHubsStore()
const chatsStore = useChatsStore()
const messagesStore = useMessagesStore()

const route = useRoute()
const router = useRouter()

onMounted(() => {
  setChatId()

  getChats()

  authStore.getUser()
    .then(us => user.value = us)

  getMessages()
})

const setChatId = () => {
  chatId.value = route.query.id as string
}

watch(
  () => route.params,
  () => {
    setChatId()
    getMessages()
    updateChat()
  }
)

const updateChat = () => {
  if (chatId.value) {
    hubsStore.connection.send('updateUserChat', { chatId: chatId.value, lastRead: new Date() })
  }
}

const getChats = async () => {
  try {
    chatsLoading.value = true

    await chatsStore.getChats()
  } finally {
    chatsLoading.value = false
  }
}

const getMessages = async () => {
  try {
    if (!chatId.value) return

    messagesLoading.value = true

    await messagesStore.getMessages(chatId.value)
  } finally {
    messagesLoading.value = false
  }
}

hubsStore.connection.start();

const sendMessage = (content: string) => {
  if (chatId.value) {
    hubsStore.connection.send('sendMessage', { chatId: chatId.value, content })
  }
}

hubsStore.connection.on('receivedMessage', async (message: any) => {
  messagesStore.messages.push(message)
  await chatsStore.getChat(chatId.value!)
})

hubsStore.connection.on('deletedMessage', async (message: any) => {
  messagesStore.messages = messagesStore.messages.filter(x => x.id != message.id)
})

hubsStore.connection.on('updatedMessage', async (message: any) => {
  const index = messagesStore.messages.findIndex(x => x.id === message.id)
  if (index > -1) {
    messagesStore.messages[index] = message
  }
})

hubsStore.connection.on('updatedChat', async (chatId: string) => {
  console.log('updated');

  await chatsStore.getChat(chatId)
})

const chatSelect = (chat: any) => {
  router.push({ path: '/chat', query: { id: chat } })
}

const action = (content: any) => {
  if (!chatId.value) return

  if (content.messageAction === MessageAction.Update) {
    hubsStore.connection.send('updateMessage', { id: content.message.id, content: content.message.content })
  }

  if (content.messageAction === MessageAction.Delete) {
    hubsStore.connection.send('deleteMessage', content.message.id)
  }
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