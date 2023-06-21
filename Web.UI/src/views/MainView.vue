<template>
  <section class="chats-main">
    <div class="chats-wrap">
      <SearchField v-model="active" :items="items" :loading="searching" @input="input" @select="itemSelect"
        @focusin="focusin" />

      <ChatsList v-if="!active" :chats="chatsStore.chats" :loading="chatsLoading" :selected="chatIdentity"
        @select="chatSelect" />
    </div>
    <div class="chat-content" v-if="chatIdentity">
      <ChatHead :chat="chat" @start-call="startCall"/>

      <section class="chat-messages">
        <div class="chat-notification">
          <p>Call #1</p>
          <div class="notify-controls">
            <button class="success">
              <span class="material-symbols-outlined">call</span>
            </button>
            <button class="danger">
              <span class="material-symbols-outlined">call_end</span>
            </button>
          </div>
        </div>

        <MessagesList v-if="user" :messages="messagesStore.messages" :user="user" :loading="messagesLoading" />
      </section>


      <MessageNew :disabled="false" @send="sendMessage" />
    </div>
  </section>
</template>

<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import { useChatsStore } from '@/stores/chats'
import ChatHead from '@/components/ChatHead.vue'
import ChatsList from '@/components/ChatsList.vue'
import SearchField from '@/components/SearchField.vue'
import MessagesList from '@/components/MessagesList.vue'
import MessageNew from '@/components/MessageNew.vue'
import { useMessagesStore } from '@/stores/messages'
import { useHubsStore } from '@/stores/hubs'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { getUsers } from '@/http/users'
import type { User } from 'oidc-client'
import type { Chat } from '@/types/chat'
import type { SearchItem } from '@/types/components'
import { useCallsStore } from '@/stores/call'

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

  authStore.getUser()
    .then(us => user.value = us)

  getMessages()

  if (chatIdentity.value) {
    chatsStore.getChat(chatIdentity.value)
      .then(c => chat.value = c)
  }
})

const focusin = () => active.value = true

const setChatId = () => {
  chatIdentity.value = route.query.id as string
}

watch(
  () => route.params,
  () => {
    setChatId()
    getMessages()
    updateChat()

    if (chatIdentity.value) {
      chatsStore.getChat(chatIdentity.value)
        .then(c => chat.value = c)
    }
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

//calls

const callstore = useCallsStore()
const test = async () => {
  try {

    await callstore.connection.start()
  } catch (error) {
    console.log(error);
  }
}

const startCall = () => {
  callstore.connection.send('startCall', { chatId: chatIdentity.value, peerUserId: '06427427-b6ad-4dca-b43f-92bc5c9b0b68' })
}

callstore.connection.on('startingCall', () => {
  hubsStore.connection.send('sendMessage', { chatId: chatIdentity.value, content: 'starting call' })
})

test()
//

// hubsStore.connection.start()

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
  console.log('updated')

  await chatsStore.getChat(chatId)
})

const chatSelect = (chat: any) => {
  router.push({ path: '/chat', query: { id: chat } })
}

const itemSelect = async (item: SearchItem) => {
  active.value = false

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

.chat-content {
  display: grid;
  overflow-y: hidden;
  grid-template-rows: 1fr 12fr auto;
  border-left: 1px solid var(--color-border-dark)
}

.chat-messages {
  position: relative;
}

.chat-notification {
  display: flex;
  user-select: none;
  align-items: center;
  position: absolute;
  width: 100%;
  z-index: 999;
  padding: 0 1em;
  background-color: var(--color-active-light);
}

.chat-notification .notify-controls {
  margin-left: auto;
}

.notify-controls button {
  border: none;
  padding: .5em;
  cursor: pointer;
  margin-right: .5em;
  border-radius: 50%;
  display: inline-flex;
  color: var(--color-text)
}

.success {
  background-color: var(--color-success)
}

.danger {
  background-color: var(--color-danger);
}
</style>