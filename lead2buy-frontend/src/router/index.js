import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'dashboard',
      // MODIFICADO: Importação dinâmica
      component: () => import('../views/DashboardView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/contacts',
      name: 'contacts',
      // MODIFICADO: Importação dinâmica
      component: () => import('../views/ContactsView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/login',
      name: 'login',
      // MODIFICADO: Importação dinâmica
      component: () => import('../views/LoginView.vue')
    },
    {
      path: '/register',
      name: 'register',
      // MODIFICADO: Importação dinâmica
      component: () => import('../views/RegisterView.vue')
    },
    {
      path: '/contact/:id',
      name: 'contact-detail',
      // MODIFICADO: Importação dinâmica
      component: () => import('../views/ContactDetailView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/funil',
      name: 'funil',
      // MODIFICADO: Importação dinâmica
      component: () => import('../views/FunilView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/network',
      name: 'network',
      // MANTIDO: Já estava com importação dinâmica
      component: () => import('../views/NetworkView.vue'),
      meta: { requiresAuth: true }
    },
  ]
})

// Nosso "Segurança" - Nenhuma alteração necessária aqui
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()

  if (!authStore.token) {
    authStore.checkAuth()
  }

  const isAuthenticated = authStore.isAuthenticated

  if (to.meta.requiresAuth && !isAuthenticated) {
    next({ name: 'login' })
  }
  else if ((to.name === 'login' || to.name === 'register') && isAuthenticated) {
    next({ name: 'contacts' })
  }
  else {
    next()
  }
})

export default router
