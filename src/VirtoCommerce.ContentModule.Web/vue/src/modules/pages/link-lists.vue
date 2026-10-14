<template>
  <VcBlade
    :title="$t('CONTENT.TYPES.LINKLISTS')"
    :subtitle="store?.name"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="50%"
  >
    <VcDataTable
      v-model:selection="selection"
      selection-mode="multiple"
      :items="lists"
      :state-key="'CONTENT_LINK_LISTS'"
      :active-item-id="activeItemId"
      :empty-state="{ icon: 'lucide-link', title: $t('CONTENT.PAGES.LINK_LISTS.EMPTY') }"
      @row-click="(e: { data: MenuLinkList }) => open(e.data)"
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
        id="links"
        :title="$t('CONTENT.PAGES.LINK_LISTS.COLUMNS.LINKS')"
        align="end"
      >
        <template #body="{ data }"><span class="vc-table-cell-number">{{ data.menuLinks?.length ?? 0 }}</span></template>
      </VcColumn>
    </VcDataTable>
  </VcBlade>
</template>

<script setup lang="ts">
import { useAsync, useBlade, useLoading, usePopup, type IBladeToolbar } from "@vc-shell/framework";
import { VcBlade, VcColumn, VcDataTable } from "@vc-shell/framework/ui";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import type { MenuLinkList } from "../../api_client/virtocommerce.content";
import type { Store } from "../../api_client/virtocommerce.store";
import { ensureStore, useContentClients } from "../api";

defineBlade({
  url: "/content-link-lists",
  name: "ContentLinkLists",
  permissions: ["content:read"],
});

const { t } = useI18n({ useScope: "global" });
const { param, options, openBlade, callParent, exposeToChildren, closeChildren } = useBlade<{ store?: Store }>();
const { showConfirmation } = usePopup();
const clients = useContentClients();

const storeId = computed(() => param.value!);
const store = ref<Store | undefined>(options.value?.store);
const lists = ref<MenuLinkList[]>([]);
const selection = ref<MenuLinkList[]>([]);
const activeItemId = ref<string>();

const { loading: listLoading, action: load } = useAsync(async () => {
  lists.value = ((await (await clients.menu()).getLists(storeId.value)) ?? []).sort((a, b) =>
    (a.name ?? "").localeCompare(b.name ?? ""),
  );
  selection.value = [];
  await ensureStore(clients, storeId.value, store);
});

const reloadAll = () => Promise.all([load(), callParent("reload").catch(() => undefined)]);

// the list comes in options, not as the child's param (the store): highlighted once its blade opened
function open(list?: MenuLinkList) {
  openBlade({
    name: "ContentLinkListDetails",
    param: storeId.value,
    options: { listId: list?.id, store: store.value },
    onOpen: () => (activeItemId.value = list?.id),
    onClose: () => (activeItemId.value = undefined),
  });
}

const { loading: removeLoading, action: removeSelected } = useAsync(async () => {
  await (await clients.menu()).deleteMenuLinkLists(storeId.value, selection.value.map((x) => x.id!));
  await reloadAll();
});

const loading = useLoading(listLoading, removeLoading);

async function remove() {
  if (!(await showConfirmation(t("CONTENT.ALERTS.DELETE_LINK_LISTS", { count: selection.value.length })))) return;
  await closeChildren();
  await removeSelected();
}

const bladeToolbar = ref<IBladeToolbar[]>([
  { id: "refresh", title: t("CONTENT.TOOLBAR.REFRESH"), icon: "lucide-refresh-cw", clickHandler: () => load() },
  { id: "add", title: t("CONTENT.TOOLBAR.ADD"), icon: "lucide-plus", permissions: ["content:create"], clickHandler: () => open() },
  {
    id: "delete",
    title: t("CONTENT.TOOLBAR.DELETE"),
    icon: "lucide-trash-2",
    permissions: ["content:delete"],
    disabled: computed(() => !selection.value.length),
    clickHandler: remove,
  },
]);

onMounted(load);

exposeToChildren({ reload: reloadAll });
</script>
