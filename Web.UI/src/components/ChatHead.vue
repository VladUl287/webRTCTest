<template>
    <div class="chat-head">
        <img :src="chat.image" width="100" height="100" />
        <p class="chat-name">{{ chat.name }}</p>
        <div class="chat-functions">

        </div>
    </div>
</template>
  
<script setup lang="ts">
import type { PropType } from 'vue';
import type { Chat } from '@/types/chat';

defineProps({
    chat: {
        type: Object as PropType<Chat>,
        required: true
    }
})

</script>
  
<style></style>