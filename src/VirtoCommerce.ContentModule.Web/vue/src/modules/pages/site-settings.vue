<template>
  <VcBlade
    :title="$t('CONTENT.PAGES.SITE_SETTINGS.TITLE')"
    :subtitle="store?.name"
    :loading="loading"
    :modified="form.isModified.value"
    :toolbar-items="toolbar"
    width="30%"
  >
    <div class="site-settings">
      <VcInput
        v-model="item.name"
        :label="$t('CONTENT.PAGES.SITE_SETTINGS.NAME')"
        required
      />
      <VcInput
        v-model="item.url"
        type="url"
        :label="$t('CONTENT.PAGES.SITE_SETTINGS.URL')"
        :placeholder="$t('CONTENT.PAGES.SITE_SETTINGS.URL_PLACEHOLDER')"
      />
      <div
        role="radiogroup"
        class="site-settings__states"
        :aria-label="$t('CONTENT.PAGES.SITE_SETTINGS.STATUS')"
      >
        <span class="site-settings__label">{{ $t("CONTENT.PAGES.SITE_SETTINGS.STATUS") }}</span>
        <div class="site-settings__options">
          <button
            v-for="state in states"
            :key="state"
            type="button"
            role="radio"
            class="site-settings__state"
            :class="{ 'site-settings__state--on': item.storeState === state }"
            :aria-checked="item.storeState === state"
            @click="item.storeState = state"
          >
            {{ $t(`CONTENT.STATES.${state.toUpperCase()}`) }}
          </button>
        </div>
      </div>
      <div
        v-for="(group, i) in locales"
        :key="group.label"
        class="site-settings__row"
        :class="{ 'site-settings__row--divided': i === 0 }"
      >
        <span class="site-settings__label">{{ $t(group.label) }}</span>
        <span class="site-settings__chips">
          <span
            v-for="code in group.codes"
            :key="code"
            class="site-settings__chip"
            :class="{ 'site-settings__chip--default': code === group.default }"
            :title="code === group.default ? $t('CONTENT.PAGES.SITE_SETTINGS.DEFAULT') : undefined"
            >{{ code }}</span
          >
          <span v-if="!group.codes.length">—</span>
        </span>
      </div>
    </div>
  </VcBlade>
</template>

<script setup lang="ts">
import { getBladeConfig, notification, useAsync, useBlade, useBladeForm, type IBladeToolbar } from "@vc-shell/framework";
import { VcBlade, VcInput } from "@vc-shell/framework/ui";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { StoreState, type Store } from "../../api_client/virtocommerce.store";
import { useContentClients } from "../api";

defineBlade({
  name: "SiteSettings",
  permissions: ["store:update"],
});

type Item = Pick<Store, "name" | "url" | "storeState">;

const states = [StoreState.Open, StoreState.RestrictedAccess, StoreState.Closed];

const { t } = useI18n({ useScope: "global" });
const { param, callParent, replaceWith } = useBlade();
const clients = useContentClients();

const store = ref<Store>();
const item = ref<Item>({});
const form = useBladeForm({ data: item, closeConfirmMessage: computed(() => t("CONTENT.ALERTS.CLOSE_CONFIRMATION")) });
// the store module's blade with every setting (it isn't there when the Store module has no UI here)
const hasStoreModule = !!getBladeConfig("StoreDetails");

// the site's languages and currencies, the default one first and marked
const sortDefault = (codes: string[] = [], first?: string) => [...codes].sort((a, b) => Number(b === first) - Number(a === first));
const locales = computed(() => [
  { label: "CONTENT.PAGES.SITE_SETTINGS.LANGUAGES", default: store.value?.defaultLanguage, codes: sortDefault(store.value?.languages, store.value?.defaultLanguage) },
  { label: "CONTENT.PAGES.SITE_SETTINGS.CURRENCIES", default: store.value?.defaultCurrency, codes: sortDefault(store.value?.currencies, store.value?.defaultCurrency) },
]);

const { loading, action: load } = useAsync(async () => {
  store.value = await (await clients.stores()).getStoreById(param.value!);
  item.value = { name: store.value.name, url: store.value.url, storeState: store.value.storeState ?? StoreState.Open };
  form.setBaseline();
});

const { loading: saving, action: save } = useAsync(async () => {
  // the rest of the store goes back as read, so only these three fields change
  await (await clients.stores()).updateStore({ ...store.value!, ...item.value, url: item.value.url || undefined } as Store);
  await load();
  notification.success(t("CONTENT.PAGES.SITE_SETTINGS.SAVED"));
  await callParent("reload").catch(() => undefined);
});

const toolbar = computed<IBladeToolbar[]>(() => [
  {
    id: "save",
    title: t("CONTENT.TOOLBAR.SAVE"),
    icon: "lucide-save",
    disabled: !form.isModified.value || !item.value.name || saving.value,
    clickHandler: () => save(),
  },
  {
    id: "allSettings",
    title: t("CONTENT.PAGES.SITE_SETTINGS.ALL_SETTINGS"),
    icon: "lucide-settings",
    isVisible: hasStoreModule,
    // in place of this panel, next to the site
    clickHandler: () => replaceWith({ name: "StoreDetails", param: param.value }),
  },
]);

onMounted(load);
</script>

<style lang="scss">
.site-settings {
  padding: 20px 22px 36px;
  display: flex;
  flex-direction: column;
  gap: 16px;

  &__label {
    font-size: 13px;
    color: var(--neutrals-500);
  }

  &__states,
  &__row {
    display: flex;
    flex-direction: column;
    gap: 6px;
    color: var(--neutrals-900);
  }

  &__row--divided {
    padding-top: 16px;
    border-top: 1px solid var(--neutrals-200);
  }

  &__chips {
    display: flex;
    flex-wrap: wrap;
    gap: 6px;
  }

  &__chip {
    padding: 3px 10px;
    border-radius: 999px;
    border: 1px solid var(--neutrals-200);
    font-size: 13px;
    color: var(--neutrals-700);

    &--default {
      border-color: var(--primary-500);
      color: var(--primary-500);
      font-weight: 600;
    }
  }

  &__options {
    display: flex;
    gap: 6px;
    flex-wrap: wrap;
  }

  &__state {
    height: 34px;
    padding: 0 14px;
    border-radius: 10px;
    border: 1px solid var(--neutrals-200);
    background: none;
    color: var(--neutrals-900);
    font: inherit;
    font-size: 13px;
    cursor: pointer;

    &--on {
      border-color: var(--primary-500);
      background: color-mix(in srgb, var(--primary-500) 14%, transparent);
      color: var(--primary-500);
      font-weight: 600;
    }
  }
}
</style>
