<template>
  <label class="theme-switch" for="theme-toggle-checkbox">
    <input
      type="checkbox"
      id="theme-toggle-checkbox"
      :checked="modelValue"
      @change="$emit('update:modelValue', $event.target.checked)"
    >
    <div class="slider">
      <div class="slider-indicator">
        <slot></slot>
      </div>
    </div>
  </label>
</template>

<script setup>
defineProps({
  modelValue: {
    type: Boolean,
    required: true
  }
});
defineEmits(['update:modelValue']);
</script>

<style scoped>
.theme-switch {
  display: inline-block;
  position: relative;
  width: 60px;
  height: 34px;
  cursor: pointer;
  -webkit-tap-highlight-color: transparent; /* Remove highlight em mobile */
}

.theme-switch input {
  opacity: 0;
  width: 0;
  height: 0;
}

.slider {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: var(--color-background-mute);
  border-radius: 34px;
  transition: background-color 0.4s;
  display: flex;
  align-items: center;
}

.slider-indicator {
  position: absolute;
  height: 26px;
  width: 26px;
  left: 4px;
  bottom: 4px;
  background-color: white;
  border-radius: 50%;
  transition: transform 0.4s;
  display: flex;
  justify-content: center;
  align-items: center;
  box-shadow: 0 2px 4px rgba(0,0,0,0.2);
}

.slider-indicator i {
  color: #f39c12; /* Cor do sol */
  transition: color 0.4s;
}

input:checked + .slider {
  background-color: var(--primary-color);
}

input:checked + .slider .slider-indicator {
  transform: translateX(26px);
}

/* Cor da lua */
input:checked + .slider .slider-indicator i {
  color: var(--color-background-soft);
}
</style>
