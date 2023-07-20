<template>
  <section class="chats-main h-100">
    <section class="chats-list-wrap">
      <SideMenu :active="menuActive">
        <template #toggle>
          <button class="base-btn" @click="toggleMenu">
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
          <button v-if="searchActive" class="base-btn" @click="disableSearch">
            <span class="material-symbols-outlined">arrow_back</span>
          </button>
          <button v-else class="base-btn" @click="toggleMenu">
            <span class="material-symbols-outlined">more_vert</span>
          </button>
        </div>

        <SearchField v-model="searchText" @input="inputSearch" @focusin="enableSearch" />
      </section>

      <section class="chats-list">
        <SearchList v-if="searchActive" :items="searchStore.items" :loading="searchStore.searching"
          @select="itemSelect" />

        <ChatsList v-else :select="chatId" :chats="chatStore.chats" :loading="chatStore.chatsLoading"
          @select="chatSelect" />
      </section>
    </section>

    <section class="chat-content" v-if="chatId">
      <ChatHead :chat="chatStore.chat" @startCall="startCall" />

      <section class="chat-messages">
        <div v-if="authStore.userId" class="h-100">
          <ChatNotification v-if="callStore.call" @success="showCall" @reject="endCall(callStore.call.id)"
            class="notification" />

          <MessagesList v-if="chatStore.chat" :chat="chatStore.chat" :messages="messageStore.messages"
            :userId="authStore.userId" :loading="messageStore.messagesLoading" @messageCheck="messageCheck" />
        </div>
      </section>

      <MessageNew class="message-new" :chatId="chatId" @sendMessage="createMessage" />
    </section>

  </section>
  <CallView v-if="callStore.call" v-model="callActive" :chatId="callStore.call.id" />
</template>

<script setup lang="ts">
import { defineAsyncComponent, onMounted, ref, watch } from 'vue'
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
import { ChatType } from '@/types/chat'
import { useMessageStore } from '@/stores/messages'
import type { SearchItem } from '@/types/components'
import { useCallStore } from '@/stores/calls'
import { useSearchStore } from '@/stores/search'
import { createMessage, onCreateMessage, onChatCreated, sendStartCall, onStartCall, sendChatCreated, onLeaveCall, onEndCall, onUpdateChat, sendEndCall, sendUpdateChat } from '@/hubs/chat'
import { debounce } from '@/helpers/debounce'

const CallView = defineAsyncComponent(() => import('@/views/CallView.vue'))

const chatId = ref<string>()
const searchText = ref<string>()

const menuActive = ref<boolean>()
const callActive = ref<boolean>()
const searchActive = ref<boolean>()

const callStore = useCallStore()
const authStore = useAuthStore()
const chatStore = useChatStore()
const searchStore = useSearchStore()
const messageStore = useMessageStore()

const route = useRoute()
const router = useRouter()

onMounted(() => {
  chatStore.getChats()
  getChatContent()
})

watch(
  () => route.params,
  () => getChatContent()
)

const getChatContent = () => {
  chatId.value = route.query.id as string

  if (chatId.value) {
    chatStore.getChat(chatId.value)
    callStore.getCall(chatId.value)
    messageStore.getMessages(chatId.value)
  }
}

const messageCheck = (date: string) => {
  chatStore.setLastRead(date)
  sendUpdateChatDebounce(date)
}

const sendUpdateChatDebounce = debounce((date: string) => {
  console.log('date', date);
  // chatId.value && sendUpdateChat({
  //   chatId: chatId.value,
  //   lastRead: date
  // })
}, 300)

const inputSearch = (value: string) => searchStore.loadUsers(value)

const startCall = () => {
  chatId.value && sendStartCall(chatId.value)
}

const endCall = (chatId: string) => sendEndCall(chatId)

const showCall = () => callActive.value = true

onStartCall(({ userId, chatId }) => {
  if (authStore.userId && authStore.userId === userId) {
    callActive.value = true
  }
  chatId && callStore.getCall(chatId)
})

onLeaveCall(({ chatId }) => callStore.getCall(chatId))

onEndCall(callStore.getCall)

const chatSelect = (chatId: string) => router.push({ path: '/chat', query: { id: chatId } })

const itemSelect = async (item: SearchItem) => {
  disableSearch()

  if (item.value) return chatSelect(item.value)

  if (authStore.userId) {
    const chatId = await chatStore.create({
      name: '', image: '',
      type: ChatType.dialog,
      userId: authStore.userId,
      users: [
        { id: authStore.userId },
        { id: +item.key }
      ]
    })

    if (chatId) {
      sendChatCreated(chatId)
      chatSelect(chatId)
    }
  }
}

onCreateMessage((message) => {
  chatId.value === message.chatId && messageStore.addMessage(message)

  chatStore.getChat(message.chatId)
})

onChatCreated(chatStore.getChat)
onUpdateChat(chatStore.getChat)

const enableSearch = () => searchActive.value = true
const disableSearch = () => {
  searchStore.setItems([])
  searchActive.value = false
  searchText.value = ''
}

const toggleMenu = () => menuActive.value = !menuActive.value
</script>

<style scoped>
.chats-main {
  display: grid;
  grid-template-columns: auto 1fr
}

.chats-list-wrap {
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

.chats-header {
  display: flex;
  align-items: center;
  padding: var(--section-gap);
  column-gap: var(--section-gap);
}

.chats-list {
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

.message-new {
  width: 50%;
  margin: 0 auto;
  padding: var(--section-gap) 0;
}
</style>