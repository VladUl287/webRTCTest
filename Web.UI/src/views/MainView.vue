<template>
  <section class="chats-main">
    <div class="chats-wrap">
      <SearchField v-model="searchActive" :items="searchItems" :loading="searching" @input="input" @select="itemSelect"
        @focusin="searchFocusIn" />

      <ChatsList v-if="!searchActive" :chats="chatStore.chats" :selected="chatId" :loading="chatsLoading"
        @select="chatSelect" />
    </div>
    <div class="chat-content" v-if="chatId">
      <ChatHead :chat="chat" @start-call="startCall" />

      <section class="chat-messages">
        <ChatNotification @success="joinCall" />

        <MessagesList v-if="user" :messages="messageStore.messages" :userId="user.profile.sub"
          :loading="messagesLoading" />
      </section>

      <MessageNew :disabled="false" @send="sendMessage" />
    </div>
  </section>
  <CallModal v-model="calling">
    <section id="videos"></section>
  </CallModal>
</template>

<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import ChatsList from '@/components/ChatsList.vue'
import SearchField from '@/components/SearchField.vue'
import MessagesList from '@/components/MessagesList.vue'
import CallModal from '@/components/CallModal.vue'
import ChatHead from '@/components/ChatHead.vue'
import MessageNew from '@/components/MessageNew.vue'
import ChatNotification from '@/components/ChatNotification.vue'
import { useAuthStore } from '@/stores/auth'
import { useChatStore } from '@/stores/chats'
import { useMessageStore } from '@/stores/messages'
import { getUsers } from '@/http/users'
import type { User } from 'oidc-client'
import type { Chat, Message } from '@/types/chat'
import type { SearchItem } from '@/types/components'
import peer from '@/peer'
import connection, { onSendMessage, onChatCreate, onJoinCall, onCalling, onChatUpdate } from '@/hubs/chat'
import { debounce } from '@/helpers/debounce'


const chat = ref<Chat>()
const chatId = ref<string>()
const user = ref<User | null>()
const searchItems = ref<SearchItem[]>([])

const calling = ref<boolean>()
const searching = ref<boolean>()
const searchActive = ref<boolean>()

const chatsLoading = ref<boolean>()
const messagesLoading = ref<boolean>()

const authStore = useAuthStore()
const chatStore = useChatStore()
const messageStore = useMessageStore()

const route = useRoute()
const router = useRouter()

onMounted(() => {
  // const observer = new MutationObserver((mutations) => {
  //   mutations.forEach(() => {
  //     console.log('style change');
  //   })
  // })

  // const target = document.querySelector('.chats-wrap');
  // observer.observe(target!, { attributes: true, attributeFilter: ['style'] });

  initialize()

  authStore.getUser()
    .then(userValue => user.value = userValue)
})

watch(
  () => route.params,
  () => initialize()
)

const initialize = () => {
  chatId.value = route.query.id as string

  getMessages()
  getChats()
  getChat()
}

const getMessages = async () => {
  try {
    if (!chatId.value) return

    messagesLoading.value = true

    await messageStore.getMessages(chatId.value)
  } finally {
    messagesLoading.value = false
  }
}

const getChats = async () => {
  try {
    chatsLoading.value = true

    await chatStore.getChats()
  } finally {
    chatsLoading.value = false
  }
}

const getChat = () => {
  if (!chatId.value) return

  chatStore.getChat(chatId.value)
    .then(chatValue => chat.value = chatValue)
}

const searchFocusIn = () => searchActive.value = true

const setChatLastRead = debounce(() => {
  connection.send('updateUserChat', { chatId: chatId.value, lastRead: new Date() })
}, 500)

const input = async (value: string) => {
  if (value && value.length > 2) {
    try {
      searching.value = true

      const users = await getUsers(value)

      searchItems.value = users.map((user) => ({
        key: user.id,
        value: user.chatId,
        label: user.userName
      }))
    } finally {
      searching.value = false
    }
  }
  else {
    searchItems.value = []
  }
}

const startCall = async () => {
  // sendMessage('starting call')

  await connection.invoke('calling', {
    chatId: chatId.value,
    peerUserId: peer.id
  })

  calling.value = true
}

const sendMessage = async (content: string) => {
  if (!chatId.value) return

  return connection.send('sendMessage', { chatId: chatId.value, content })
}

const joinCall = async () => {
  const camera_stream = await navigator.mediaDevices.getUserMedia({ video: true })

  appendVideo(camera_stream)

  return connection.send('joinCall', {
    chatId: chatId.value,
    peerUserId: peer.id
  })
}

peer.on('call', async (call) => {
  const camera_stream = await navigator.mediaDevices.getUserMedia({ video: true })

  call.answer(camera_stream)
  call.on('stream', appendVideo)
})

onCalling((chatId: string) => {
  //get call
})

onJoinCall(async (peerId: string) => {
  if (peerId === peer.id) return

  const camera_stream = await navigator.mediaDevices.getUserMedia({ video: true })

  const call = peer.call(peerId, camera_stream)
  call.on('stream', appendVideo)
})

onSendMessage((message: Message) => {
  if (!chatId.value) return

  messageStore.addMessage(message)
  chatStore.getChat(chatId.value)
})

onChatCreate(async (chatId: string) => {
  const chat = await chatStore.getChat(chatId)
  chat && chatStore.chats.push(chat)
})

onChatUpdate((chatId: string) => chatStore.getChat(chatId))

const appendVideo = (stream: MediaStream) => {
  const video = document.createElement('video')
  video.srcObject = stream
  video.autoplay = true

  const videos = document.querySelector('#videos')

  videos?.appendChild(video)
}

const chatSelect = (chatId: string) => {
  router.push({ path: '/chat', query: { id: chatId } })
}

const itemSelect = async (item: SearchItem) => {
  searchActive.value = false

  searchItems.value = []

  if (item.value) {
    return router.push({ path: '/chat', query: { id: item.value } })
  }

  const chatId = await chatStore.create({
    name: '', image: '', userId: +user.value!.profile.sub, type: 1, users: [
      { id: user.value!.profile.sub },
      { id: item.key }
    ]
  })

  connection.send('chatCreated', chatId)

  router.push({ path: '/chat', query: { id: chatId } })
}

// const startCall = async () => {
//   call.value = true
//   chatConnection.send('Calling', { chatId: chatId.value, peerUserId: 'ed7594b7-6998-4d82-a552-4a09c2916307' })
//   sendMessage('starting call')
// }
// const sendMessage = (content: string) => {
//   if (chatId.value) {
//     chatConnection.send('sendMessage', { chatId: chatId.value, content })
//   }
// }
// const joinCall = () => {
//   console.log('start join call', {
//     chatId: chatId.value,
//     peerUserId: peer.id
//   })
//   return chatConnection.send('joinCall', {
//     chatId: chatId.value,
//     peerUserId: peer.id
//   })
// }
// peer.on('call', async (call) => {
//   console.log('call');
//   const camera_stream = await navigator.mediaDevices.getUserMedia({ video: true })
//   call.answer(camera_stream)
//   call.on('stream', userVideoStream => {
//     const video: HTMLVideoElement | null = document.querySelector('#to')
//     if (video) {
//       video.srcObject = userVideoStream
//     }
//   })
// })
// chatConnection.on('Calling', async (chatId: any) => {
//   console.log(chatId);
// })
// chatConnection.on('JoinCall', async (peerId: any) => {
//   const camera_stream = await navigator.mediaDevices.getUserMedia({ video: true })
//   const video: HTMLVideoElement | null = document.querySelector('#from')
//   video!.srcObject = camera_stream
//   const call = peer.call(peerId, camera_stream)
//   call.on('stream', userVideoStream => {
//     const video: HTMLVideoElement | null = document.querySelector('#to')
//     if (video) {
//       video.srcObject = userVideoStream
//     }
//   })
// })
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
  overflow: hidden;
  resize: horizontal;
  background-color: var(--color-background);
  border-right: 1px solid var(--color-border-dark);
}

.chat-content {
  display: grid;
  overflow-y: hidden;
  grid-template-rows: 1fr 12fr auto;
}

.chat-messages {
  overflow: hidden;
  position: relative;
}
</style>