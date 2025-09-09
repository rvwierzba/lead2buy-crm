import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import ContactsView from '../views/ContactsView.vue'
import ContactDetailView from '../views/ContactDetailView.vue';
import DashboardView from '../views/DashboardView.vue';
import FunilView from '../views/FunilView.vue';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
      {
        path: '/',
        name: 'dashboard',
        component: DashboardView,
        meta: { requiresAuth: true }
      },
      {
        path: '/contacts',
        name: 'contacts',
        component: ContactsView,
        meta: { requiresAuth: true }
      },
      {
        path: '/login',
        name: 'login',
        component: LoginView
      },
      {
        path: '/register',
        name: 'register',
        component: RegisterView
      },
    {
      path: '/contact/:id', // O ':' indica que 'id' é um parâmetro dinâmico
      name: 'contact-detail',
      component: ContactDetailView,
      meta: { requiresAuth: true }
    },
    {
      path: '/funil',
      name: 'funil',
      component: FunilView,
      meta: { requiresAuth: true }
    },
    {
      path: '/network',
      name: 'network',
      component: () => import('../views/NetworkView.vue'),
      meta: { requiresAuth: true }
    },
  ]
})

// Nosso "Segurança" - VERSÃO CORRIGIDA
router.beforeEach((to, from, next) => {
  // A chamada para useAuthStore() foi movida para DENTRO da função.
  // Isso garante que o Pinia já estará instalado quando este código rodar.
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
