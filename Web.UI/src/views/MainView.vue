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
          <ProfileCard :profile="authStore.profile" />
          <ProfileControls @logout="authStore.logout" />
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

        <SearchField v-model="searchText" @input="inputSearch" @focusin="enableSearch" />
      </section>

      <section class="chats-content">
        <SearchList v-if="searchActive" :items="searchItems" :loading="itemsLoading" @select="itemSelect" />

        <ChatsList v-else :select="chatId" :chats="chatStore.chats" :loading="chatsLoading" @select="chatSelect" />
      </section>
    </section>

    <section class="chat-content" v-if="chatId">
      <ChatHead :chat="chat" @start-call="startCall" />

      <div class="chat-messages" v-if="authStore.profile">
        <ChatNotification @success="joinCall" class="notification" />

        <MessagesList :chat="chat" :messages="messageStore.messages" :userId="authStore.profile.sub"
          :loading="messagesLoading" @messageCheck="setChatLastRead" />
      </div>

      <section class="message-new-wrap">
        <MessageNew :disabled="false" @send="sendMessage" />
      </section>
    </section>

  </section>
  <CallModal v-model="calling" @endcall="endCall">
    <section id="videos"></section>
  </CallModal>
</template>

<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import ChatsList from '@/components/ChatsList.vue'
import SearchField from '@/components/SearchField.vue'
import MessagesList from '@/components/MessagesList.vue'
import SearchList from '@/components/SearchList.vue'
import CallModal from '@/components/CallModal.vue'
import ChatHead from '@/components/ChatHead.vue'
import MessageNew from '@/components/MessageNew.vue'
import SideMenu from '@/components/controls/SideMenu.vue'
import ChatNotification from '@/components/ChatNotification.vue'
import ProfileCard from '@/components/ProfileCard.vue'
import ProfileControls from '@/components/ProfileControls.vue'
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
const searchText = ref<string>()
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
  getChat(chatId.value)
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

const getChat = async (chatId: string) => {
  try {
    if (!chatId) return

    chat.value = await chatStore.getChat(chatId)
  } catch {
    router.replace({ path: '/chat' })
  }
}

const setChatLastRead = (date: string) => {
  if (chat.value) {
    // setLastRead(chat.value.lastRead)
  }
}

const setLastRead = debounce((date: string) => {
  connection.send('updateChat', { chatId: chatId.value, lastRead: date[0] })
}, 5000)

const inputSearch = async (value: string) => {
  try {
    if (value && value.length > 2) {
      itemsLoading.value = true

      const users = await getUsers(value)

      searchItems.value = users.map(user => ({
        key: user.id,
        image: user.image,
        value: user.chatId,
        label: user.userName
      }))
    }
    else {
      searchItems.value = []
    }
  }
  finally {
    itemsLoading.value = false
  }
}

const startCall = async () => {
  await connection.invoke('calling', {
    chatId: chatId.value,
    peerUserId: peer.id
  })

  const camera_stream = await navigator.mediaDevices.getUserMedia({ video: true })
  appendVideo(camera_stream, peer.id)

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
  appendVideo(camera_stream, peer.id)

  return connection.send('joinCall', {
    chatId: chatId.value,
    peerUserId: peer.id
  })
}

const chatSelect = (chatId: string) => router.push({ path: '/chat', query: { id: chatId } })

const itemSelect = async (item: SearchItem) => {
  searchActive.value = false
  searchItems.value = []
  clearSearchField()

  if (item.value) {
    return router.push({ path: '/chat', query: { id: item.value } })
  }

  if (authStore.profile) {
    const chatId = await chatStore.create({
      name: authStore.profile.name || 'test name', image: '', userId: +authStore.profile.sub, type: 1, users: [
        { id: authStore.profile.sub },
        { id: item.key }
      ]
    })

    connection.send('chatCreated', chatId)

    router.push({ path: '/chat', query: { id: chatId } })
  }
}

const endCall = () => {
  // peer.removeAllListeners()
  peer.disconnect()
}

peer.on('call', async (call) => {
  const camera_stream = await navigator.mediaDevices.getUserMedia({ video: true })

  call.answer(camera_stream)
  call.on('stream', stream => appendVideo(stream, call.peer))
})

peer.on('disconnected', (event) => {
  console.log('disconnected', event);
  
  removeVideo(event)
})

onCalling((chatId: string) => { })

onJoinCall(async (peerId: string) => {
  // if (peerId === peer.id) return

  calling.value = true

  const camera_stream = await navigator.mediaDevices.getUserMedia({ video: true })
  const call = peer.call(peerId, camera_stream)
  call.on('stream', stream => appendVideo(stream, peerId))
})

onSendMessage((message: Message) => {
  chatId.value && messageStore.addMessage(message)

  chatStore.getChat(message.chatId)
})

onChatCreated(async (chatId: string) => {
  const chat = await chatStore.getChat(chatId)
  chat && chatStore.chats.push(chat)
})

onChatUpdate((chatId: string) => {
  console.log('chat udpated');

  getChat(chatId)
})

const appendVideo = (stream: MediaStream, id: string) => {
  const videos = document.querySelector('#videos')

  if (videos?.querySelector('#' + 'a' + id)) {
    return
  }

  const video = document.createElement('video')
  video.srcObject = stream
  video.autoplay = true
  video.id = 'a' + id

  videos?.appendChild(video)
}

const removeVideo = (id: string) => {
  const videos = document.querySelector('#videos')

  videos?.querySelector('#' + 'a' + id)?.remove()
}

const enableSearch = () => searchActive.value = true
const disableSearch = () => {
  searchActive.value = false
  searchItems.value = []
  clearSearchField()
}

const toggleMenu = () => menuActive.value = !menuActive.value

const clearSearchField = () => searchText.value = ''
</script>

<style>
#videos {
  width: 100%;
  height: 100%;
  display: flex;
  overflow-y: auto;
  column-gap: 1em;
  align-items: center;
  justify-content: center;
}

#videos video {
  padding: 2em 0;
  max-width: 100%;
  max-height: 100%;
  border-radius: .5em;
}

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
  background-color: var(--color-background-dark);
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

.chats-content {
  padding: 0 .5em;
}

.chat-content {
  display: grid;
  overflow: hidden;
  grid-template-rows: minmax(70px, auto) 1fr auto;
}

.chat-messages {
  overflow: hidden;
  position: relative;
}

.notification {
  position: absolute;
  z-index: 1;
}

.message-new-wrap {
  width: 50%;
  margin: 0 auto;
  padding: .5em 0;
}
</style>