<template>
  <section class="chats-main h-100">
    <section class="chats-list">
      <SideMenu :active="menuActive">
        <template #toggle>
          <button class="chat-control" @click="toggleMenu">
            <span class="material-symbols-outlined">more_vert</span>
          </button>
        </template>
        <template #content>
          <ProfileCard />
          <ProfileControls />
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
      <ChatHead :chat="chat" @startCall="startCall" />

      <div class="chat-messages" v-if="userId">
        <ChatNotification v-if="call" @success="accept" @reject="endCall(call.chatId)" class="notification" />

        <MessagesList :chat="chat" :messages="messageStore.messages" :userId="userId" :loading="messagesLoading"
          @messageCheck="setChatLastRead" />
      </div>

      <section class="message-new-wrap">
        <MessageNew @send="send" />
      </section>
    </section>

  </section>
  <CallView v-if="call" v-model="callActive" :chatId="call.chatId" />
</template>

<script setup lang="ts">
import { computed, defineAsyncComponent, onMounted, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import ChatsList from '@/components/ChatsList.vue'
import SearchField from '@/components/SearchField.vue'
import MessagesList from '@/components/MessagesList.vue'
import SearchList from '@/components/SearchList.vue'
import ChatHead from '@/components/ChatHead.vue'
import MessageNew from '@/components/MessageNew.vue'
import SideMenu from '@/components/controls/SideMenu.vue'
import ProfileCard from '@/components/ProfileCard.vue'
import ProfileControls from '@/components/ProfileControls.vue'
import ChatNotification from '@/components/ChatNotification.vue'
import { useAuthStore } from '@/stores/auth'
import { useChatStore } from '@/stores/chats'
import { getUsers } from '@/http/users'
import { ChatType, type Chat } from '@/types/chat'
import { useMessageStore } from '@/stores/messages'
import type { SearchItem } from '@/types/components'
import { useCallStore } from '@/stores/calls'
import { sendMessage, onSendMessage, onChatCreated, sendStartCall, onStartCall, sendChatCreated, onLeaveCall, onEndCall, updateChat, onUpdateChat, sendEndCall } from '@/hubs/chat'

const CallView = defineAsyncComponent(() => import('@/views/CallView.vue'))

const chat = ref<Chat>()
const call = ref<any>()
const chatId = ref<string>()
const searchText = ref<string>()
const searchItems = ref<SearchItem[]>([])

const menuActive = ref<boolean>()
const callActive = ref<boolean>()
const itemsLoading = ref<boolean>()
const searchActive = ref<boolean>()

const chatsLoading = ref<boolean>()
const messagesLoading = ref<boolean>()

const callStore = useCallStore()
const authStore = useAuthStore()
const chatStore = useChatStore()
const messageStore = useMessageStore()

const route = useRoute()
const router = useRouter()

onMounted(() => {
  getChatsList()
  initChatInfo()
})

watch(
  () => route.params,
  () => initChatInfo()
)

const userId = computed(() => {
  if (authStore.profile) {
    return +authStore.profile.sub
  }
  return undefined
})

const initChatInfo = () => {
  chatId.value = route.query.id as string

  getChat(chatId.value)
  getCall(chatId.value)
  getMessages()
}

const getChatsList = async () => {
  try {
    chatsLoading.value = true

    await chatStore.getChats()
  } finally {
    chatsLoading.value = false
  }
}

const getChat = async (chatId: string) => {
  try {
    if (chatId) {
      chat.value = await chatStore.getChat(chatId)
    }
  } catch {
    router.replace({ path: '/chat' })
  }
}

const getCall = async (chatId: string) => {
  if (chatId) {
    call.value = await callStore.getCall(chatId)
  }
}

const getMessages = async () => {
  try {
    if (chatId.value) {
      messagesLoading.value = true

      await messageStore.getMessages(chatId.value)
    }
  } finally {
    messagesLoading.value = false
  }
}

const setChatLastRead = (date: string) => {
  // chatId.value && updateChat({
  //   chatId: chatId.value,
  //   lastRead: new Date(date)
  // })
}

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

const send = (content: string) => {
  return chatId.value && sendMessage({
    chatId: chatId.value,
    content
  })
}

const startCall = () => {
  if (chatId.value && userId.value) {
    sendStartCall({ chatId: chatId.value, userId: userId.value })
  }
}

const endCall = (chatId: string) => sendEndCall(chatId)

const accept = () => callActive.value = true

onStartCall(({ userId: user, chatId }) => {
  if (user && user === userId.value) {
    callActive.value = true
  }
  chatId && getCall(chatId)
})

onLeaveCall(({ chatId }) => getCall(chatId))

onEndCall(getCall)

const chatSelect = (chatId: string) => router.push({ path: '/chat', query: { id: chatId } })

const itemSelect = async (item: SearchItem) => {
  disableSearch()

  if (item.value) return chatSelect(item.value)

  if (authStore.profile) {
    const chatId = await chatStore.create({
      name: '', image: '',
      type: ChatType.dialog,
      userId: authStore.profile.sub,
      users: [
        { id: authStore.profile.sub },
        { id: item.key }
      ]
    })

    if (chatId) {
      sendChatCreated(chatId)
      chatSelect(chatId)
    }
  }
}

onSendMessage((message) => {
  chatId.value && messageStore.addMessage(message)

  chatStore.getChat(message.chatId)
})

onChatCreated(getChat)
onUpdateChat(getChat)

const enableSearch = () => searchActive.value = true
const disableSearch = () => {
  searchActive.value = false
  searchItems.value = []
  searchText.value = ''
}

const toggleMenu = () => menuActive.value = !menuActive.value
</script>

<style scoped>
.chats-main {
  display: grid;
  grid-template-columns: auto 1fr
}

.chats-list {
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
  align-items: center;
  padding: var(--section-gap);
  column-gap: var(--section-gap);
}

.chats-content {
  padding: 0 var(--section-gap);
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
  padding: var(--section-gap) 0;
}
</style>