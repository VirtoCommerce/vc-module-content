<template>
  <VcBlade
    :title="$t('CONTENT.PAGES.STORES.TITLE')"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="50%"
  >
    <VcDataTable
      v-model:search-value="searchValue"
      :items="rows"
      :total-count="totalCount"
      :pagination="pagination"
      :searchable="true"
      :state-key="'CONTENT_STORES'"
      :active-item-id="activeChildParam"
      :row-actions="rowActions"
      :empty-state="{ icon: 'lucide-store', title: $t('CONTENT.PAGES.STORES.EMPTY') }"
      @row-click="(e: { data: Row }) => openStore(e.data.id!)"
      @pagination-click="pagination.goToPage"
    >
      <VcColumn
        id="name"
        :title="$t('CONTENT.PAGES.STORES.COLUMNS.NAME')"
        body-class="content-list__name"
        always-visible
      />
      <VcColumn
        id="activeThemeName"
        :title="$t('CONTENT.PAGES.STORES.COLUMNS.THEME')"
      />
      <VcColumn
        v-for="col in countColumns"
        :id="col"
        :key="col"
        :title="$t(`CONTENT.TYPES.${col.toUpperCase()}`)"
        align="end"
      >
        <template #body="{ data }"><span class="vc-table-cell-number">{{ count(data[col]) }}</span></template>
      </VcColumn>
    </VcDataTable>
  </VcBlade>
</template>

<script setup lang="ts">
import {
  useAsync,
  useBlade,
  useDataTablePagination,
  useFunctions,
  useLatestRequest,
  useTableSearch,
  type IBladeToolbar,
} from "@vc-shell/framework";
import { VcBlade, VcColumn, VcDataTable } from "@vc-shell/framework/ui";
import { onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import type { Store } from "../../api_client/virtocommerce.store";
import { count, useContentClients } from "../api";

defineBlade({
  url: "/content",
  name: "ContentStores",
  isWorkspace: true,
  permissions: ["content:access"],
  menuItem: { title: "CONTENT.MENU.TITLE", icon: "lucide-code-xml", priority: 20 },
});

type Row = Store & { activeThemeName?: string; themes?: number; pages?: number; blogs?: number; linkLists?: number };

const PAGE_SIZE = 20;
const countColumns = ["themes", "pages", "blogs", "linkLists"] as const;

const { t } = useI18n({ useScope: "global" });
const { openBlade, query, exposeToChildren, activeChildParam } = useBlade();
const { debounce } = useFunctions();
const clients = useContentClients();

const rows = ref<Row[]>([]);
const totalCount = ref(0);
const { searchValue } = useTableSearch({ stateKey: "CONTENT_STORES" });
const pagination = useDataTablePagination({ stateKey: "CONTENT_STORES", pageSize: PAGE_SIZE, totalCount });

// a newer search wins: responses of an older one (list or counters) are dropped
const search = useLatestRequest();
const loading = search.pending;

const { action: load } = useAsync(async () => {
  const request = search.begin();
  try {
    const result = await (await clients.stores()).searchStores({
      keyword: searchValue.value || undefined,
      skip: pagination.skip,
      take: PAGE_SIZE,
      sort: "name",
    });
    if (!request.isCurrent()) return;
    rows.value = result.results ?? [];
    totalCount.value = result.totalCount ?? 0;
  } finally {
    request.complete();
  }
  // per-store counters load after the list, like the AngularJS tiles
  rows.value.forEach((row) => countStore(row.id!, request.isCurrent));
});

/** a store's active theme and content counts */
async function countStore(storeId: string, isCurrent = () => true) {
  const [content, menu] = await Promise.all([clients.content(), clients.menu()]);
  const [stats, lists] = await Promise.all([
    content.getStoreContentStats(storeId).catch(() => undefined),
    menu.getLists(storeId).catch(() => []),
  ]);
  const i = rows.value.findIndex((x) => x.id === storeId);
  if (!isCurrent() || i < 0) return;
  rows.value[i] = {
    ...rows.value[i],
    activeThemeName: stats?.activeThemeName,
    themes: stats?.themesCount,
    pages: stats?.pagesCount,
    blogs: stats?.blogsCount,
    linkLists: lists?.length ?? 0,
  };
}

function openStore(storeId: string) {
  openBlade({ name: "ContentStore", param: storeId });
}

function openStorefront(row: Row, previewTheme = false) {
  if (row.url) {
    window.open(previewTheme ? `${row.url}?previewtheme=${row.activeThemeName}` : row.url, "_blank");
  }
}

const rowActions = (row: Row) => [
  {
    type: "info" as const,
    id: "storefront",
    title: t("CONTENT.PAGES.STORES.ACTIONS.OPEN_STOREFRONT"),
    icon: "lucide-external-link",
    disabled: !row.url,
    clickHandler: () => openStorefront(row),
  },
  {
    type: "info" as const,
    id: "preview",
    title: t("CONTENT.PAGES.STORES.ACTIONS.PREVIEW_THEME"),
    icon: "lucide-eye",
    disabled: !row.url || !row.activeThemeName,
    clickHandler: () => openStorefront(row, true),
  },
];

const bladeToolbar = ref<IBladeToolbar[]>([
  { id: "refresh", title: t("CONTENT.TOOLBAR.REFRESH"), icon: "lucide-refresh-cw", clickHandler: () => load() },
]);

watch(searchValue, () => pagination.setPage(1));
watch([searchValue, () => pagination.skip], debounce(load, 300));

onMounted(async () => {
  await load();
  // /content?storeId=… (the AngularJS workspace link) opens that store straight away
  if (query.value?.storeId) openStore(query.value.storeId);
});

// a change in a store's content blades: only that row's counters
exposeToChildren({ refreshStore: (storeId: string) => countStore(storeId) });
</script>
