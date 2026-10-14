<template>
  <VcBlade
    :title="title"
    :subtitle="subtitle"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="70%"
  >
    <VcContainer>
      <VcForm class="content-form tw-space-y-4">
        <VcField
          v-if="!isNew"
          :label="$t('CONTENT.PAGES.PAGE.FIELDS.PATH')"
          :model-value="relativeUrl"
          copyable
        />
        <VcRow class="tw-gap-4">
          <VcCol>
            <VcInput
              v-model="item.pageName"
              :label="$t('CONTENT.PAGES.PAGE.FIELDS.NAME')"
              :placeholder="$t('CONTENT.PAGES.PAGE.FIELDS.NAME_PLACEHOLDER')"
              :error-message="nameError"
              required
            />
          </VcCol>
          <VcCol>
            <VcSelect
              v-model="item.language"
              :label="$t('CONTENT.PAGES.PAGE.FIELDS.LANGUAGE')"
              :options="languages"
              :option-value="(x: string) => x"
              :option-label="(x: string) => x"
              :placeholder="$t('CONTENT.PAGES.PAGE.FIELDS.LANGUAGE_PLACEHOLDER')"
            />
          </VcCol>
        </VcRow>

        <!-- raw Markdown/HTML: the rich-text VcEditor drops inline HTML when it re-serializes a page -->
        <VcTextarea
          v-model="item.content"
          class="content-code"
          :label="$t('CONTENT.PAGES.PAGE.FIELDS.CONTENT')"
          :hint="$t('CONTENT.PAGES.PAGE.CONTENT_HINT')"
        />

        <VcCard
          v-if="hasFrontMatter(extension)"
          :header="$t('CONTENT.PAGES.PAGE.METADATA')"
        >
          <FrontMatterFields v-model="item.metadata" />
        </VcCard>
      </VcForm>
    </VcContainer>
  </VcBlade>
</template>

<script setup lang="ts">
import { useAsync, useBlade, useBladeForm, useLoading, usePopup, type IBladeToolbar } from "@vc-shell/framework";
import {
  VcBlade,
  VcCard,
  VcCol,
  VcContainer,
  VcField,
  VcForm,
  VcInput,
  VcRow,
  VcSelect,
  VcTextarea,
} from "@vc-shell/framework/ui";
import FrontMatterFields from "../components/FrontMatterFields.vue";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import type { Store } from "../../api_client/virtocommerce.store";
import {
  buildFrontMatter,
  buildPageName,
  ensureStore,
  hasFrontMatter,
  parentFolder,
  parseFrontMatter,
  parsePageName,
  readFile,
  saveFile,
  undraft,
  useContentClients,
  type ContentFile,
  type ContentType,
  type FrontMatter,
} from "../api";

defineBlade({
  url: "/content-page",
  name: "ContentPageDetails",
  permissions: ["content:read"],
});

interface PageItem {
  pageName: string;
  language?: string;
  content: string;
  metadata: FrontMatter;
}

const { t } = useI18n({ useScope: "global" });
const { param, options, closeSelf, callParent } = useBlade<{
  contentType?: ContentType;
  store?: Store;
  folderUrl?: string;
  item?: ContentFile;
}>();
const { showConfirmation, showInfo } = usePopup();
const clients = useContentClients();

const contentType = computed<ContentType>(() => options.value?.contentType ?? "pages");
const storeId = computed(() => param.value!);
const store = ref<Store | undefined>(options.value?.store);
const languages = computed(() => store.value?.languages ?? []);

const file = ref<ContentFile | undefined>(options.value?.item);
const isNew = computed(() => !file.value);
const relativeUrl = computed(() => undraft(file.value?.relativeUrl));
const folderUrl = computed(() => (file.value ? parentFolder(relativeUrl.value) : (options.value?.folderUrl ?? "")));
const extension = ref("md");

const item = ref<PageItem>({ pageName: "", content: "", metadata: {} });

const form = useBladeForm({
  data: item,
  closeConfirmMessage: computed(() => t("CONTENT.ALERTS.CLOSE_CONFIRMATION")),
});

const isPost = computed(() => contentType.value === "blogs");
const title = computed(() =>
  isNew.value ? t(isPost.value ? "CONTENT.PAGES.PAGE.TITLE_NEW_POST" : "CONTENT.PAGES.PAGE.TITLE_NEW") : file.value!.name!,
);
const subtitle = computed(() => t(isPost.value ? "CONTENT.PAGES.PAGE.SUBTITLE_POST" : "CONTENT.PAGES.PAGE.SUBTITLE"));
const nameError = computed(() =>
  item.value.pageName && !/^[^\\/:*?"<>|.][^\\/:*?"<>|]*$/.test(item.value.pageName)
    ? t("CONTENT.VALIDATION.FILE_NAME")
    : undefined,
);

const reloadParent = () => callParent("reload").catch(() => undefined);

const { loading: loadLoading, action: load } = useAsync(async () => {
  await ensureStore(clients, storeId.value, store);
  if (file.value) {
    const name = parsePageName(file.value.name!, languages.value);
    extension.value = name.extension;
    const raw = await readFile(contentType.value, storeId.value, relativeUrl.value!);
    const { metadata, content } = hasFrontMatter(name.extension) ? parseFrontMatter(raw) : { metadata: {}, content: raw };
    item.value = { pageName: name.pageName, language: name.language, content, metadata };
  }
  form.setBaseline();
});

const { loading: saveLoading, action: save } = useAsync(async () => {
  if (!item.value.pageName || nameError.value) return;
  const oldUrl = relativeUrl.value;
  const fileName = buildPageName(item.value.pageName, item.value.language, extension.value);
  const raw = hasFrontMatter(extension.value) ? buildFrontMatter(item.value.metadata, item.value.content) : item.value.content;
  const [saved] = await saveFile(contentType.value, storeId.value, folderUrl.value, fileName, raw, true);
  const wasNew = isNew.value;
  file.value = saved;
  // renamed: the old file (and its draft) goes away
  if (oldUrl && oldUrl !== undraft(saved.relativeUrl)) {
    await (await clients.content()).deleteContent(contentType.value, storeId.value, [oldUrl]);
  }
  form.setBaseline();
  await reloadParent();
  if (wasNew) await closeSelf();
});

const { loading: publishLoading, action: setPublished } = useAsync<boolean>(async (publish) => {
  const client = await clients.content();
  await client.publishing(contentType.value, storeId.value, relativeUrl.value, publish);
  const status = await client.publishStatus(contentType.value, storeId.value, relativeUrl.value);
  file.value = {
    ...file.value!,
    published: status.published,
    hasChanges: status.hasChanges,
    relativeUrl: status.hasChanges ? `${relativeUrl.value}-draft` : relativeUrl.value,
  };
  await reloadParent();
});

function preview() {
  const storeUrl = store.value?.url?.replace(/\/$/, "");
  if (!storeUrl) return showInfo(t("CONTENT.ALERTS.NO_STORE_URL"));
  // the storefront's designer preview takes the document id the AngularJS editor builds
  const real = file.value?.hasChanges ? `${relativeUrl.value}-draft` : relativeUrl.value;
  const documentId = btoa(`${storeId.value}::${contentType.value}::${real}`).replace(/=/g, "-");
  window.open(`${storeUrl}/designer-preview?pageId=${encodeURIComponent(documentId)}`, "_blank");
}

const { loading: deleteLoading, action: deletePage } = useAsync(async () => {
  await (await clients.content()).deleteContent(contentType.value, storeId.value, [relativeUrl.value!]);
  form.setBaseline();
  await reloadParent();
  await closeSelf();
});

const loading = useLoading(loadLoading, saveLoading, publishLoading, deleteLoading);

async function remove() {
  if (await showConfirmation(t("CONTENT.ALERTS.DELETE_PAGE", { name: file.value?.name }))) await deletePage();
}

const canPublish = computed(() => !isNew.value && !form.isModified.value);
const isPublished = computed(() => !!file.value?.published && !file.value?.hasChanges);

const bladeToolbar = ref<IBladeToolbar[]>([
  {
    id: "save",
    title: computed(() => t(isNew.value ? "CONTENT.TOOLBAR.CREATE" : "CONTENT.TOOLBAR.SAVE")),
    icon: "lucide-save",
    // saving uploads the file (UploadContent: content:create)
    permissions: ["content:create"],
    disabled: computed(() => !item.value.pageName || !!nameError.value || (!isNew.value && !form.canSave.value)),
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
    id: "publish",
    title: t("CONTENT.TOOLBAR.PUBLISH"),
    icon: "lucide-file-check",
    permissions: ["content:create"],
    isVisible: computed(() => !isNew.value && !isPublished.value),
    disabled: computed(() => !canPublish.value),
    clickHandler: () => setPublished(true),
  },
  {
    id: "unpublish",
    title: t("CONTENT.TOOLBAR.UNPUBLISH"),
    icon: "lucide-file-x",
    permissions: ["content:create"],
    isVisible: computed(() => !isNew.value && isPublished.value),
    disabled: computed(() => !canPublish.value),
    clickHandler: () => setPublished(false),
  },
  {
    id: "preview",
    title: t("CONTENT.TOOLBAR.PREVIEW"),
    icon: "lucide-eye",
    isVisible: computed(() => !isNew.value),
    clickHandler: preview,
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
