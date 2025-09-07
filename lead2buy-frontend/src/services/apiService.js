import axios from 'axios';
import { useAuthStore } from '../stores/authStore';

const apiClient = axios.create({
  // --- A ÚNICA ALTERAÇÃO NECESSÁRIA ESTÁ AQUI ---
  // A baseURL agora é relativa ('/'), pois o frontend e o backend
  // estão no mesmo domínio (https://crm.rvwtech.com.br).
  baseURL: '/',
  // --- FIM DA ALTERAÇÃO ---
  headers: {
    'Content-Type': 'application/json'
  }
});

// Interceptor de REQUISIÇÃO: Adiciona o token antes de enviar
apiClient.interceptors.request.use(config => {
  const authStore = useAuthStore();
  const token = authStore.token;
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
}, error => {
  return Promise.reject(error);
});

// Interceptor de RESPOSTA: Lida com token expirado (erro 401)
apiClient.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    const authStore = useAuthStore();
    if (error.response && error.response.status === 401) {
      console.log("Token expirado ou inválido. Deslogando...");
      authStore.logout();
    }
    return Promise.reject(error);
  }
);

// O restante do seu arquivo, com todos os seus métodos, continua igual
export default {
  // --- Métodos de Autenticação ---
  register(userData) {
    // A chamada agora será para: https://crm.rvwtech.com.br/api/auth/register
    return apiClient.post('/api/auth/register', userData);
  },
  login(credentials) {
    return apiClient.post('/api/auth/login', credentials);
  },

  // --- Métodos para Contatos ---
  getContacts(search = '', status = '') {
    return apiClient.get('/api/contacts', { params: { search, status } });
  },
  getContactById(id) {
    return apiClient.get(`/api/contacts/${id}`);
  },
  createContact(contactData) {
    return apiClient.post('/api/contacts', contactData);
  },
  updateContact(id, contactData) {
    return apiClient.put(`/api/contacts/${id}`, contactData);
  },
  deleteContact(id) {
    return apiClient.delete(`/api/contacts/${id}`);
  },
  exportContacts() {
    return apiClient.get('/api/contacts/export', { responseType: 'blob' });
  },
  importContacts(file) {
    const formData = new FormData();
    formData.append('file', file);
    return apiClient.post('/api/contacts/import', formData, { headers: { 'Content-Type': 'multipart/form-data' } });
  },
  downloadImportTemplate() {
    return apiClient.get('/api/contacts/import-template', { responseType: 'blob' });
  },

  // --- Métodos para Tarefas ---
  getTasksForContact(contactId) {
    // Pequena correção: idealmente, você deve filtrar as tarefas por contato
    return apiClient.get(`/api/tasks/contact/${contactId}`);
  },
  createTask(taskData) {
    return apiClient.post('/api/tasks', taskData);
  },
  updateTask(id, taskData) {
    return apiClient.put(`/api/tasks/${id}`, taskData);
  },
  deleteTask(id) {
    return apiClient.delete(`/api/tasks/${id}`);
  },

  // --- Métodos para Dashboard ---
  getDashboardStats() {
    return apiClient.get('/api/dashboard/statistics');
  },
  getLeadsOverTime() {
    return apiClient.get('/api/dashboard/leads-over-time');
  },
  getPerformanceBySource() {
    return apiClient.get('/api/dashboard/performance-by-source');
  },

  // --- Métodos para Chatbot ---
  converseWithChatbot(prompt) {
    return apiClient.post('/api/chatbot/converse', { prompt });
  },
  converseWithAttachment(prompt, file, userId) {
    const formData = new FormData();
    formData.append('prompt', prompt);
    formData.append('file', file);
    formData.append('userId', userId); // Garante que o userId seja enviado
    return apiClient.post('/api/chatbot/converse-with-attachment', formData, { headers: { 'Content-Type': 'multipart/form-data' } });
  }
};
