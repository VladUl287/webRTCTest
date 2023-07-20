import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/chat',
      name: 'chat',
      component: () => import('@/views/MainView.vue')
    },
    {
      path: '/callback',
      name: 'callback',
      component: () => import('@/views/CallbackView.vue')
    },
    {
      path: '/logout',
      name: 'logout',
      component: () => import('@/views/LogoutView.vue')
    },
    {
      path: '/:catchAll(.*)',
      redirect: {
        path: '/chat'
      }
    }
  ]
})

export default router
