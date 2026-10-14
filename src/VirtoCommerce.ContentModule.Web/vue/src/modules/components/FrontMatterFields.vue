<template>
  <div class="content-metadata">
    <template
      v-for="prop in definitions"
      :key="prop.id"
    >
      <VcSwitch
        v-if="prop.valueType === 'Boolean'"
        :model-value="!!modelValue[prop.name!]"
        :label="prop.name"
        @update:model-value="(v?: boolean) => set(prop.name!, !!v)"
      />
      <VcInput
        v-else-if="prop.valueType === 'DateTime'"
        type="date"
        :model-value="dateValue(modelValue[prop.name!])"
        :label="prop.name"
        :hint="prop.description"
        @update:model-value="(v: unknown) => set(prop.name!, toDateString(v))"
      />
      <VcInput
        v-else
        :model-value="textValue(modelValue[prop.name!])"
        :label="prop.name"
        :hint="prop.isArray ? $t('CONTENT.PAGES.PAGE.LIST_HINT') : prop.description"
        @update:model-value="(v: unknown) => set(prop.name!, fromText(v, prop.isArray))"
      />
    </template>
    <!-- front matter keys the platform has no definition for stay editable as text -->
    <VcInput
      v-for="key in extraKeys"
      :key="key"
      :model-value="textValue(modelValue[key])"
      :label="key"
      @update:model-value="(v: unknown) => set(key, fromText(v, Array.isArray(modelValue[key])))"
    />
  </div>
</template>

<script setup lang="ts">
// The page/blog front matter, edited through the platform's FrontMatterHeaders dynamic property
// definitions (what the AngularJS editors render with va-generic-value-input).
import { DynamicPropertiesClient, useApiClient } from "@vc-shell/framework";
import { VcInput, VcSwitch } from "@vc-shell/framework/ui";
import { computed, ref } from "vue";
import type { FrontMatter } from "../api";

const props = defineProps<{ modelValue: FrontMatter }>();
const emit = defineEmits<{ "update:modelValue": [value: FrontMatter] }>();

const definitions = ref<{ id?: string; name?: string; valueType?: string; isArray?: boolean; description?: string }[]>([]);
const extraKeys = computed(() => Object.keys(props.modelValue).filter((k) => !definitions.value.some((p) => p.name === k)));

useApiClient(DynamicPropertiesClient)
  .getApiClient()
  .then((client) =>
    client.searchDynamicProperties({ objectType: "VirtoCommerce.ContentModule.Core.Model.FrontMatterHeaders", take: 200 }),
  )
  .then((result) => {
    definitions.value = (result.results ?? []).sort((a, b) => (a.displayOrder ?? 0) - (b.displayOrder ?? 0));
  });

const textValue = (v: unknown) => (Array.isArray(v) ? v.join(", ") : v == null ? "" : String(v));
const fromText = (v: unknown, isArray?: boolean) => {
  const text = v == null ? "" : String(v);
  return isArray ? text.split(",").map((x) => x.trim()).filter(Boolean) : text || undefined;
};
const dateValue = (v: unknown) => (v ? String(v).slice(0, 10) : undefined);
const toDateString = (v: unknown) => {
  if (!v) return undefined;
  const d = v instanceof Date ? v : new Date(String(v));
  if (isNaN(d.getTime())) return undefined;
  const pad = (n: number) => String(n).padStart(2, "0");
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`;
};

function set(key: string, value: unknown) {
  emit("update:modelValue", { ...props.modelValue, [key]: value });
}
</script>

<style lang="scss">
.content-metadata {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(240px, 1fr));
  gap: 16px;
  padding: 16px;
}
</style>
