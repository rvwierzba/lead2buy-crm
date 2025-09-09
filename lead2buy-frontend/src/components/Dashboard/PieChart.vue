<template>
  <div class="chart-container">
    <Pie v-if="chartData.labels && chartData.labels.length" :data="chartData" :options="chartOptions" />
    <div v-else class="loading-text">Carregando dados do gráfico...</div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed, watch } from 'vue';
import { Pie } from 'vue-chartjs';
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from 'chart.js';
import apiService from '@/services/apiService';
import { useTheme } from '@/composables/useTheme';

ChartJS.register(ArcElement, Tooltip, Legend);

const { theme } = useTheme();
const rawData = ref([]);
const chartData = ref({ labels: [], datasets: [] });

const chartOptions = computed(() => ({
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'top',
      labels: { color: theme.value === 'dark' ? '#cbd5e1' : '#64748b' }
    }
  }
}));

const updateChartData = () => {
  chartData.value = {
    labels: rawData.value.map(d => d.source),
    datasets: [
      {
        backgroundColor: ['#42A5F5', '#66BB6A', '#FFA726', '#26A69A', '#AB47BC', '#78909C'],
        data: rawData.value.map(d => d.count),
      },
    ],
  };
};

onMounted(async () => {
  try {
    const response = await apiService.getPerformanceBySource();
    rawData.value = response.data;
    updateChartData();
  } catch (err) {
    console.error("Erro ao carregar dados do gráfico de pizza:", err);
  }
});

watch(theme, updateChartData);
</script>

<style scoped>
.chart-container {
  position: relative; height: 300px;
  display: flex; justify-content: center; align-items: center;
}
.loading-text { color: var(--color-text-mute); }
</style>
