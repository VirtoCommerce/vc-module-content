<template>
  <VcBlade
    :title="isNew ? $t('CONTENT.PAGES.LINK_LIST.TITLE_NEW') : item.name"
    :subtitle="$t('CONTENT.PAGES.LINK_LIST.SUBTITLE')"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="70%"
  >
    <VcContainer>
      <VcForm class="content-form tw-space-y-4">
        <VcRow class="tw-gap-4">
          <VcCol>
            <VcInput
              v-model="item.name"
              :label="$t('CONTENT.PAGES.LINK_LIST.FIELDS.NAME')"
              :error-message="nameError"
              required
            />
          </VcCol>
          <VcCol>
            <VcSelect
              v-model="item.language"
              :label="$t('CONTENT.PAGES.LINK_LIST.FIELDS.LANGUAGE')"
              :options="store?.languages ?? []"
              :option-value="(x: string) => x"
              :option-label="(x: string) => x"
              :placeholder="$t('CONTENT.PAGES.PAGE.FIELDS.LANGUAGE_PLACEHOLDER')"
            />
          </VcCol>
        </VcRow>

        <VcCard :header="$t('CONTENT.PAGES.LINK_LIST.LINKS')">
          <VcDataTable
            v-model:selection="selectedLinks"
            selection-mode="multiple"
            data-key="key"
            :items="item.menuLinks"
            :reorderable-rows="true"
            :column-switcher="false"
            :empty-state="{ icon: 'lucide-link', title: $t('CONTENT.PAGES.LINK_LIST.NO_LINKS') }"
            @row-reorder="(e: { value: Link[] }) => (item.menuLinks = e.value)"
          >
            <VcColumn
              id="title"
              :title="$t('CONTENT.PAGES.LINK_LIST.COLUMNS.TITLE')"
            >
              <template #body="{ data }">
                <VcInput
                  v-model="data.title"
                  size="small"
                  required
                />
              </template>
            </VcColumn>
            <VcColumn
              id="url"
              :title="$t('CONTENT.PAGES.LINK_LIST.COLUMNS.URL')"
            >
              <template #body="{ data }">
                <VcInput
                  v-model="data.url"
                  size="small"
                  required
                />
              </template>
            </VcColumn>
            <VcColumn
              id="associatedObjectType"
              :title="$t('CONTENT.PAGES.LINK_LIST.COLUMNS.TYPE')"
              width="150px"
            >
              <template #body="{ data }">
                <VcSelect
                  :model-value="data.associatedObjectType"
                  :options="objectTypes"
                  size="small"
                  :placeholder="$t('CONTENT.PAGES.LINK_LIST.NONE')"
                  @update:model-value="(v: string | undefined) => setType(data, v)"
                />
              </template>
            </VcColumn>
            <VcColumn
              id="associatedObjectId"
              :title="$t('CONTENT.PAGES.LINK_LIST.COLUMNS.OBJECT')"
            >
              <template #body="{ data }">
                <VcSelect
                  v-if="data.associatedObjectType"
                  :key="data.associatedObjectType"
                  :model-value="data.associatedObjectId"
                  :options="(keyword?: string, skip?: number, ids?: string[]) => searchCatalog(data, keyword, skip, ids)"
                  option-label="name"
                  size="small"
                  searchable
                  :emit-value="false"
                  @update:model-value="(v?: { id: string; name: string }) => setObject(data, v)"
                />
              </template>
            </VcColumn>
          </VcDataTable>
        </VcCard>
      </VcForm>
    </VcContainer>
  </VcBlade>
</template>

<script setup lang="ts">
import { generateId, useAsync, useBlade, useBladeForm, useLoading, usePopup, type IBladeToolbar } from "@vc-shell/framework";
import {
  VcBlade,
  VcCard,
  VcCol,
  VcColumn,
  VcContainer,
  VcDataTable,
  VcForm,
  VcInput,
  VcRow,
  VcSelect,
} from "@vc-shell/framework/ui";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import type { MenuLink, MenuLinkList } from "../../api_client/virtocommerce.content";
import type { Store } from "../../api_client/virtocommerce.store";
import { ensureStore, useContentClients } from "../api";

defineBlade({
  url: "/content-link-list",
  name: "ContentLinkListDetails",
  permissions: ["content:read"],
});

type Link = MenuLink & { key: string };
type Item = Omit<MenuLinkList, "menuLinks"> & { menuLinks: Link[] };

const { t } = useI18n({ useScope: "global" });
const { param, options, closeSelf, callParent, replaceWith } = useBlade<{ listId?: string; store?: Store; clone?: MenuLinkList }>();
const { showConfirmation, showError } = usePopup();
const clients = useContentClients();

const storeId = computed(() => param.value!);
const store = ref<Store | undefined>(options.value?.store);
const isNew = computed(() => !options.value?.listId);
const item = ref<Item>({ storeId: storeId.value, menuLinks: [] });
const selectedLinks = ref<Link[]>([]);
const nameError = ref<string>();

const form = useBladeForm({ data: item, closeConfirmMessage: computed(() => t("CONTENT.ALERTS.CLOSE_CONFIRMATION")) });

const objectTypes = computed(() => [
  { id: "product", title: t("CONTENT.PAGES.LINK_LIST.PRODUCT") },
  { id: "category", title: t("CONTENT.PAGES.LINK_LIST.CATEGORY") },
]);

const withKey = (link: MenuLink): Link => ({ ...link, key: link.id ?? generateId() });

function fill(list: MenuLinkList) {
  // the storefront orders links by priority, highest first
  const links = [...(list.menuLinks ?? [])].sort((a, b) => (b.priority ?? 0) - (a.priority ?? 0));
  item.value = { ...list, menuLinks: links.map(withKey) };
}

const { loading: loadLoading, action: load } = useAsync(async () => {
  await ensureStore(clients, storeId.value, store);
  if (options.value?.listId) fill(await (await clients.menu()).getList(options.value.listId, storeId.value));
  else if (options.value?.clone) fill(options.value.clone);
  else fill({ storeId: storeId.value, menuLinks: [{ priority: 0 }] });
  form.setBaseline();
});

function setType(link: Link, type?: string) {
  link.associatedObjectType = type || undefined;
  link.associatedObjectId = undefined;
  link.associatedObjectName = undefined;
}

function setObject(link: Link, value?: { id: string; name: string }) {
  link.associatedObjectId = value?.id;
  link.associatedObjectName = value?.name;
}

// The AngularJS editor picks products/categories in the Catalog blades; here the catalog search API does it.
async function searchCatalog(link: Link, keyword?: string, skip = 0, ids?: string[]) {
  if (ids?.length) {
    return { results: link.associatedObjectId ? [{ id: link.associatedObjectId, name: link.associatedObjectName ?? link.associatedObjectId }] : [], totalCount: 1 };
  }
  const kind = link.associatedObjectType === "category" ? "categories" : "products";
  const res = await fetch(`/api/catalog/search/${kind}`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ keyword: keyword || undefined, skip, take: 20, responseGroup: "Info", searchInChildren: true }),
  });
  const data = res.ok ? await res.json() : {};
  const results = ((data.items ?? data.results ?? []) as { id: string; name: string }[]).map((x) => ({ id: x.id, name: x.name }));
  return { results, totalCount: data.totalCount ?? results.length };
}

const linksValid = computed(() =>
  item.value.menuLinks.every((x) => x.title && x.url && (!x.associatedObjectType || x.associatedObjectId)),
);

const reloadParent = () => callParent("reload").catch(() => undefined);

const { loading: saveLoading, action: save } = useAsync(async () => {
  nameError.value = undefined;
  const client = await clients.menu();
  // checkname answers { result: bool } (the generated client types it as a bare bool)
  const check = (await client.checkName(storeId.value, item.value.name, item.value.language ?? "", item.value.id ?? "")) as unknown;
  if (!(typeof check === "boolean" ? check : (check as { result?: boolean })?.result)) {
    nameError.value = t("CONTENT.VALIDATION.LINK_LIST_EXISTS");
    showError(t("CONTENT.VALIDATION.LINK_LIST_EXISTS"));
    return;
  }
  const count = item.value.menuLinks.length;
  const menuLinks = item.value.menuLinks.map(({ key: _key, ...link }, i) => ({
    ...link,
    menuLinkListId: item.value.id,
    priority: (count - 1 - i) * 10,
  }));
  await client.updateMenuLinkList(storeId.value, { ...item.value, storeId: storeId.value, menuLinks });
  form.setBaseline();
  if (isNew.value) {
    await reloadParent();
    await closeSelf();
  } else await Promise.all([load(), reloadParent()]);
});

async function removeLinks() {
  if (!(await showConfirmation(t("CONTENT.ALERTS.DELETE_LINKS", { count: selectedLinks.value.length })))) return;
  item.value.menuLinks = item.value.menuLinks.filter((x) => !selectedLinks.value.includes(x));
  selectedLinks.value = [];
}

const { loading: deleteLoading, action: deleteList } = useAsync(async () => {
  await (await clients.menu()).deleteMenuLinkLists(storeId.value, [item.value.id!]);
  form.setBaseline();
  await reloadParent();
  await closeSelf();
});

const loading = useLoading(loadLoading, saveLoading, deleteLoading);

async function remove() {
  if (await showConfirmation(t("CONTENT.ALERTS.DELETE_LINK_LISTS", { count: 1 }))) await deleteList();
}

function clone() {
  const copy: MenuLinkList = {
    ...item.value,
    id: undefined,
    name: `${item.value.name} copy`,
    menuLinks: item.value.menuLinks.map(({ key: _key, ...x }) => ({ ...x, id: undefined, menuLinkListId: undefined })),
  };
  replaceWith({ name: "ContentLinkListDetails", param: storeId.value, options: { store: store.value, clone: copy } });
}

const bladeToolbar = ref<IBladeToolbar[]>([
  {
    id: "save",
    title: computed(() => t(isNew.value ? "CONTENT.TOOLBAR.CREATE" : "CONTENT.TOOLBAR.SAVE")),
    icon: "lucide-save",
    permissions: ["content:update"],
    disabled: computed(() => !item.value.name || !linksValid.value || (!isNew.value && !form.isModified.value)),
    clickHandler: () => save(),
  },
  {
    id: "reset",
    title: t("CONTENT.TOOLBAR.RESET"),
    icon: "lucide-undo-2",
    isVisible: computed(() => !isNew.value),
    disabled: computed(() => !form.isModified.value),
    clickHandler: () => form.revert(),
  },
  {
    id: "addLink",
    title: t("CONTENT.PAGES.LINK_LIST.ADD_LINK"),
    icon: "lucide-plus",
    clickHandler: () => item.value.menuLinks.push(withKey({ priority: 0, menuLinkListId: item.value.id })),
  },
  {
    id: "deleteLinks",
    title: t("CONTENT.PAGES.LINK_LIST.DELETE_LINKS"),
    icon: "lucide-list-x",
    disabled: computed(() => !selectedLinks.value.length),
    clickHandler: removeLinks,
  },
  {
    id: "clone",
    title: t("CONTENT.PAGES.LINK_LIST.CLONE"),
    icon: "lucide-copy",
    permissions: ["content:create"],
    isVisible: computed(() => !isNew.value),
    disabled: computed(() => form.isModified.value),
    clickHandler: clone,
  },
  {
    id: "delete",
    title: t("CONTENT.TOOLBAR.DELETE"),
    icon: "lucide-trash-2",
    permissions: ["content:delete"],
    isVisible: computed(() => !isNew.value),
    clickHandler: remove,
  },
]);

onMounted(load);
</script>
