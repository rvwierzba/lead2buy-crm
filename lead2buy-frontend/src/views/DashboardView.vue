<template>
  <div class="dashboard-page">
    <header class="page-header">
      <h1>Dashboard</h1>
    </header>

    <div v-if="loading" class="loading-message">Carregando estatísticas...</div>

    <div v-if="stats" class="stats-grid">
      <StatCard title="Total de Leads" :value="stats.totalLeads" />
      <StatCard title="Oportunidades" :value="stats.opportunities" />
      <StatCard title="Leads Convertidos" :value="stats.convertedLeads" />
      <StatCard title="Taxa de Conversão" :value="`${stats.conversionRate}%`" />
    </div>

    <div class="charts-grid">
      <div class="chart-container">
        <h3>Novos Leads (Últimos 30 dias)</h3>
        <div class="chart-wrapper">
          <LineChart v-if="leadsChartData.datasets[0].data.length > 0" :chartData="leadsChartData" :chartOptions="chartOptions" />
          <div v-else class="no-data-message">
            Sem dados para exibir no período.
          </div>
        </div>
      </div>

      <div class="chart-container">
        <h3>Performance por Origem</h3>
        <div class="chart-wrapper">
          <PieChart v-if="performanceChartData.datasets[0].data.length > 0" :chartData="performanceChartData" :chartOptions="{ responsive: true, maintainAspectRatio: false }" />
          <div v-else class="no-data-message">
            Adicione contatos com uma origem para ver os dados.
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import apiService from '../services/apiService';
import StatCard from '../components/Dashboard/StatCard.vue';
import LineChart from '../components/Dashboard/LineChart.vue';
import PieChart from '../components/Dashboard/PieChart.vue';

const stats = ref(null);
const loading = ref(true);

const leadsChartData = ref({
  labels: [],
  datasets: [{
    label: 'Novos Leads',
    backgroundColor: 'rgba(74, 71, 163, 0.2)',
    borderColor: '#4A47A3',
    tension: 0.4,
    data: []
  }]
});

const performanceChartData = ref({
    labels: [],
    datasets: [{
        backgroundColor: ['#4A47A3', '#28A745', '#FFC107', '#17A2B8', '#6C757D'],
        data: []
    }]
});

const chartOptions = ref({
  responsive: true,
  maintainAspectRatio: false,
  scales: {
    y: {
      beginAtZero: true,
      ticks: {
        stepSize: 1
      }
    }
  }
});

onMounted(async () => {
  try {
    const [statsResponse, leadsOverTimeResponse, performanceResponse] = await Promise.all([
      apiService.getDashboardStats(),
      apiService.getLeadsOverTime(),
      apiService.getPerformanceBySource()
    ]);

    stats.value = statsResponse.data;

    const leadsData = leadsOverTimeResponse.data;
    if (leadsData) {
      leadsChartData.value.labels = leadsData.map(d => new Date(d.date).toLocaleDateString('pt-BR', {timeZone: 'UTC'}));
      leadsChartData.value.datasets[0].data = leadsData.map(d => d.count);
    }

    const performanceData = performanceResponse.data;
    if (performanceData) {
      performanceChartData.value.labels = performanceData.map(d => d.source);
      performanceChartData.value.datasets[0].data = performanceData.map(d => d.count);
    }

  } catch (error) {
    console.error("Erro ao buscar dados do dashboard:", error);
  } finally {
    loading.value = false;
  }
});
</script>

<style scoped>
.dashboard-page {
  padding: 2rem 3rem;
}
.page-header h1 {
  color: var(--text-color);
  font-size: 28px;
  margin-bottom: 2rem;
}
.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 1.5rem;
  margin-bottom: 3rem;
}
.charts-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
    gap: 2rem;
}
.chart-container {
  background-color: var(--ui-bg);
  border: 1px solid var(--ui-border);
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
}
.chart-container h3 {
  margin-top: 0;
  color: var(--text-color);
}
.chart-wrapper {
  height: 350px;
}
.loading-message, .no-data-message {
  height: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
  color: var(--text-color-muted);
  min-height: 100px;
}
</style>
