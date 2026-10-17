<template>
  <div class="site-list">
    <button
      v-for="row in rows"
      :key="row.id"
      type="button"
      class="site-list__row"
      :class="{ 'site-list__row--active': row.id === activeId }"
      @click="open(row.id)"
    >
      <span
        class="site-list__icon"
        aria-hidden="true"
      >
        <VcIcon
          :icon="row.icon"
          size="s"
        />
      </span>
      <span class="site-list__text">
        <span class="site-list__name">{{ row.title }}</span>
        <span class="site-list__detail">{{ row.detail }}</span>
      </span>
      <span class="site-list__count">{{ row.count }}</span>
      <VcIcon
        class="site-list__chevron"
        icon="lucide-chevron-right"
        size="s"
      />
    </button>
  </div>
</template>

<script setup lang="ts">
// The site's files (a widget on its page, or its content without the Page Builder): themes and file pages.
import { useBlade } from "@vc-shell/framework";
import { VcIcon } from "@vc-shell/framework/ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import type { ContentStatistic } from "../../api_client/virtocommerce.content";
import type { Store } from "../../api_client/virtocommerce.store";
import { useContentClients } from "../api";

const props = defineProps<{ storeId: string; store?: Store }>();

type RowId = "themes" | "pages";

const { t } = useI18n({ useScope: "global" });
// the site page's blade: what opens here is its child
const { openBlade } = useBlade();
const clients = useContentClients();

const stats = ref<ContentStatistic>();
const activeId = ref<RowId>();

// counts the server doesn't know (-1) are left out rather than shown as 0
const count = (value?: number) => (value === undefined || value < 0 ? "" : value);

const rows = computed(() => [
  {
    id: "themes" as const,
    icon: "lucide-palette",
    title: t("CONTENT.TYPES.THEMES"),
    detail: stats.value?.activeThemeName
      ? t("CONTENT.SITE_TAB.THEMES_DETAIL", { name: stats.value.activeThemeName })
      : t("CONTENT.SITE_TAB.NO_ACTIVE_THEME"),
    count: count(stats.value?.themesCount),
  },
  { id: "pages" as const, icon: "lucide-file-text", title: t("CONTENT.TYPES.PAGES"), detail: t("CONTENT.SITE_TAB.PAGES_DETAIL"), count: count(stats.value?.pagesCount) },
]);

async function load() {
  stats.value = await (await clients.content()).getStoreContentStats(props.storeId).catch(() => undefined);
}
// the site page re-reads its store after a change in a child blade: the counts follow
watch(() => props.store, load, { immediate: true });

// the rows are content types, not the child's param (the store): highlighted once its blade opened
function open(id: RowId) {
  const onOpen = () => (activeId.value = id);
  const onClose = () => (activeId.value = undefined);
  const base = { param: props.storeId, onOpen, onClose };
  if (id === "themes") openBlade({ ...base, name: "ContentThemes", options: { store: props.store } });
  else openBlade({ ...base, name: "ContentFiles", options: { contentType: id, store: props.store } });
}
</script>

<style lang="scss">
/* a site tab's list: rows in one rounded card (the Files tab, and the look other modules' tabs follow) */
.site-list {
  border: 1px solid var(--neutrals-200);
  border-radius: 18px;
  overflow: hidden;
  background: var(--additional-50);

  &__row {
    width: 100%;
    display: flex;
    align-items: center;
    gap: 16px;
    padding: 15px 20px;
    border: 0;
    background: none;
    color: var(--neutrals-900);
    font: inherit;
    text-align: left;
    cursor: pointer;

    & + & {
      border-top: 1px solid var(--neutrals-200);
    }

    &:hover,
    &--active {
      background: var(--neutrals-50);
    }
  }

  &__icon {
    width: 32px;
    height: 32px;
    flex: none;
    border-radius: 9px;
    background: var(--neutrals-100);
    color: var(--neutrals-500);
    display: flex;
    align-items: center;
    justify-content: center;
  }

  &__text {
    flex: 1;
    min-width: 0;
    display: flex;
    flex-direction: column;
  }

  &__name {
    font-weight: 600;
  }

  &__detail,
  &__count {
    font-size: 13px;
    color: var(--neutrals-500);
  }

  &__chevron {
    color: var(--neutrals-500);
    flex: none;
  }
}
</style>
