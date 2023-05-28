<template>
    <section>
        <div v-for="dialog of chatStore.dialogs" :key="dialog.id">
            <p>{{ dialog.image }}</p>
            <p>{{ dialog.name }}</p>
        </div>
    </section>
</template>
  
<script setup lang="ts">
import { useChatStore } from '@/stores/chat';
import { useRoute } from 'vue-router';

const route = useRoute()

const chat = route.params.dialog as string;

const chatStore = useChatStore()

chatStore.loading(chat)

</script>
  
<style scoped></style>