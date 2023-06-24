<template>
  <section class="chats-main">
    <div class="chats-wrap">
      <SearchField v-model="searchActive" :items="items" :loading="searching" @input="input" @select="itemSelect"
        @focusin="searchFocusIn" />

      <ChatsList v-if="!searchActive" :chats="chatsStore.chats" :selected="chatId" :loading="chatsLoading"
        @select="chatSelect" />

      <button @click="joinCall">join call</button>
    </div>
    <div class="chat-content" v-if="chatId">
      <section id="videos">
        <video autoplay id="from"></video>
        <video autoplay id="to"></video>
      </section>
      <!-- <ChatHead :chat="chat" @start-call="startCall" />

      <section class="chat-messages">
        <ChatNotification @success="joinCall" />

        <MessagesList v-if="user" :messages="messagesStore.messages" :userId="user.profile.sub"
          :loading="messagesLoading" />
      </section>

      <MessageNew :disabled="false" @send="sendMessage" /> -->
    </div>
  </section>
  <CallModal v-model="call" />
</template>

<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useChatsStore } from '@/stores/chats'
import ChatHead from '@/components/ChatHead.vue'
import ChatsList from '@/components/ChatsList.vue'
import SearchField from '@/components/SearchField.vue'
import MessagesList from '@/components/MessagesList.vue'
import CallModal from '@/components/CallModal.vue'
import MessageNew from '@/components/MessageNew.vue'
import ChatNotification from '@/components/ChatNotification.vue'
import { useMessagesStore } from '@/stores/messages'
import { useAuthStore } from '@/stores/auth'
import { getUsers } from '@/http/users'
import type { User } from 'oidc-client'
import type { Chat } from '@/types/chat'
import type { SearchItem } from '@/types/components'
import peer from '@/peer'
import chatConnection from '@/hubs/chat'

const chat = ref<Chat>()
const user = ref<User | null>()

const chatId = ref<string>()
const chatsLoading = ref<boolean>()
const searching = ref<boolean>()
const messagesLoading = ref<boolean>()
const items = ref<SearchItem[]>([])

const authStore = useAuthStore()
const chatsStore = useChatsStore()
const messagesStore = useMessagesStore()

const searchActive = ref<boolean>()
const call = ref<boolean>(false)

const route = useRoute()
const router = useRouter()

onMounted(() => {
  initialize()

  authStore.getUser()
    .then(u => user.value = u)
})

watch(
  () => route.params,
  () => {
    initialize()
  }
)

const initialize = () => {
  chatId.value = route.query.id as string

  getMessages()

  getChats()

  if (chatId.value) {
    chatsStore.getChat(chatId.value)
      .then(c => chat.value = c)
  }
}

const searchFocusIn = () => searchActive.value = true

const updateChat = () => {
  if (chatId.value) {
    chatConnection.send('updateUserChat', { chatId: chatId.value, lastRead: new Date() })
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

  chatConnection.send('Calling', { chatId: chatId.value, peerUserId: 'ed7594b7-6998-4d82-a552-4a09c2916307' })

  sendMessage('starting call')
}

const sendMessage = (content: string) => {
  if (chatId.value) {
    chatConnection.send('sendMessage', { chatId: chatId.value, content })
  }
}

const joinCall = () => {
  console.log('start join call', {
    chatId: chatId.value,
    peerUserId: peer.id
  })
  return chatConnection.send('joinCall', {
    chatId: chatId.value,
    peerUserId: peer.id
  })
}

peer.on('call', async (call) => {
  console.log('call');

  const camera_stream = await navigator.mediaDevices.getUserMedia({ video: true })

  call.answer(camera_stream)
  call.on('stream', userVideoStream => {
    const video: HTMLVideoElement | null = document.querySelector('#to')
    if (video) {
      video.srcObject = userVideoStream
    }
    // const videos: HTMLElement | null = document.querySelector('#videos')

    // if (videos) {
    //   try {
    //     const video = document.createElement('video')
    //     console.log('stream', userVideoStream)
    //     video.srcObject = userVideoStream
    //     video.width = 350
    //     video.height = 150

    //     videos.appendChild(video)
    //     video.play()
    //   } catch (error) {
    //     console.log(error);
    //   }
    // }
  })
})

chatConnection.on('Calling', async (chatId: any) => {
  console.log(chatId);
})

chatConnection.on('JoinCall', async (peerId: any) => {

  const camera_stream = await navigator.mediaDevices.getUserMedia({ video: true })

  const video: HTMLVideoElement | null = document.querySelector('#from')

  video!.srcObject = camera_stream

  peer.call(peerId, camera_stream)
})

chatConnection.on('LeaveCall', async (peerId: any) => {
  console.log(peerId);
})

chatConnection.on('receivedMessage', async (message: any) => {
  messagesStore.messages.push(message)
  await chatsStore.getChat(chatId.value!)
})

chatConnection.on('deletedMessage', async (message: any) => {
  messagesStore.messages = messagesStore.messages.filter(x => x.id != message.id)
  await chatsStore.getChat(message.chatId)
})

chatConnection.on('updatedMessage', async (message: any) => {
  const index = messagesStore.messages.findIndex(x => x.id === message.id)
  if (index > -1) {
    messagesStore.messages[index] = message
    await chatsStore.getChat(message.chatId)
  }
})

chatConnection.on('chatCreated', async (chatId: string) => {
  const chat = await chatsStore.getChat(chatId)

  if (chat) {
    chatsStore.chats.push(chat)
  }
})

chatConnection.on('updatedChat', async (chatId: string) => {
  console.log('updated')

  await chatsStore.getChat(chatId)
})

const chatSelect = (chat: any) => {
  router.push({ path: '/chat', query: { id: chat } })
}

const itemSelect = async (item: SearchItem) => {
  searchActive.value = false

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

  chatConnection.send('chatCreated', chatId)

  router.push({ path: '/chat', query: { id: chatId } })
}
</script>

<style scoped>
video {
  width: 350px;
  height: 150px;
}

.chats-main {
  height: 100%;
  display: grid;
  grid-template-columns: auto 1fr;
}

.chats-wrap {
  width: 400px;
  max-width: 600px;
  min-width: 400px;
  background-color: var(--color-background);
}

.chat-content {
  /* display: grid;
  overflow-y: hidden;
  grid-template-rows: 1fr 12fr auto;
  border-left: 1px solid var(--color-border-dark) */
}

.chat-messages {
  position: relative;
}
</style>