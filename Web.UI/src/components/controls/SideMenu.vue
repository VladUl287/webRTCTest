<template>
    <div class="side-menu" :class="{ 'active': active }">
        <section class="controls">
            <slot name="controls"></slot>
        </section>
        <slot name="content"></slot>
    </div>
</template>

<script setup lang="ts">
defineProps({
    active: Boolean
})
</script>

<style scoped>
.side-menu {
    inset: 0;
    /* display: none; */
    position: absolute;
    transform: translateX(-100%);
    transition: transform 150ms linear;
    background-color: var(--color-background)
}

.side-menu.active {
    /* display: block; */
    transform: unset;
}

.side-menu .controls {
    width: fit-content;
    position: absolute;
    right: .5em;
    top: 10px;
}
</style>