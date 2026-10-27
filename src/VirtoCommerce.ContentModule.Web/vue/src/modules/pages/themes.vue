<template>
  <VcBlade
    :title="$t('CONTENT.TYPES.THEMES')"
    :subtitle="store?.name"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="50%"
  >
    <VcDataTable
      data-key="relativeUrl"
      :items="themes"
      :state-key="'CONTENT_THEMES'"
      :active-item-id="activeItemId"
      :row-actions="rowActions"
      row-actions-mode="dropdown"
      :empty-state="{ icon: 'lucide-palette', title: $t('CONTENT.PAGES.THEMES.EMPTY') }"
      @row-click="(e: { data: ContentFile }) => openTheme(e.data)"
    >
      <VcColumn
        id="name"
        :title="$t('CONTENT.PAGES.THEMES.COLUMNS.NAME')"
        body-class="content-list__name"
        always-visible
      />
      <VcColumn
        id="active"
        :title="$t('CONTENT.PAGES.THEMES.COLUMNS.STATUS')"
      >
        <template #body="{ data }">
          <div
            v-if="data.name === activeTheme"
            class="tw-inline-flex"
          >
            <VcStatus variant="success">{{ $t("CONTENT.PAGES.THEMES.ACTIVE") }}</VcStatus>
          </div>
        </template>
      </VcColumn>
      <VcColumn
        id="modifiedDate"
        :title="$t('CONTENT.PAGES.FILES.COLUMNS.MODIFIED')"
        type="datetime"
      />
    </VcDataTable>

    <NamePrompt
      :prompt="prompt"
      @done="prompt = undefined"
    />
    <VcPopup
      v-if="uploading"
      :model-value="true"
      :title="$t('CONTENT.PAGES.THEMES.UPLOAD_TITLE')"
      @update:model-value="uploading = false"
    >
      <template #content>
        <VcFileUpload
          accept=".zip"
          :loading="uploadLoading"
          @upload="upload"
        />
      </template>
    </VcPopup>
  </VcBlade>
</template>

<script setup lang="ts">
import { useAsync, useBlade, useLoading, usePermissions, usePopup, type IBladeToolbar } from "@vc-shell/framework";
import { VcBlade, VcColumn, VcDataTable, VcFileUpload, VcPopup, VcStatus } from "@vc-shell/framework/ui";
import { computed, onMounted, ref, shallowRef } from "vue";
import { useI18n } from "vue-i18n";
import type { Store } from "../../api_client/virtocommerce.store";
import NamePrompt, { type NamePromptRequest } from "../components/NamePrompt.vue";
import {
  isFolder,
  saveFile,
  setStoreDefaultTheme,
  storeDefaultTheme,
  useContentClients,
  validateFolderName,
  type ContentFile,
} from "../api";

defineBlade({
  url: "/content-themes",
  name: "ContentThemes",
  permissions: ["content:read"],
});

const { t } = useI18n({ useScope: "global" });
const { param, openBlade, callParent, exposeToChildren, closeChildren } = useBlade<{ store?: Store }>();
const { showConfirmation, showInfo } = usePopup();
const { hasAccess } = usePermissions();
const clients = useContentClients();

const storeId = computed(() => param.value!);
const store = ref<Store>();
const themes = ref<ContentFile[]>([]);
const activeTheme = computed(() => storeDefaultTheme(store.value));
const activeItemId = ref<string>();
const prompt = shallowRef<NamePromptRequest>();
const uploading = ref(false);
const uploadLoading = ref(false);

const { loading: listLoading, action: load } = useAsync(async () => {
  const [content, stores] = await Promise.all([clients.content(), clients.stores()]);
  const [items, s] = await Promise.all([
    content.searchContent("themes", storeId.value),
    stores.getStoreById(storeId.value),
  ]);
  themes.value = (items ?? []).filter(isFolder);
  store.value = s;
});

const reloadAll = () => Promise.all([load(), callParent("reload").catch(() => undefined)]);

const { loading: running, action: runAction } = useAsync<() => Promise<unknown>>(async (action) => {
  await action!();
});

const loading = useLoading(listLoading, running);

// the list (and the parent's counts) show the outcome, also of a failed or partly done change
async function run(action: () => Promise<unknown>) {
  await runAction(action).catch(() => undefined);
  await reloadAll();
}

// the row is a theme folder, not the child's param (the store): highlighted once its blade opened
function openTheme(theme: ContentFile) {
  openBlade({
    name: "ContentFiles",
    param: storeId.value,
    options: { contentType: "themes", store: store.value, root: theme.relativeUrl, rootUrl: theme.url, title: theme.name },
    onOpen: () => (activeItemId.value = theme.relativeUrl),
    onClose: () => (activeItemId.value = undefined),
  });
}

function askName(title: string, value?: string) {
  return new Promise<string | undefined>(
    (resolve) =>
      (prompt.value = {
        title,
        label: t("CONTENT.PAGES.THEMES.NAME"),
        value,
        validate: (v) => {
          if (validateFolderName(v)) return t("CONTENT.VALIDATION.FOLDER_NAME");
          if (v !== value && themes.value.some((x) => x.name === v)) return t("CONTENT.VALIDATION.THEME_EXISTS");
          return undefined;
        },
        resolve,
      }),
  );
}

async function addTheme() {
  const name = await askName(t("CONTENT.PAGES.THEMES.NEW"));
  if (!name) return;
  const first = !themes.value.length;
  await run(async () => {
    await (await clients.content()).createContentFolder("themes", storeId.value, { name });
    // the first theme of a store becomes its active theme (as in the AngularJS blade)
    if (first) await setStoreDefaultTheme(clients, storeId.value, name);
  });
}

async function rename(theme: ContentFile) {
  const name = await askName(t("CONTENT.PAGES.FILES.RENAME"), theme.name);
  if (!name || name === theme.name) return;
  await closeChildren();
  await run(async () => {
    await (await clients.content()).moveContent("themes", storeId.value, theme.relativeUrl, `/${name}`);
    if (theme.name === activeTheme.value) await setStoreDefaultTheme(clients, storeId.value, name);
  });
}

async function setActive(theme: ContentFile) {
  await run(() => setStoreDefaultTheme(clients, storeId.value, theme.name!));
}

async function remove(theme: ContentFile) {
  const message = t(themes.value.length > 1 ? "CONTENT.ALERTS.DELETE_THEME" : "CONTENT.ALERTS.DELETE_LAST_THEME", {
    name: theme.name,
  });
  if (!(await showConfirmation(message))) return;
  await closeChildren();
  await run(async () => {
    await (await clients.content()).deleteContent("themes", storeId.value, [theme.relativeUrl!]);
    if (theme.name === activeTheme.value) await setStoreDefaultTheme(clients, storeId.value, "");
  });
}

function preview(theme: ContentFile) {
  if (store.value?.url) window.open(`${store.value.url}?previewtheme=${theme.name}`, "_blank");
  else showInfo(t("CONTENT.ALERTS.NO_STORE_URL"));
}

async function upload(files: FileList) {
  const file = files[0];
  if (!file) return;
  const name = file.name.replace(/\.zip$/i, "");
  if (themes.value.some((x) => x.name === name) && !(await showConfirmation(t("CONTENT.ALERTS.OVERWRITE_THEME", { name })))) {
    return;
  }
  const first = !themes.value.length;
  uploadLoading.value = true;
  await run(async () => {
    // upload the archive next to the themes, then unpack it into "/<name>" (the server removes the archive)
    await saveFile("themes", storeId.value, "", file.name, file);
    await (await clients.content()).unpack("themes", storeId.value, file.name, name);
    if (first) await setStoreDefaultTheme(clients, storeId.value, name);
    uploading.value = false;
  });
  uploadLoading.value = false;
}

const rowActions = (theme: ContentFile) =>
  [
    {
      type: "info" as const,
      id: "setActive",
      title: t("CONTENT.PAGES.THEMES.SET_ACTIVE"),
      icon: "lucide-circle-check",
      hidden: theme.name === activeTheme.value || !hasAccess("content:update"),
      clickHandler: () => setActive(theme),
    },
    { type: "info" as const, id: "preview", title: t("CONTENT.TOOLBAR.PREVIEW"), icon: "lucide-eye", clickHandler: () => preview(theme) },
    { type: "info" as const, id: "rename", title: t("CONTENT.PAGES.FILES.ACTIONS.RENAME"), icon: "lucide-text-cursor-input", hidden: !hasAccess("content:update"), clickHandler: () => rename(theme) },
    { type: "info" as const, id: "delete", title: t("CONTENT.TOOLBAR.DELETE"), icon: "lucide-trash-2", variant: "danger" as const, hidden: !hasAccess("content:delete"), clickHandler: () => remove(theme) },
  ].filter((x) => !x.hidden);

const bladeToolbar = ref<IBladeToolbar[]>([
  { id: "refresh", title: t("CONTENT.TOOLBAR.REFRESH"), icon: "lucide-refresh-cw", clickHandler: () => load() },
  { id: "add", title: t("CONTENT.TOOLBAR.ADD"), icon: "lucide-plus", permissions: ["content:create"], clickHandler: addTheme },
  {
    id: "upload",
    title: t("CONTENT.TOOLBAR.UPLOAD"),
    icon: "lucide-upload",
    permissions: ["content:create"],
    clickHandler: () => (uploading.value = true),
  },
]);

onMounted(load);

exposeToChildren({ reload: reloadAll });
</script>
