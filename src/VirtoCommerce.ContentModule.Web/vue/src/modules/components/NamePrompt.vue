<template>
  <VcPopup
    v-if="prompt"
    :model-value="true"
    :title="prompt.title"
    @update:model-value="close"
  >
    <template #content>
      <form
        class="content-prompt"
        @submit.prevent="confirm"
      >
        <VcInput
          v-model="value"
          :label="prompt.label"
          :error-message="error"
          required
          autofocus
        />
      </form>
    </template>
    <template #footer>
      <div class="content-prompt__footer">
        <VcButton
          variant="secondary"
          @click="close"
        >
          {{ $t("CONTENT.PROMPT.CANCEL") }}
        </VcButton>
        <VcButton
          :disabled="!value.trim() || !!error"
          @click="confirm"
        >
          {{ $t("CONTENT.PROMPT.OK") }}
        </VcButton>
      </div>
    </template>
  </VcPopup>
</template>

<script setup lang="ts">
import { VcButton, VcInput, VcPopup } from "@vc-shell/framework/ui";
import { computed, ref, watch } from "vue";

export interface NamePromptRequest {
  title: string;
  label: string;
  value?: string;
  /** returns an error message for an invalid name */
  validate?: (value: string) => string | undefined;
  resolve: (value: string | undefined) => void;
}

const props = defineProps<{ prompt?: NamePromptRequest }>();
const emit = defineEmits<{ done: [] }>();

const value = ref("");
watch(
  () => props.prompt,
  (p) => (value.value = p?.value ?? ""),
  { immediate: true },
);
const error = computed(() => (value.value ? props.prompt?.validate?.(value.value.trim()) : undefined));

function close() {
  props.prompt?.resolve(undefined);
  emit("done");
}

function confirm() {
  if (!value.value.trim() || error.value) return;
  props.prompt?.resolve(value.value.trim());
  emit("done");
}
</script>

<style lang="scss">
.content-prompt {
  min-width: 360px;
}
.content-prompt__footer {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
  width: 100%;
}
</style>
