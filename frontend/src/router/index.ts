import { createRouter, createWebHistory } from 'vue-router'
import Links from '@/views/Links.vue'
import LinkPerformance from '@/views/LinkPerformance.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'Links',
      component: Links
    },
    {
      path: '/link/:id',
      name: 'LinkPerformance',
      component: LinkPerformance,
      props: true
    },
    {
      path: '/:pathMatch(.*)*',
      redirect: '/'
    }
  ]
})

export default router
