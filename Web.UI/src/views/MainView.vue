<template>
  <section class="chats-main">
    <div class="chats-wrap">
      <SearchField :items="searchItems" :active="searchActive" @search="search" />
      <ChatsList v-if="!searchActive" :chats="chatsStore.chats" :loading="chatsLoading" :selected="chat"
        @select="chatSelect" />
    </div>
    <div class="messages-main" v-if="chat">
      <!-- <ChatHead :chat=""/> -->
      <div class="messges-header"></div>
      <MessagesList :messages="messagesStore.messages" :user="user" :loading="messagesLoading" @action="action" />
      <MessageNew :disabled="false" @send="sendMessage" />
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { useChatsStore } from '@/stores/chats';
import ChatsList from '@/components/ChatsList.vue'
import SearchField from '@/components/SearchField.vue'
import MessagesList from '@/components/MessagesList.vue'
import MessageNew from '@/components/MessageNew.vue'
import { useMessagesStore } from '@/stores/messages';
import { useHubsStore } from '@/stores/hubs';
import { useRoute, useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import type { User } from 'oidc-client';
import { MessageAction } from '@/types/chat';
import { getUsers } from '@/http/users';
import type { SearchItem } from '@/types/components';

const chat = ref<string>()
const user = ref<User | null>()
const chatsLoading = ref<boolean>()
const messagesLoading = ref<boolean>()
const searchItems = ref<SearchItem[]>([])

const authStore = useAuthStore()
const hubsStore = useHubsStore()
const chatsStore = useChatsStore()
const messagesStore = useMessagesStore()

const searchActive = computed<boolean>(() => searchItems.value.length > 0)

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
  chat.value = route.query.id as string
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
  if (chat.value) {
    hubsStore.connection.send('updateUserChat', { chatId: chat.value, lastRead: new Date() })
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
    if (!chat.value) return

    messagesLoading.value = true

    await messagesStore.getMessages(chat.value)
  } finally {
    messagesLoading.value = false
  }
}

const search = async (value: string) => {
  if (value && value.length > 2) {
    const users = await getUsers(value)

    searchItems.value = users.map(
      (user) => ({ key: user.id, value: user.userName }))
  }
  else {
    searchItems.value = []
  }
}

// hubsStore.connection.start();

const sendMessage = (content: string) => {
  if (chat.value) {
    hubsStore.connection.send('sendMessage', { chatId: chat.value, content })
  }
}

hubsStore.connection.on('receivedMessage', async (message: any) => {
  messagesStore.messages.push(message)
  await chatsStore.getChat(chat.value!)
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
  if (!chat.value) return

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

.chats-wrap {
  width: 500px;
  max-width: 600px;
  min-width: 400px;
  background-color: #001524;
}

.messages-main {
  display: grid;
  overflow-y: hidden;
  grid-template-rows: 1fr 11fr auto;
}
</style>