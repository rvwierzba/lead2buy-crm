import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import Toast from 'vue-toastification' // <-- 1. Importa a biblioteca
import 'vue-toastification/dist/index.css' // <-- 2. Importa o CSS padrão

import App from './App.vue'
import router from './router'

const app = createApp(App)

app.use(createPinia())
app.use(router)
app.use(Toast) // <-- 3. Diz ao Vue para usar o sistema de Toast

app.mount('#app')
