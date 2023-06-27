<template>
    <section class="img-wrap">
        <img :src="src" @error.once="error" alt="Avatar" />
    </section>
</template>

<script setup lang="ts">
const props = defineProps({
    src: String,
    alter: String
})

const error = (event: Event) => {
    const imgElement = (event.target as HTMLImageElement)

    if (imgElement && props.alter) {
        imgElement.src = props.alter
    }
}
</script>

<style scoped>
.img-wrap {
    width: 50px;
    height: 50px;
    user-select: none;
}

.img-wrap img {
    width: 100%;
    height: 100%;
    border-radius: 50%;
}
</style>