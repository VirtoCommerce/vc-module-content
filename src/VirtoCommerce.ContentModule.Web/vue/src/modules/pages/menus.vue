<template>
  <VcBlade
    :title="$t('CONTENT.MENU.MENUS')"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="50%"
  >
    <div
      v-if="sites.length > 1"
      class="content-site-filter"
    >
      <VcSelect
        v-model="siteId"
        :options="siteOptions"
        option-value="id"
        option-label="name"
        :clearable="false"
        :aria-label="$t('CONTENT.SITES.FILTER')"
      />
    </div>
    <VcDataTable
      :items="rows"
      :active-item-id="activeId"
      :column-switcher="false"
      :empty-state="{ icon: 'lucide-list-tree', title: $t('CONTENT.PAGES.LINK_LISTS.EMPTY') }"
      @row-click="(e: { data: Row }) => open(e.data)"
    >
      <VcColumn
        id="name"
        :title="$t('CONTENT.PAGES.LINK_LISTS.COLUMNS.NAME')"
        body-class="content-list__name"
        always-visible
      />
      <VcColumn
        id="language"
        :title="$t('CONTENT.PAGES.LINK_LISTS.COLUMNS.LANGUAGE')"
      />
      <VcColumn
        id="siteName"
        :title="$t('CONTENT.SITES.COLUMN')"
      />
      <VcColumn
        id="links"
        :title="$t('CONTENT.PAGES.LINK_LISTS.COLUMNS.LINKS')"
        align="end"
      >
        <template #body="{ data }"><span class="vc-table-cell-number">{{ data.links }}</span></template>
      </VcColumn>
    </VcDataTable>
  </VcBlade>
</template>

<script setup lang="ts">
import { useAsync, useBlade, type IBladeToolbar } from "@vc-shell/framework";
import { VcBlade, VcColumn, VcDataTable, VcSelect } from "@vc-shell/framework/ui";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import type { MenuLinkList } from "../../api_client/virtocommerce.content";
import type { Store } from "../../api_client/virtocommerce.store";
import { useContentClients } from "../api";
import { useAllSites } from "../composables/useAllSites";

// every site's menus (link lists) in one list
defineBlade({
  url: "/content-menus",
  name: "ContentMenus",
  isWorkspace: true,
  permissions: ["content:access"],
  menuItem: { title: "CONTENT.MENU.MENUS", icon: "lucide-list-tree", priority: 42 },
});

type Row = { id: string; name?: string; language?: string; links: number; siteName?: string; store: Store; list: MenuLinkList };

const { t } = useI18n({ useScope: "global" });
const { openBlade, exposeToChildren } = useBlade();
const clients = useContentClients();
const { sites, siteId, siteOptions, loadSites, shows, addTo } = useAllSites();

const all = ref<Row[]>([]);
const rows = computed(() => all.value.filter((x) => shows(x.store.id)));
const activeId = ref<string>();

const { loading, action: load } = useAsync(async () => {
  const [stores, menu] = await Promise.all([loadSites(), clients.menu()]);
  const perSite = await Promise.all(
    stores.map(async (store) =>
      ((await menu.getLists(store.id!).catch(() => [])) ?? []).map((list) => ({
        id: list.id!,
        name: list.name,
        language: list.language,
        links: list.menuLinks?.length ?? 0,
        siteName: store.name,
        store,
        list,
      })),
    ),
  );
  all.value = perSite.flat().sort((a, b) => (a.name ?? "").localeCompare(b.name ?? ""));
});

function open(row: Row) {
  openBlade({
    name: "ContentLinkListDetails",
    param: row.store.id,
    options: { listId: row.list.id, store: row.store },
    onOpen: () => (activeId.value = row.id),
    onClose: () => (activeId.value = undefined),
  });
}

const bladeToolbar = computed<IBladeToolbar[]>(() => [
  { id: "refresh", title: t("CONTENT.TOOLBAR.REFRESH"), icon: "lucide-refresh-cw", clickHandler: () => load() },
  // a new menu belongs to a site: the one picked in the filter (or the only one)
  {
    id: "add",
    title: t("CONTENT.PAGES.MENUS.ADD"),
    icon: "lucide-plus",
    permissions: "content:create",
    disabled: !addTo.value,
    clickHandler: () => addTo.value && openBlade({ name: "ContentLinkListDetails", param: addTo.value.id, options: { store: addTo.value } }),
  },
]);

onMounted(load);

// a menu was added, changed or deleted
exposeToChildren({ reload: () => load() });
</script>
