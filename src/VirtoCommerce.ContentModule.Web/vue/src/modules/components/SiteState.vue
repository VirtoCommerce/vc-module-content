<template>
  <span class="site-state">
    <span
      class="site-state__dot"
      aria-hidden="true"
      :style="{ background: state.dot }"
    />{{ $t(state.label) }}
  </span>
</template>

<script setup lang="ts">
import { computed } from "vue";
import type { Store } from "../../api_client/virtocommerce.store";
import { siteState } from "../api";

const props = defineProps<{ store?: Store }>();
const state = computed(() => siteState(props.store));
</script>

<style lang="scss">
.site-state {
  display: inline-flex;
  align-items: center;
  gap: 7px;
  flex: none;
  padding-top: 2px;
  font-size: 13px;
  color: var(--neutrals-500);

  &__dot {
    width: 8px;
    height: 8px;
    border-radius: 50%;
  }
}
</style>
