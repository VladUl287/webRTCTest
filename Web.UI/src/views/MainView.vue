<template>
  <section class="chats-main">
    <div class="chats-wrap">
      <SearchField v-model="active" :items="items" :loading="searching" @input="input" @select="itemSelect"
        @focusin="focusin" />

      <ChatsList v-if="!active" :chats="chatsStore.chats" :selected="chatId" :loading="chatsLoading"
        @select="chatSelect" />
    </div>
    <div class="chat-content" v-if="chatId">
      <ChatHead :chat="chat" @start-call="startCall" />

      <section class="chat-messages">
        <div class="chat-notification">
          <p>Call #1</p>
          <div class="notify-controls">
            <button class="success" @click="joinCall">
              <span class="material-symbols-outlined">call</span>
            </button>
            <button class="danger">
              <span class="material-symbols-outlined">call_end</span>
            </button>
          </div>
        </div>

        <MessagesList v-if="user" :messages="messagesStore.messages" :userId="user.profile.sub"
          :loading="messagesLoading" />
      </section>


      <MessageNew :disabled="false" @send="sendMessage" />
    </div>
  </section>
  <CallModal v-model="call" />
</template>

<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import { useChatsStore } from '@/stores/chats'
import ChatHead from '@/components/ChatHead.vue'
import ChatsList from '@/components/ChatsList.vue'
import SearchField from '@/components/SearchField.vue'
import MessagesList from '@/components/MessagesList.vue'
import CallModal from '@/components/CallModal.vue'
import MessageNew from '@/components/MessageNew.vue'
import { useMessagesStore } from '@/stores/messages'
import { useHubsStore } from '@/stores/hubs'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { getUsers } from '@/http/users'
import type { User } from 'oidc-client'
import type { Chat } from '@/types/chat'
import type { SearchItem } from '@/types/components'
import peer from '@/peer'
import chatHub from '@/hubs/chat'

// const user = await useAuthStore().getUser()

// const access_token = user?.access_token

const chat = ref<Chat>()
const user = ref<User | null>()

const chatId = ref<string>()
const chatsLoading = ref<boolean>()
const searching = ref<boolean>()
const messagesLoading = ref<boolean>()
const items = ref<SearchItem[]>([])

const authStore = useAuthStore()
const hubsStore = useHubsStore()
const chatsStore = useChatsStore()
const messagesStore = useMessagesStore()

const active = ref<boolean>()
const call = ref<boolean>(false)

const route = useRoute()
const router = useRouter()

onMounted(() => {
  setChatId()

  getChats()

  authStore.getUser()
    .then(us => user.value = us)

  getMessages()

  if (chatId.value) {
    chatsStore.getChat(chatId.value)
      .then(c => chat.value = c)
  }
})

const focusin = () => active.value = true

const setChatId = () => {
  chatId.value = route.query.id as string
}

watch(
  () => route.params,
  () => {
    setChatId()
    getMessages()
    updateChat()

    if (chatId.value) {
      chatsStore.getChat(chatId.value)
        .then(c => chat.value = c)
    }
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

const startCall = async () => {
  call.value = true

  // hubsStore.connection.send('startCall', { chatId: chatIdentity.value, peerUserId: 'ed7594b7-6998-4d82-a552-4a09c2916307' })

  // sendMessage('starting call')
}

hubsStore.connection.start()

const sendMessage = (content: string) => {
  if (chatId.value) {
    hubsStore.connection.send('sendMessage', { chatId: chatId.value, content })
  }
}

const joinCall = () => {
  return hubsStore.connection.send('joinCall', {
    chatId: chatId.value,
    peerUserId: 'ed7594b7-6998-4d82-a552-4a09c2916307'
  })
}

hubsStore.connection.on('StartingCall', async (chatId: any) => {
  console.log(chatId);
})

hubsStore.connection.on('JoinedCall', async (peerId: any) => {
  const camera_stream = await navigator.mediaDevices.getUserMedia({ video: true })

  peer.call(peerId, camera_stream)
})

hubsStore.connection.on('LeaveCall', async (peerId: any) => {
  console.log(peerId);
})

hubsStore.connection.on('receivedMessage', async (message: any) => {
  messagesStore.messages.push(message)
  await chatsStore.getChat(chatId.value!)
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