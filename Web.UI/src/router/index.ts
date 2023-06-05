import { createRouter, createWebHistory } from 'vue-router'
import MainView from '@/views/MainView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/chat',
      name: 'chat',
      component: MainView
    },
    {
      path: '/account',
      name: 'account',
      component: () => import('@/views/AccountView.vue')
    }
  ]
})

export default router
