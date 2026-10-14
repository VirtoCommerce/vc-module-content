<template>
  <VcBlade
    :title="store?.name ?? param"
    :subtitle="$t('CONTENT.PAGES.STORE.SUBTITLE')"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="40%"
  >
    <VcDataTable
      :items="rows"
      :active-item-id="activeItemId"
      :column-switcher="false"
      @row-click="(e: { data: Row }) => open(e.data.id)"
    >
      <VcColumn
        id="title"
        :title="$t('CONTENT.PAGES.STORE.COLUMNS.TYPE')"
        body-class="content-list__name"
        always-visible
      >
        <template #body="{ data }">
          <span class="content-type">
            <VcIcon
              :icon="data.icon"
              size="s"
            />
            {{ data.title }}
          </span>
        </template>
      </VcColumn>
      <VcColumn
        id="details"
        :title="$t('CONTENT.PAGES.STORE.COLUMNS.DETAILS')"
      />
      <VcColumn
        id="count"
        :title="$t('CONTENT.PAGES.STORE.COLUMNS.COUNT')"
        align="end"
      />
    </VcDataTable>
  </VcBlade>
</template>

<script setup lang="ts">
import { useAsync, useBlade, usePopup, AppsClient, useApiClient, type IBladeToolbar } from "@vc-shell/framework";
import { VcBlade, VcColumn, VcDataTable, VcIcon } from "@vc-shell/framework/ui";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import type { ContentStatistic } from "../../api_client/virtocommerce.content";
import type { Store } from "../../api_client/virtocommerce.store";
import { count, openPageBuilder, storeDefaultTheme, useContentClients } from "../api";

defineBlade({
  url: "/content-store",
  name: "ContentStore",
  permissions: ["content:read"],
});

type Row = { id: "themes" | "pages" | "blogs" | "linkLists"; icon: string; title: string; details?: string; count: string | number };

const { t } = useI18n({ useScope: "global" });
const { param, openBlade, exposeToChildren, callParent } = useBlade();
const { showInfo } = usePopup();
const clients = useContentClients();

const store = ref<Store>();
const stats = ref<ContentStatistic>();
const linkLists = ref<number>();
const activeItemId = ref<string>();
const hasPageBuilder = ref(false);

const rows = computed<Row[]>(() => [
  {
    id: "themes",
    icon: "lucide-palette",
    title: t("CONTENT.TYPES.THEMES"),
    details: stats.value?.activeThemeName
      ? t("CONTENT.PAGES.STORE.ACTIVE_THEME", { name: stats.value.activeThemeName })
      : t("CONTENT.PAGES.STORE.NO_ACTIVE_THEME"),
    count: count(stats.value?.themesCount),
  },
  { id: "pages", icon: "lucide-file-text", title: t("CONTENT.TYPES.PAGES"), count: count(stats.value?.pagesCount) },
  { id: "blogs", icon: "lucide-newspaper", title: t("CONTENT.TYPES.BLOGS"), count: count(stats.value?.blogsCount) },
  { id: "linkLists", icon: "lucide-link", title: t("CONTENT.TYPES.LINKLISTS"), count: count(linkLists.value) },
]);

const { loading, action: load } = useAsync(async () => {
  const [stores, content, menu] = await Promise.all([clients.stores(), clients.content(), clients.menu()]);
  [store.value, stats.value, linkLists.value] = await Promise.all([
    stores.getStoreById(param.value!),
    content.getStoreContentStats(param.value!).catch(() => undefined),
    menu.getLists(param.value!).then((x) => x?.length ?? 0, () => 0),
  ]);
});

// the rows are content types, not the child's param (the store): highlighted once its blade opened
function open(id: Row["id"]) {
  const onOpen = () => (activeItemId.value = id);
  const onClose = () => (activeItemId.value = undefined);
  const storeId = param.value;
  if (id === "themes") openBlade({ name: "ContentThemes", param: storeId, options: { store: store.value }, onOpen, onClose });
  else if (id === "linkLists") openBlade({ name: "ContentLinkLists", param: storeId, options: { store: store.value }, onOpen, onClose });
  else openBlade({ name: "ContentFiles", param: storeId, options: { contentType: id, store: store.value }, onOpen, onClose });
}

function openStorefront() {
  if (store.value?.url) window.open(store.value.url, "_blank");
  else showInfo(t("CONTENT.ALERTS.NO_STORE_URL"));
}

// Page Builder (another module) has its own app; designed pages are edited there
const { getApiClient: getAppsClient } = useApiClient(AppsClient);
getAppsClient()
  .then((client) => client.getApps())
  .then((apps) => (hasPageBuilder.value = apps.some((x) => x.id === "page-builder-shell")))
  .catch(() => undefined);

const bladeToolbar = ref<IBladeToolbar[]>([
  { id: "refresh", title: t("CONTENT.TOOLBAR.REFRESH"), icon: "lucide-refresh-cw", clickHandler: () => load() },
  {
    id: "storefront",
    title: t("CONTENT.PAGES.STORES.ACTIONS.OPEN_STOREFRONT"),
    icon: "lucide-external-link",
    clickHandler: openStorefront,
  },
  {
    id: "pageBuilder",
    title: t("CONTENT.TOOLBAR.PAGE_BUILDER"),
    icon: "lucide-layout-template",
    isVisible: computed(() => hasPageBuilder.value),
    clickHandler: () => openPageBuilder(param.value!),
  },
]);

onMounted(load);

// A child changed this store's content. The parent refreshes only what that touches, so it never re-reads
// itself over unsaved edits: the content workspace's list one row, the Store module's store blade
// (this blade opened from its widget) its widget badges. A parent without the method ignores the call.
exposeToChildren({
  async reload() {
    const theme = storeDefaultTheme(store.value);
    await Promise.all([
      load(),
      callParent("refreshStore", param.value).catch(() => undefined),
      callParent("reloadWidgets").catch(() => undefined),
    ]);
    // the active theme is the store's DefaultThemeName dynamic property: the store blade takes over that part
    if (storeDefaultTheme(store.value) !== theme) await callParent("refreshPart", "dynamicProperties").catch(() => undefined);
  },
});
</script>

<style lang="scss">
.content-type {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  color: var(--neutrals-900);
}
</style>
