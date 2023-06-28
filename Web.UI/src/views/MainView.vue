<template>
  <section class="chats-main">
    <section class="chats-wrap">
      <SideMenu :active="menuActive">
        <template #control>
          <button class="chat-control" @click="toggleMenu">
            <span class="material-symbols-outlined">more_vert</span>
          </button>
        </template>
        <template #content>
          <AccountCard :user="authStore.user" />
          <AccountControls @logout="authStore.logout" />
        </template>
      </SideMenu>

      <section class="chats-header">
        <div>
          <button v-if="searchActive" class="chat-control" @click="disableSearch">
            <span class="material-symbols-outlined">arrow_back</span>
          </button>
          <button v-else class="chat-control" @click="toggleMenu">
            <span class="material-symbols-outlined">more_vert</span>
          </button>
        </div>

        <SearchField :active="searchActive" @input="inputSearch" @focusin="enableSearch" />
      </section>

      <section>
        <SearchList v-if="searchActive" :items="searchItems" :loading="itemsLoading" @select="itemSelect" />

        <ChatsList v-else :select="chatId" :chats="chatStore.chats" :loading="chatsLoading" @select="chatSelect" />
      </section>
    </section>

    <section class="chat-content" v-if="chatId">
      <ChatHead :chat="chat" @start-call="startCall" />

      <div class="chat-messages" v-if="authStore.user">
        <ChatNotification @success="joinCall" class="notification" />

        <MessagesList :chat="chat" :messages="messageStore.messages" :userId="authStore.user.profile.sub"
          :loading="messagesLoading" @lastReaded="setChatLastRead" />
      </div>

      <MessageNew :disabled="false" @send="sendMessage" />
    </section>

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
import AccountCard from '@/components/AccountCard.vue'
import SearchList from '@/components/SearchList.vue'
import CallModal from '@/components/CallModal.vue'
import ChatHead from '@/components/ChatHead.vue'
import MessageNew from '@/components/MessageNew.vue'
import SideMenu from '@/components/controls/SideMenu.vue'
import ChatNotification from '@/components/ChatNotification.vue'
import AccountControls from '@/components/AccountControls.vue'
import { useAuthStore } from '@/stores/auth'
import { useChatStore } from '@/stores/chats'
import { getUsers } from '@/http/users'
import type { Chat, Message } from '@/types/chat'
import { useMessageStore } from '@/stores/messages'
import type { SearchItem } from '@/types/components'
import peer from '@/peer'
import { debounce } from '@/helpers/debounce'
import connection, { callSendMessage, onSendMessage, onChatCreated, onJoinCall, onCalling, onChatUpdate } from '@/hubs/chat'

const chat = ref<Chat>()
const chatId = ref<string>()
// const user = ref<User | null>()
const searchItems = ref<SearchItem[]>([])

const calling = ref<boolean>()
const itemsLoading = ref<boolean>()
const searchActive = ref<boolean>()
const menuActive = ref<boolean>()

const chatsLoading = ref<boolean>()
const messagesLoading = ref<boolean>()

const authStore = useAuthStore()
const chatStore = useChatStore()
const messageStore = useMessageStore()

const route = useRoute()
const router = useRouter()

onMounted(() => {
  initializeChat()
  getChats()
})

watch(
  () => route.params,
  () => initializeChat()
)

const initializeChat = () => {
  chatId.value = route.query.id as string

  getMessages()
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

const getChat = async () => {
  try {
    if (!chatId.value) return

    chat.value = await chatStore.getChat(chatId.value)
  } catch {
    router.replace({ path: '/chat' })
  }
}

const setChatLastRead = debounce((date: string) => {
  connection.send('updateChat', { chatId: chatId.value, lastRead: date[0] })
}, 500)

const inputSearch = async (value: string) => {
  if (!value || value.length < 3) searchItems.value = []

  try {
    itemsLoading.value = true

    const users = await getUsers(value)

    searchItems.value = users.map(user => ({
      key: user.id,
      value: user.chatId,
      label: user.userName
    }))
  } finally {
    itemsLoading.value = false
  }
}

const startCall = async () => {
  await connection.invoke('calling', {
    chatId: chatId.value,
    peerUserId: peer.id
  })

  calling.value = true
}

const sendMessage = (content: string) => {
  return chatId.value && callSendMessage({
    chatId: chatId.value,
    content
  })
}

const joinCall = async () => {
  const camera_stream = await navigator.mediaDevices.getUserMedia({ video: true })
  appendVideo(camera_stream)

  return connection.send('joinCall', {
    chatId: chatId.value,
    peerUserId: peer.id
  })
}

const chatSelect = (chatId: string) => router.push({ path: '/chat', query: { id: chatId } })

const itemSelect = async (item: SearchItem) => {
  searchActive.value = false
  searchItems.value = []

  if (item.value) {
    return router.push({ path: '/chat', query: { id: item.value } })
  }

  const chatId = await chatStore.create({
    name: authStore.user?.profile.name || 'test name', image: '', userId: +authStore.user!.profile.sub, type: 1, users: [
      { id: authStore.user!.profile.sub },
      { id: item.key }
    ]
  })

  connection.send('chatCreated', chatId)

  router.push({ path: '/chat', query: { id: chatId } })
}

peer.on('call', async (call) => {
  const camera_stream = await navigator.mediaDevices.getUserMedia({ video: true })

  call.answer(camera_stream)
  call.on('stream', appendVideo)
})

onCalling((chatId: string) => { })

onJoinCall(async (peerId: string) => {
  if (peerId === peer.id) return

  const camera_stream = await navigator.mediaDevices.getUserMedia({ video: true })
  const call = peer.call(peerId, camera_stream)
  call.on('stream', appendVideo)
})

onSendMessage((message: Message) => {
  chatId.value && messageStore.addMessage(message)

  chatStore.getChat(message.chatId)
})

onChatCreated(async (chatId: string) => {
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

const enableSearch = () => searchActive.value = true
const disableSearch = () => searchActive.value = false

const toggleMenu = () => menuActive.value = !menuActive.value
</script>

<style scoped>
.chats-main {
  height: 100%;
  display: grid;
  grid-template-columns: auto 1fr
}

.chats-wrap {
  width: 500px;
  max-width: 600px;
  min-width: 400px;
  position: relative;
  background-color: var(--color-background);
  border-right: 1px solid var(--color-border-dark);
  /* resize */
  overflow: hidden;
  resize: horizontal;
}

.chat-control {
  display: flex;
  padding: .6em;
  border-radius: 50%;
  background-color: transparent;
  color: var(--color-placeholder);
  border: 1px solid var(--color-border-dark);
}

.chats-header {
  display: flex;
  padding: .5em;
  column-gap: .5em;
  align-items: center;
}

.chat-content {
  display: grid;
  overflow: hidden;
  grid-template-rows: auto 1fr auto;
}

.chat-messages {
  overflow: hidden;
  position: relative;
}

.notification {
  position: absolute;
  z-index: 1;
}
</style>