import axios from 'axios';
import { useAuthStore } from '../stores/authStore';

const apiClient = axios.create({
  baseURL: '/',
  headers: {
    'Content-Type': 'application/json'
  }
});

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
  register(userData) {
    return apiClient.post('/api/auth/register', userData);
  },
  login(credentials) {
    return apiClient.post('/api/auth/login', credentials);
  },
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
  getTasksForContact(contactId) {
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
  getDashboardStats() {
    return apiClient.get('/api/dashboard/statistics');
  },
  getLeadsOverTime() {
    return apiClient.get('/api/dashboard/leads-over-time');
  },
  getPerformanceBySource() {
    return apiClient.get('/api/dashboard/performance-by-source');
  },
  converseWithChatbot(prompt) {
    return apiClient.post('/api/chatbot/converse', { prompt });
  },
  converseWithAttachment(prompt, file, userId) {
    const formData = new FormData();
    formData.append('prompt', prompt);
    formData.append('file', file);
    formData.append('userId', userId);
    return apiClient.post('/api/chatbot/converse-with-attachment', formData, { headers: { 'Content-Type': 'multipart/form-data' } });
  },
   getRecentContacts() {
    return apiClient.get('/api/dashboard/recent-contacts');
  },
  getUpcomingTasks() {
    return apiClient.get('/api/dashboard/upcoming-tasks');
  },
  // --- Métodos para Network ---
  getNetworkContacts() {
    return apiClient.get('/api/network');
  },
  createNetworkContact(contactData) {
    return apiClient.post('/api/network', contactData);
  },
  updateNetworkContact(id, contactData) {
    return apiClient.put(`/api/network/${id}`, contactData);
  },
  deleteNetworkContact(id) {
    return apiClient.delete(`/api/network/${id}`);
  }
};
