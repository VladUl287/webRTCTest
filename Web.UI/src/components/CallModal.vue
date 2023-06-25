<template>
    <dialog ref="dialog">
        <p>Lorem ipsum, dolor sit amet consectetur adipisicing elit!</p>
        <!-- <section id="videos"></section> -->
        <button @click="ok">ok</button>
    </dialog>
</template>

<script setup lang="ts">
import { ref, watch, onMounted, type PropType } from 'vue'
import peer from '@/peer'

const dialog = ref<HTMLDialogElement>()

const props = defineProps({
    modelValue: Boolean
})

const emits = defineEmits<{
    (e: 'update:modelValue', value: Boolean): void
}>()

onMounted(() => {
    peer.on('call', async (call) => {
        const camera_stream = await navigator.mediaDevices.getUserMedia({ video: true })

        call.answer(camera_stream)
        call.on('stream', userVideoStream => {
            const videos: HTMLElement | null = document.querySelector('#videos')

            if (videos) {
                try {
                    const video = document.createElement('video')
                    video.srcObject = userVideoStream

                    videos.appendChild(video)
                } catch (error) {
                    console.log(error);
                }
            }
        })
    })
})

watch(
    () => props.modelValue,
    (dialogVisible) => {
        if (dialogVisible) {
            openModal()
            return dialog.value?.showModal()
        }
        dialog.value?.close()
    }
)

const ok = () => emits('update:modelValue', false)

const openModal = async () => {
    const videos: HTMLElement | null = document.querySelector('#videos')

    if (videos) {
        try {
            const camera_stream = await navigator.mediaDevices.getUserMedia({ video: true })

            const video = document.createElement('video')
            video.setAttribute('autoplay', 'true')
            video.srcObject = camera_stream
            videos.id = 'from'

            videos.appendChild(video)
        } catch (error) {
            console.log(error);
        }
    }
}
</script>

<style scoped>
dialog {
    color: var(--color-text);
    border-color: var(--color-border-light);
    background-color: var(--color-background);
}
</style>