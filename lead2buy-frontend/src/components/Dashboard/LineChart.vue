<template>
  <div class="chart-container">
    <Line v-if="chartData && chartData.labels && chartData.labels.length" :data="chartData" :options="chartOptions" />
    <div v-else class="loading-text">Carregando dados do gráfico...</div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { Line } from 'vue-chartjs';
import { Chart as ChartJS, CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend } from 'chart.js';
import apiService from '@/services/apiService';
import { useTheme } from '@/composables/useTheme';

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend);

const { theme } = useTheme();
const rawData = ref([]);
const loading = ref(true);
const error = ref(null);

const chartData = computed(() => ({
  labels: rawData.value.map(d => d.date),
  datasets: [
    {
      label: 'Novos Leads',
      backgroundColor: 'rgba(59, 130, 246, 0.2)',
      borderColor: 'rgba(59, 130, 246, 1)',
      data: rawData.value.map(d => d.count),
      fill: true,
      tension: 0.4,
    },
  ],
}));

const chartOptions = computed(() => ({
  responsive: true,
  maintainAspectRatio: false,
  scales: {
    y: {
      ticks: { color: theme.value === 'dark' ? '#cbd5e1' : '#64748b' },
      grid: { color: theme.value === 'dark' ? 'rgba(255, 255, 255, 0.1)' : 'rgba(0, 0, 0, 0.1)' }
    },
    x: {
      ticks: { color: theme.value === 'dark' ? '#cbd5e1' : '#64748b' },
      grid: { color: 'transparent' }
    }
  },
  plugins: {
    legend: { labels: { color: theme.value === 'dark' ? '#cbd5e1' : '#64748b' } }
  }
}));

onMounted(async () => {
  try {
    const response = await apiService.getLeadsOverTime();
    rawData.value = response.data;
  } catch (err) {
    console.error(err);
    error.value = "Falha ao carregar dados";
  } finally {
    loading.value = false;
  }
});
</script>

<style scoped>
.chart-container {
  position: relative;
  height: 300px;
}
.loading-text {
  text-align: center;
  color: var(--color-text-mute);
  padding-top: 2rem;
}
</style>
