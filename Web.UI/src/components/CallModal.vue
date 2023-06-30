<template>
    <dialog ref="dialog">
        <div class="wrap">
            <section id="header">
                <p>Lorem ipsum, dolor sit amet consectetur adipisicing elit!</p>
            </section>
            <slot></slot>
            <section id="footer">
                <button @click="ok">ok</button>
            </section>
        </div>
    </dialog>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'

const dialog = ref<HTMLDialogElement>()

const props = defineProps({
    modelValue: Boolean
})

const emits = defineEmits<{
    (e: 'update:modelValue', value: Boolean): void
}>()

watch(
    () => props.modelValue,
    (visible) => {
        visible ?
            dialog.value?.showModal() :
            dialog.value?.close()
    }
)

const ok = () => emits('update:modelValue', false)
</script>

<style scoped>
dialog {
    width: 90%;
    height: 70%;
    color: var(--color-text);
    border: 1px solid var(--color-border-light);
    background-color: var(--color-background-dark);
}

dialog::backdrop {
    background-color: #000000cc;
}

.wrap {
    height: 100%;
    display: grid;
    grid-template-rows: 1fr 11fr 1fr;
}

#header {
    text-align: center;
}

#footer {
    margin: 0 auto;
    width: fit-content;
}

#footer button {
    cursor: pointer;
}
</style>