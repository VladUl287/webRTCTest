<template>
  <section class="chats-main">
    <div class="chats-wrap">
      <SearchField :active="active" :items="items" :loading="searching" @input="input" @select="itemSelect"
        @focusin="focus" @focusout="focus" />

      <ChatsList v-if="!active" :chats="chatsStore.chats" :loading="chatsLoading" :selected="chatIdentity"
        @select="chatSelect" />
    </div>
    <div class="messages-main" v-if="chatIdentity">
      <ChatHead :chat="chat" />

      <MessagesList v-if="user" :messages="messagesStore.messages" :user="user" :loading="messagesLoading"
        @action="action" />

      <MessageNew :disabled="false" @send="sendMessage" />
    </div>
  </section>
</template>

<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import { useChatsStore } from '@/stores/chats';
import ChatHead from '@/components/ChatHead.vue'
import ChatsList from '@/components/ChatsList.vue'
import SearchField from '@/components/SearchField.vue'
import MessagesList from '@/components/MessagesList.vue'
import MessageNew from '@/components/MessageNew.vue'
import { useMessagesStore } from '@/stores/messages';
import { useHubsStore } from '@/stores/hubs';
import { useRoute, useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import type { User } from 'oidc-client';
import { MessageAction, type Chat } from '@/types/chat';
import { getUsers } from '@/http/users';
import type { SearchItem } from '@/types/components';

const chat = ref<Chat | undefined>()
const user = ref<User | null>()
const chatIdentity = ref<string>()
const chatsLoading = ref<boolean>()
const searching = ref<boolean>()
const messagesLoading = ref<boolean>()
const items = ref<SearchItem[]>([])

const authStore = useAuthStore()
const hubsStore = useHubsStore()
const chatsStore = useChatsStore()
const messagesStore = useMessagesStore()

const active = ref<boolean>()

const route = useRoute()
const router = useRouter()

onMounted(() => {
  setChatId()

  getChats()

  chatsStore.getChat(chatIdentity.value!)
    .then(c => chat.value = c)

  authStore.getUser()
    .then(us => user.value = us)

  getMessages()
})

const focus = () => active.value = !active.value

const setChatId = () => {
  chatIdentity.value = route.query.id as string
}

watch(
  () => route.params,
  () => {
    setChatId()
    getMessages()
    updateChat()

    chatsStore.getChat(chatIdentity.value!)
      .then(c => chat.value = c)
  }
)

const updateChat = () => {
  if (chatIdentity.value) {
    hubsStore.connection.send('updateUserChat', { chatId: chatIdentity.value, lastRead: new Date() })
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
    if (!chatIdentity.value) return

    messagesLoading.value = true

    await messagesStore.getMessages(chatIdentity.value)
  } finally {
    messagesLoading.value = false
  }
}

const input = async (value: string) => {
  if (value && value.length > 2) {
    try {
      searching.value = true

      const users = await getUsers(value)

      items.value = users.map((user) => ({
        key: user.id,
        value: user.chatId,
        label: user.userName
      }))
    } finally {
      searching.value = false
    }
  }
  else {
    items.value = []
  }
}

hubsStore.connection.start();

const sendMessage = (content: string) => {
  if (chatIdentity.value) {
    hubsStore.connection.send('sendMessage', { chatId: chatIdentity.value, content })
  }
}

hubsStore.connection.on('receivedMessage', async (message: any) => {
  messagesStore.messages.push(message)
  await chatsStore.getChat(chatIdentity.value!)
})

hubsStore.connection.on('deletedMessage', async (message: any) => {
  messagesStore.messages = messagesStore.messages.filter(x => x.id != message.id)
  await chatsStore.getChat(message.chatId)
})

hubsStore.connection.on('updatedMessage', async (message: any) => {
  const index = messagesStore.messages.findIndex(x => x.id === message.id)
  if (index > -1) {
    messagesStore.messages[index] = message
    await chatsStore.getChat(message.chatId)
  }
})

hubsStore.connection.on('chatCreated', async (chatId: string) => {
  const chat = await chatsStore.getChat(chatId)

  if (chat) {
    chatsStore.chats.push(chat)
  }
})

hubsStore.connection.on('updatedChat', async (chatId: string) => {
  console.log('updated');

  await chatsStore.getChat(chatId)
})

const chatSelect = (chat: any) => {
  router.push({ path: '/chat', query: { id: chat } })
}

const itemSelect = async (item: SearchItem) => {
  items.value = []

  if (item.value) {
    return router.push({ path: '/chat', query: { id: item.value } })
  }

  const chatId = await chatsStore.create({
    name: item.label, image: '', userId: +user.value!.profile.sub, type: 1, users: [
      { id: +user.value!.profile.sub },
      { id: item.key }
    ]
  })

  hubsStore.connection.send('chatCreated', chatId)

  router.push({ path: '/chat', query: { id: chatId } })
}

const action = (content: any) => {
  if (!chatIdentity.value) return

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
  background-color: var(--color-background);
}

.messages-main {
  display: grid;
  overflow-y: hidden;
  border-left: 1px solid gray;
  grid-template-rows: 1fr 11fr auto;
}
</style>