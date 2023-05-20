<template>
    <section>
        <div v-for="dialog of dialogStore.dialogs" :key="dialog.id">
            <p>{{ dialog.image }}</p>
            <p>{{ dialog.name }}</p>
        </div>
    </section>
</template>
  
<script setup lang="ts">
import { useDialogStore } from '@/stores/dialog';
import { useRoute } from 'vue-router';

const route = useRoute()

const dialog = route.params.dialog as string;

const dialogStore = useDialogStore()

dialogStore.loading(dialog)

</script>
  
<style scoped></style>