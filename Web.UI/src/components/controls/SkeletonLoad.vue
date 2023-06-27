<template>
    <div class="main">
        <div class="circle"></div>
        <section class="group">
            <div class="line short"></div>
            <div class="line"></div>
        </section>
    </div>
</template>

<style scoped>
.main {
    width: 100%;
    height: 100%;
    display: flex;
    padding: 0 .5em;
    column-gap: .5em;
    align-items: center;
    border-radius: var(--border-radius);
    border: 1px solid var(--color-border-dark);

    --border-radius: 5px;
}

.circle {
    min-width: 45px;
    min-height: 45px;
    border-radius: 50%;
    background-color: var(--color-border-dark);
}

.group {
    width: 100%;
}

.line {
    height: .5em;
    border-radius: var(--border-radius);
    background-color: var(--color-border-dark);
}

.line:not(:last-child) {
    margin-bottom: .7em;
}

.short {
    width: 60%;
}
</style>