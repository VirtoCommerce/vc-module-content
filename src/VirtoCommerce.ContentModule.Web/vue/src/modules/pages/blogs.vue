<template>
  <VcBlade
    :title="$t('CONTENT.MENU.BLOGS')"
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
      :empty-state="{ icon: 'lucide-notebook-pen', title: $t('CONTENT.PAGES.BLOGS.EMPTY') }"
      @row-click="(e: { data: Row }) => open(e.data)"
    >
      <VcColumn
        id="name"
        :title="$t('CONTENT.PAGES.BLOGS.COLUMNS.NAME')"
        body-class="content-list__name"
        always-visible
      />
      <VcColumn
        id="siteName"
        :title="$t('CONTENT.SITES.COLUMN')"
      />
    </VcDataTable>
  </VcBlade>
</template>

<script setup lang="ts">
import { useAsync, useBlade, type IBladeToolbar } from "@vc-shell/framework";
import { VcBlade, VcColumn, VcDataTable, VcSelect } from "@vc-shell/framework/ui";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import type { Store } from "../../api_client/virtocommerce.store";
import { isFolder, useContentClients, type ContentFile } from "../api";
import { useAllSites } from "../composables/useAllSites";

// every site's blogs in one list; a blog opens its posts, as in the site's files
defineBlade({
  url: "/content-blogs",
  name: "ContentBlogs",
  isWorkspace: true,
  permissions: ["content:access"],
  menuItem: { title: "CONTENT.MENU.BLOGS", icon: "lucide-notebook-pen", priority: 41 },
});

type Row = { id: string; name?: string; siteName?: string; store: Store; blog: ContentFile };

const { t } = useI18n({ useScope: "global" });
const { openBlade, exposeToChildren } = useBlade();
const clients = useContentClients();
const { sites, siteId, siteOptions, loadSites, shows, addTo } = useAllSites();

const all = ref<Row[]>([]);
const rows = computed(() => all.value.filter((x) => shows(x.store.id)));
const activeId = ref<string>();

const { loading, action: load } = useAsync(async () => {
  const [stores, content] = await Promise.all([loadSites(), clients.content()]);
  const perSite = await Promise.all(
    stores.map(async (store) =>
      (((await content.searchContent("blogs", store.id!).catch(() => [])) ?? []) as ContentFile[])
        .filter(isFolder)
        .map((blog) => ({ id: `${store.id}/${blog.relativeUrl}`, name: blog.name, siteName: store.name, store, blog })),
    ),
  );
  all.value = perSite.flat();
});

function open(row: Row) {
  openBlade({
    name: "ContentFiles",
    param: row.store.id,
    options: { contentType: "blogs", store: row.store, root: row.blog.relativeUrl, title: row.name },
    onOpen: () => (activeId.value = row.id),
    onClose: () => (activeId.value = undefined),
  });
}

function add() {
  if (addTo.value) openBlade({ name: "ContentBlogDetails", param: addTo.value.id, options: { contentType: "blogs", store: addTo.value, folderUrl: "" } });
}

const bladeToolbar = computed<IBladeToolbar[]>(() => [
  { id: "refresh", title: t("CONTENT.TOOLBAR.REFRESH"), icon: "lucide-refresh-cw", clickHandler: () => load() },
  // a new blog belongs to a site: the one picked in the filter (or the only one)
  { id: "add", title: t("CONTENT.PAGES.FILES.ADD_BLOG"), icon: "lucide-plus", permissions: "content:create", disabled: !addTo.value, clickHandler: add },
]);

onMounted(load);

// a blog was added, renamed or deleted
exposeToChildren({ reload: () => load() });
</script>
