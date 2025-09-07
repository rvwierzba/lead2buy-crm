import axios from 'axios';
import { useAuthStore } from '../stores/authStore';

const apiClient = axios.create({
    // Aponte diretamente para o IP do seu servidor aqui como fallback
    baseURL: 'https://crm.rvwtech.com.br/api',
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


export default {
    // --- Métodos de Autenticação ---
    register(userData) {
        return apiClient.post('/auth/register', userData);
    },
    login(credentials) {
        return apiClient.post('/auth/login', credentials);
    },

    // --- Métodos para Contatos ---
    getContacts(search = '', status = '') {
        return apiClient.get('/contacts', { params: { search, status } });
    },
    getContactById(id) {
        return apiClient.get(`/contacts/${id}`);
    },
    createContact(contactData) {
        return apiClient.post('/contacts', contactData);
    },
    updateContact(id, contactData) {
        return apiClient.put(`/contacts/${id}`, contactData);
    },
    deleteContact(id) {
        return apiClient.delete(`/contacts/${id}`);
    },
    exportContacts() {
        return apiClient.get('/contacts/export', { responseType: 'blob' });
    },
    importContacts(file) {
        const formData = new FormData();
        formData.append('file', file);
        return apiClient.post('/contacts/import', formData, { headers: { 'Content-Type': 'multipart/form-data' } });
    },
    downloadImportTemplate() {
        return apiClient.get('/contacts/import-template', { responseType: 'blob' });
    },

    // --- Métodos para Tarefas ---
    getTasksForContact(contactId) {
        return apiClient.get('/tasks');
    },
    createTask(taskData) {
        return apiClient.post('/tasks', taskData);
    },
    updateTask(id, taskData) {
        return apiClient.put(`/tasks/${id}`, taskData);
    },
    deleteTask(id) {
        return apiClient.delete(`/tasks/${id}`);
    },

    // --- Métodos para Dashboard ---
    getDashboardStats() {
        return apiClient.get('/dashboard/statistics');
    },
    getLeadsOverTime() {
        return apiClient.get('/dashboard/leads-over-time');
    },
    getPerformanceBySource() {
    return apiClient.get('/dashboard/performance-by-source');
    },

    // --- Métodos para Chatbot ---
    converseWithChatbot(prompt) {
        return apiClient.post('/chatbot/converse', { prompt });
    },
    converseWithAttachment(prompt, file) {
        const formData = new FormData();
        formData.append('prompt', prompt);
        formData.append('file', file);
        return apiClient.post('/chatbot/converse-with-attachment', formData, { headers: { 'Content-Type': 'multipart/form-data' } });
    }
};
