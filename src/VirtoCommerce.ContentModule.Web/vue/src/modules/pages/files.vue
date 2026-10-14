<template>
  <VcBlade
    :title="title"
    :subtitle="store?.name"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="50%"
  >
    <VcDataTable
      v-model:search-value="searchValue"
      v-model:selection="selection"
      selection-mode="multiple"
      data-key="relativeUrl"
      :items="items"
      :searchable="true"
      :state-key="`CONTENT_FILES_${contentType.toUpperCase()}`"
      :active-item-id="activeItemId"
      :row-actions="rowActions"
      row-actions-mode="dropdown"
      :empty-state="{ icon: 'lucide-folder-open', title: $t('CONTENT.PAGES.FILES.EMPTY') }"
      @row-click="(e: { data: ContentFile }) => openItem(e.data)"
    >
      <template #header>
        <div class="content-crumbs">
          <VcBreadcrumbs
            :items="crumbs"
            separated
          />
        </div>
      </template>
      <VcColumn
        id="name"
        :title="$t('CONTENT.PAGES.FILES.COLUMNS.NAME')"
        always-visible
      >
        <template #body="{ data }">
          <span class="content-file">
            <VcIcon
              :icon="iconOf(data)"
              size="s"
            />
            <span>{{ data.name }}</span>
          </span>
        </template>
      </VcColumn>
      <VcColumn
        v-if="contentType !== 'themes'"
        id="status"
        :title="$t('CONTENT.PAGES.FILES.COLUMNS.STATUS')"
      >
        <template #body="{ data }">
          <div
            v-if="!isFolder(data)"
            class="tw-inline-flex"
          >
            <VcStatus :variant="publishState(data).variant">{{ publishState(data).label }}</VcStatus>
          </div>
        </template>
      </VcColumn>
      <VcColumn
        id="size"
        :title="$t('CONTENT.PAGES.FILES.COLUMNS.SIZE')"
        align="end"
      />
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
      :title="$t('CONTENT.PAGES.FILES.UPLOAD_TITLE')"
      @update:model-value="uploading = false"
    >
      <template #content>
        <VcFileUpload
          accept="*"
          multiple
          :loading="uploadLoading"
          @upload="upload"
        />
        <form
          class="content-upload-url"
          @submit.prevent="uploadUrl"
        >
          <VcInput
            v-model="externalUrl"
            type="url"
            class="content-upload-url__input"
            :label="$t('CONTENT.PAGES.FILES.UPLOAD_URL')"
            :placeholder="$t('CONTENT.PAGES.FILES.UPLOAD_URL_PLACEHOLDER')"
          />
          <VcButton
            :disabled="!/^https?:\/\/.+/.test(externalUrl)"
            :loading="uploadLoading"
            @click="uploadUrl"
          >
            {{ $t("CONTENT.TOOLBAR.UPLOAD") }}
          </VcButton>
        </form>
      </template>
    </VcPopup>
  </VcBlade>
</template>

<script setup lang="ts">
import { useAsync, useBlade, useFunctions, useLoading, usePermissions, usePopup, notification, type IBladeToolbar } from "@vc-shell/framework";
import { VcBlade, VcBreadcrumbs, VcButton, VcColumn, VcDataTable, VcFileUpload, VcIcon, VcInput, VcPopup, VcStatus } from "@vc-shell/framework/ui";
import { computed, onMounted, ref, shallowRef, watch } from "vue";
import { useI18n } from "vue-i18n";
import type { Store } from "../../api_client/virtocommerce.store";
import NamePrompt, { type NamePromptRequest } from "../components/NamePrompt.vue";
import {
  ensureStore,
  folderCrumbs,
  isDesignedPage,
  isFolder,
  isImage,
  isText,
  openPageBuilder,
  renameBlog,
  saveFile,
  trimSlashes,
  undraft,
  uploadFromUrl,
  useContentClients,
  validateFolderName,
  type ContentFile,
  type ContentType,
} from "../api";

defineBlade({
  url: "/content-files",
  name: "ContentFiles",
  permissions: ["content:read"],
});

const { t } = useI18n({ useScope: "global" });
const { param, options, openBlade, callParent, exposeToChildren, closeChildren } = useBlade<{
  contentType?: ContentType;
  store?: Store;
  /** a theme's files are browsed below "/<theme>" */
  root?: string;
  /** the theme folder's absolute url */
  rootUrl?: string;
  title?: string;
}>();
const { debounce } = useFunctions();
const { showConfirmation } = usePopup();
const { hasAccess } = usePermissions();
const clients = useContentClients();

const contentType = computed<ContentType>(() => options.value?.contentType ?? "pages");
const storeId = computed(() => param.value!);
const store = ref<Store | undefined>(options.value?.store);
const root = computed(() => options.value?.root ?? "");
const folderUrl = ref(root.value);
/** absolute folder urls by relative url: the folder endpoint takes the parent as absolute (Azure reads a relative one's first segment as the container) */
const folderAbsoluteUrls = new Map<string, string | undefined>([[trimSlashes(root.value), options.value?.rootUrl]]);

const items = ref<ContentFile[]>([]);
const selection = ref<ContentFile[]>([]);
const searchValue = ref<string>();
const activeItemId = ref<string>();
const indexedSearch = ref(false);
const prompt = shallowRef<NamePromptRequest>();
const uploading = ref(false);
const externalUrl = ref("");

const title = computed(
  () => options.value?.title ?? t(`CONTENT.TYPES.${contentType.value.toUpperCase()}`),
);
/** inside a blog folder you add posts; at the blogs root you add blogs */
const isBlogsRoot = computed(() => contentType.value === "blogs" && !trimSlashes(folderUrl.value));

const rootTitle = computed(() => options.value?.title ?? t("CONTENT.PAGES.FILES.ROOT"));
const crumbs = computed(() => folderCrumbs(root.value, folderUrl.value, rootTitle.value, navigate));

function iconOf(item: ContentFile) {
  if (isFolder(item)) return contentType.value === "blogs" && isBlogsRoot.value ? "lucide-newspaper" : "lucide-folder";
  if (isImage(item)) return "lucide-image";
  if (isDesignedPage(item)) return "lucide-layout-template";
  return "lucide-file-text";
}

function publishState(item: ContentFile) {
  if (item.published && !item.hasChanges) return { label: t("CONTENT.STATUS.PUBLISHED"), variant: "success" as const };
  if (item.published) return { label: t("CONTENT.STATUS.CHANGED"), variant: "warning" as const };
  return { label: t("CONTENT.STATUS.DRAFT"), variant: "info" as const };
}

const { loading: listLoading, action: load } = useAsync(async () => {
  const client = await clients.content();
  const keyword = searchValue.value || undefined;
  if (keyword && indexedSearch.value && contentType.value !== "themes") {
    // the full-text endpoint answers with a serialized Task: the items are under "result"
    const response = (await client.fulltextSearchContent({
      storeId: storeId.value,
      contentType: contentType.value,
      folderUrl: folderUrl.value || undefined,
      keyword,
      searchPhrase: `${keyword} contentType:${contentType.value}`,
      take: 100,
    })) as unknown as { result?: ContentFile[] } | ContentFile[];
    items.value = (Array.isArray(response) ? response : response?.result) ?? [];
  } else {
    items.value = ((await client.searchContent(contentType.value, storeId.value, folderUrl.value || undefined, keyword)) ??
      []) as ContentFile[];
  }
  // every folder you can open was listed first
  for (const x of items.value) if (isFolder(x)) folderAbsoluteUrls.set(trimSlashes(x.relativeUrl), x.url);
  selection.value = [];
});

async function reloadAll() {
  await Promise.all([load(), callParent("reload").catch(() => undefined)]);
}

function navigate(url: string) {
  closeChildren();
  folderUrl.value = url;
  searchValue.value = undefined;
  load();
}

// the row is a file, not the child's param (the store): highlighted once its blade opened
function openItem(item: ContentFile) {
  if (isFolder(item)) return navigate(item.relativeUrl!);
  if (isDesignedPage(item)) return openPageBuilder(storeId.value);
  const onOpen = () => (activeItemId.value = item.relativeUrl);
  const onClose = () => (activeItemId.value = undefined);
  const childOptions = { contentType: contentType.value, store: store.value, folderUrl: folderUrl.value, item };
  if (contentType.value === "themes") {
    if (isText(item) || isImage(item)) openBlade({ name: "ContentAssetDetails", param: storeId.value, options: childOptions, onOpen, onClose });
  } else if (isText(item)) {
    openBlade({ name: "ContentPageDetails", param: storeId.value, options: childOptions, onOpen, onClose });
  }
}

function addItem() {
  const childOptions = { contentType: contentType.value, store: store.value, folderUrl: folderUrl.value };
  if (isBlogsRoot.value) openBlade({ name: "ContentBlogDetails", param: storeId.value, options: childOptions });
  else if (contentType.value === "themes") openBlade({ name: "ContentAssetDetails", param: storeId.value, options: childOptions });
  else openBlade({ name: "ContentPageDetails", param: storeId.value, options: childOptions });
}

function ask(request: Omit<NamePromptRequest, "resolve">) {
  return new Promise<string | undefined>((resolve) => (prompt.value = { ...request, resolve }));
}

async function newFolder() {
  const name = await ask({
    title: t("CONTENT.PAGES.FILES.NEW_FOLDER"),
    label: t("CONTENT.PAGES.FILES.FOLDER_NAME"),
    validate: (v) => (validateFolderName(v) ? t("CONTENT.VALIDATION.FOLDER_NAME") : undefined),
  });
  if (!name) return;
  // the open folder's absolute url, as the AngularJS blades sent (none at the pages root)
  const parentUrl = folderAbsoluteUrls.get(trimSlashes(folderUrl.value));
  await run(async (client) => client.createContentFolder(contentType.value, storeId.value, { name, parentUrl }));
}

async function rename(item: ContentFile) {
  const oldName = undraft(item.name)!;
  const blog = isBlogsRoot.value && isFolder(item);
  const name = await ask({
    title: t("CONTENT.PAGES.FILES.RENAME"),
    label: t("CONTENT.PAGES.FILES.NEW_NAME"),
    value: oldName,
    validate: (v) => {
      if (!blog) return /[\\/:*?"<>|]/.test(v) ? t("CONTENT.VALIDATION.FILE_NAME") : undefined;
      if (validateFolderName(v)) return t("CONTENT.VALIDATION.FOLDER_NAME");
      return v !== oldName && items.value.some((x) => x.name === v) ? t("CONTENT.VALIDATION.BLOG_EXISTS") : undefined;
    },
  });
  if (!name || name === oldName) return;
  // a blog's metadata file is named after it and moves too
  if (blog) return run((client) => renameBlog(client, storeId.value, oldName, name));
  // relativeUrl is escaped and, on Azure, a folder's ends in "/": cut at the last "/" instead of by the name's length
  const url = undraft(item.relativeUrl)!;
  const parent = url.replace(/\/+$/, "").replace(/[^/]*$/, "");
  await run((client) => client.moveContent(contentType.value, storeId.value, url, parent + name));
}

async function duplicate(item: ContentFile) {
  await run((client) => client.copyFile(contentType.value, storeId.value, item.relativeUrl));
}

async function remove(list: ContentFile[]) {
  if (!list.length) return;
  const message =
    isBlogsRoot.value && list.some(isFolder)
      ? t("CONTENT.ALERTS.DELETE_BLOGS", { count: list.length })
      : t("CONTENT.ALERTS.DELETE_FILES", { count: list.length });
  if (!(await showConfirmation(message))) return;
  await closeChildren();
  await run((client) => client.deleteContent(contentType.value, storeId.value, list.map((x) => x.relativeUrl!)));
}

type ContentApi = Awaited<ReturnType<typeof clients.content>>;
const { loading: running, action: runAction } = useAsync<(client: ContentApi) => Promise<unknown>>(async (action) => {
  await action!(await clients.content());
});

// the list (and the parent's counts) show the outcome, also of a failed or partly done change
async function run(action: (client: ContentApi) => Promise<unknown>) {
  await runAction(action).catch(() => undefined);
  await reloadAll();
}

const { loading: filesUploading, action: uploadFiles } = useAsync<FileList>(async (files) => {
  // one after the other, as the AngularJS uploader queue
  for (const file of Array.from(files!)) {
    await saveFile(contentType.value, storeId.value, folderUrl.value, file.name, file);
  }
  uploading.value = false;
});

const { loading: urlUploading, action: uploadLink } = useAsync(async () => {
  await uploadFromUrl(contentType.value, storeId.value, folderUrl.value, externalUrl.value);
  externalUrl.value = "";
  uploading.value = false;
});

const uploadLoading = useLoading(filesUploading, urlUploading);
const loading = useLoading(listLoading, running);

async function upload(files: FileList) {
  await uploadFiles(files).catch(() => undefined);
  await reloadAll();
}

async function uploadUrl() {
  if (!/^https?:\/\/.+/.test(externalUrl.value)) return;
  await uploadLink().catch(() => undefined);
  await reloadAll();
}

function move(list: ContentFile[]) {
  if (!list.length) return;
  openBlade({
    name: "ContentFolderPicker",
    param: storeId.value,
    options: { contentType: contentType.value, root: root.value, rootTitle: rootTitle.value, from: folderUrl.value, items: list },
  });
}

function download(item: ContentFile) {
  const link = document.createElement("a");
  link.href = item.url!;
  link.download = undraft(item.name) ?? "download";
  link.click();
}

async function copyUrl(item: ContentFile) {
  await navigator.clipboard.writeText(item.url!);
  notification.success(t("CONTENT.ALERTS.URL_COPIED"));
}

const rowActions = (item: ContentFile) => {
  const file = !isFolder(item);
  const canUpdate = hasAccess("content:update");
  return [
    {
      type: "info" as const,
      id: "pageBuilder",
      title: t("CONTENT.PAGES.FILES.ACTIONS.PAGE_BUILDER"),
      icon: "lucide-layout-template",
      hidden: !isDesignedPage(item),
      clickHandler: () => openPageBuilder(storeId.value),
    },
    {
      type: "info" as const,
      id: "editBlog",
      title: t("CONTENT.PAGES.FILES.ACTIONS.EDIT_BLOG"),
      icon: "lucide-pencil",
      hidden: !(isBlogsRoot.value && isFolder(item)),
      clickHandler: () =>
        openBlade({
          name: "ContentBlogDetails",
          param: storeId.value,
          options: { contentType: contentType.value, store: store.value, folderUrl: folderUrl.value, item },
        }),
    },
    // gated as the server endpoints are: move and copy-file need content:update, delete content:delete
    { type: "info" as const, id: "rename", title: t("CONTENT.PAGES.FILES.ACTIONS.RENAME"), icon: "lucide-text-cursor-input", hidden: !canUpdate, clickHandler: () => rename(item) },
    { type: "info" as const, id: "move", title: t("CONTENT.PAGES.FILES.ACTIONS.MOVE"), icon: "lucide-folder-input", hidden: !canUpdate, clickHandler: () => move([item]) },
    { type: "info" as const, id: "duplicate", title: t("CONTENT.PAGES.FILES.ACTIONS.DUPLICATE"), icon: "lucide-copy", hidden: !file || !canUpdate, clickHandler: () => duplicate(item) },
    { type: "info" as const, id: "download", title: t("CONTENT.PAGES.FILES.ACTIONS.DOWNLOAD"), icon: "lucide-download", hidden: !file, clickHandler: () => download(item) },
    { type: "info" as const, id: "copyUrl", title: t("CONTENT.PAGES.FILES.ACTIONS.COPY_URL"), icon: "lucide-link", clickHandler: () => copyUrl(item) },
    { type: "info" as const, id: "delete", title: t("CONTENT.TOOLBAR.DELETE"), icon: "lucide-trash-2", variant: "danger" as const, hidden: !hasAccess("content:delete"), clickHandler: () => remove([item]) },
  ].filter((x) => !x.hidden);
};

const addTitle = computed(() => {
  if (contentType.value === "themes") return t("CONTENT.PAGES.FILES.ADD_FILE");
  if (isBlogsRoot.value) return t("CONTENT.PAGES.FILES.ADD_BLOG");
  return t(contentType.value === "blogs" ? "CONTENT.PAGES.FILES.ADD_POST" : "CONTENT.PAGES.FILES.ADD_PAGE");
});

const bladeToolbar = ref<IBladeToolbar[]>([
  { id: "refresh", title: t("CONTENT.TOOLBAR.REFRESH"), icon: "lucide-refresh-cw", clickHandler: () => load() },
  { id: "add", title: addTitle, icon: "lucide-plus", permissions: ["content:create"], clickHandler: addItem },
  {
    id: "newFolder",
    title: t("CONTENT.PAGES.FILES.NEW_FOLDER"),
    icon: "lucide-folder-plus",
    permissions: ["content:create"],
    isVisible: computed(() => contentType.value !== "blogs"),
    clickHandler: newFolder,
  },
  {
    id: "upload",
    title: t("CONTENT.TOOLBAR.UPLOAD"),
    icon: "lucide-upload",
    permissions: ["content:create"],
    clickHandler: () => (uploading.value = true),
  },
  {
    id: "move",
    title: t("CONTENT.PAGES.FILES.ACTIONS.MOVE"),
    icon: "lucide-folder-input",
    permissions: ["content:update"],
    disabled: computed(() => !selection.value.length),
    clickHandler: () => move(selection.value),
  },
  {
    id: "delete",
    title: t("CONTENT.TOOLBAR.DELETE"),
    icon: "lucide-trash-2",
    permissions: ["content:delete"],
    disabled: computed(() => !selection.value.length),
    clickHandler: () => remove(selection.value),
  },
]);

watch(searchValue, debounce(load, 300));

onMounted(() =>
  // the first listing has no keyword, so it doesn't wait for the full-text flag
  Promise.all([
    ensureStore(clients, storeId.value, store),
    fetch("/api/content/search/enabled")
      .then((r) => r.json())
      .then((enabled) => (indexedSearch.value = !!enabled?.result))
      .catch(() => undefined),
    load(),
  ]),
);

exposeToChildren({ reload: reloadAll });
</script>

<style lang="scss">
.content-upload-url {
  display: flex;
  align-items: flex-end;
  gap: 8px;
  margin-top: 16px;

  &__input {
    flex: 1;
  }
}
</style>
