<template>
  <div class="page-container">
    <header class="page-header">
      <h1 class="page-title">Dashboard</h1>
      <p class="page-subtitle">Visão geral do seu funil de vendas e atividades.</p>
    </header>

    <div v-if="loadingStats" class="loading-text">Carregando métricas...</div>
    <div v-else-if="statsError" class="error-text">{{ statsError }}</div>
    <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
      <StatCard title="Total de Leads" :value="stats.totalLeads" :icon="UsersIcon" />
      <StatCard title="Novos (Mês)" :value="stats.newContactsThisMonth" :icon="UserPlusIcon" />
      <StatCard title="Oportunidades" :value="stats.opportunities" :icon="CurrencyDollarIcon" />
      <StatCard title="Tarefas Pendentes" :value="stats.pendingTasks" :icon="ClipboardDocumentListIcon" />
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-5 gap-6 mb-8">
      <div class="lg:col-span-3 card">
        <h3 class="card-header">Aquisição de Leads (Últimos 6 meses)</h3>
        <LineChart />
      </div>
      <div class="lg:col-span-2 card">
        <h3 class="card-header">Origem dos Leads</h3>
        <PieChart />
      </div>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <RecentContacts />
      <UpcomingTasks />
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import apiService from '@/services/apiService';
import StatCard from '@/components/Dashboard/StatCard.vue';
import LineChart from '@/components/Dashboard/LineChart.vue';
import PieChart from '@/components/Dashboard/PieChart.vue';
import RecentContacts from '@/components/Dashboard/RecentContacts.vue';
import UpcomingTasks from '@/components/Dashboard/UpcomingTasks.vue';
import { UsersIcon, UserPlusIcon, CurrencyDollarIcon, ClipboardDocumentListIcon } from '@heroicons/vue/24/outline';

const stats = ref({
  totalLeads: 0,
  newContactsThisMonth: 0,
  opportunities: 0,
  pendingTasks: 0,
});
const loadingStats = ref(true);
const statsError = ref(null);

const fetchDashboardStats = async () => {
  try {
    const response = await apiService.getDashboardStats();
    stats.value = response.data;
  } catch (err) {
    console.error("Erro ao buscar estatísticas do dashboard:", err);
    statsError.value = "Não foi possível carregar as estatísticas.";
  } finally {
    loadingStats.value = false;
  }
};

onMounted(() => {
  fetchDashboardStats();
});
</script>

<style scoped>
.page-container {
  padding: 2rem;
  background-color: var(--color-background);
  min-height: 100vh;
}
.page-header {
  margin-bottom: 2rem;
}
.page-title {
  font-size: 2.25rem;
  font-weight: 700;
  color: var(--color-heading);
}
.page-subtitle {
  color: var(--color-text-mute);
  margin-top: 0.25rem;
}
.loading-text, .error-text {
  text-align: center;
  color: var(--color-text-mute);
  padding: 2rem;
}
.error-text {
  color: #ef4444;
}
.card {
  background-color: var(--color-background-soft);
  border: 1px solid var(--color-border);
  border-radius: 12px;
  padding: 1.5rem;
}
.card-header {
  font-size: 1.125rem;
  font-weight: 600;
  color: var(--color-heading);
  margin-bottom: 1rem;
}
</style>
