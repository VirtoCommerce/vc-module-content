<template>
  <VcBlade
    :title="isNew ? $t('CONTENT.PAGES.BLOG.TITLE_NEW') : folder!.name"
    :subtitle="$t('CONTENT.PAGES.BLOG.SUBTITLE')"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="50%"
  >
    <VcContainer>
      <VcForm class="content-form tw-space-y-4">
        <VcInput
          v-model="item.name"
          :label="$t('CONTENT.PAGES.BLOG.FIELDS.NAME')"
          :hint="$t('CONTENT.VALIDATION.FOLDER_NAME')"
          :error-message="nameError"
          required
        />
        <VcCard :header="$t('CONTENT.PAGES.PAGE.METADATA')">
          <FrontMatterFields v-model="item.metadata" />
        </VcCard>
      </VcForm>
    </VcContainer>
  </VcBlade>
</template>

<script setup lang="ts">
// A blog is a folder "/<name>" in the blogs content; its metadata lives in "/<name>/<name>.md".
import { useAsync, useBlade, useBladeForm, useLoading, usePopup, type IBladeToolbar } from "@vc-shell/framework";
import { VcBlade, VcCard, VcContainer, VcForm, VcInput } from "@vc-shell/framework/ui";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import FrontMatterFields from "../components/FrontMatterFields.vue";
import {
  buildFrontMatter,
  parseFrontMatter,
  readFile,
  renameBlog,
  saveFile,
  useContentClients,
  validateFolderName,
  type ContentFile,
  type FrontMatter,
} from "../api";

defineBlade({
  url: "/content-blog",
  name: "ContentBlogDetails",
  permissions: ["content:read"],
});

const { t } = useI18n({ useScope: "global" });
const { param, options, closeSelf, callParent } = useBlade<{ item?: ContentFile }>();
const { showConfirmation } = usePopup();
const clients = useContentClients();

const storeId = computed(() => param.value!);
const folder = ref<ContentFile | undefined>(options.value?.item);
const isNew = computed(() => !folder.value);
const item = ref<{ name: string; metadata: FrontMatter }>({ name: folder.value?.name ?? "", metadata: {} });
const existing = ref<string[]>([]);

const form = useBladeForm({ data: item, closeConfirmMessage: computed(() => t("CONTENT.ALERTS.CLOSE_CONFIRMATION")) });

const nameError = computed(() => {
  const name = item.value.name;
  if (!name) return undefined;
  if (validateFolderName(name)) return t("CONTENT.VALIDATION.FOLDER_NAME");
  if (name !== folder.value?.name && existing.value.includes(name)) return t("CONTENT.VALIDATION.BLOG_EXISTS");
  return undefined;
});

const blogFile = (name: string) => `/${name}/${name}.md`;

// the blogs (for the unique-name check) and this blog's metadata, side by side
const { loading: loadLoading, action: load } = useAsync(async () => {
  const name = folder.value?.name;
  const [blogs, raw] = await Promise.all([
    (await clients.content()).searchContent("blogs", storeId.value),
    name ? readFile("blogs", storeId.value, blogFile(name)).catch(() => "") : undefined,
  ]);
  existing.value = (blogs ?? []).map((x) => x.name!);
  if (name) item.value = { name, metadata: parseFrontMatter(raw ?? "").metadata };
  form.setBaseline();
});

const { loading: saveLoading, action: save } = useAsync(async () => {
  if (!item.value.name || nameError.value) return;
  const name = item.value.name;
  const oldName = folder.value?.name;
  if (oldName && oldName !== name) await renameBlog(await clients.content(), storeId.value, oldName, name);
  await saveFile("blogs", storeId.value, `/${name}`, `${name}.md`, buildFrontMatter(item.value.metadata, ""), true);
  folder.value = { ...folder.value, name, relativeUrl: `/${name}`, type: "folder" };
  form.setBaseline();
  await callParent("reload").catch(() => undefined);
  if (!oldName) await closeSelf();
});

const { loading: deleteLoading, action: deleteBlog } = useAsync(async () => {
  await (await clients.content()).deleteContent("blogs", storeId.value, [`/${folder.value!.name}`]);
  form.setBaseline();
  await callParent("reload").catch(() => undefined);
  await closeSelf();
});

const loading = useLoading(loadLoading, saveLoading, deleteLoading);

async function remove() {
  if (await showConfirmation(t("CONTENT.ALERTS.DELETE_BLOGS", { count: 1 }))) await deleteBlog();
}

const bladeToolbar = ref<IBladeToolbar[]>([
  {
    id: "save",
    title: computed(() => t(isNew.value ? "CONTENT.TOOLBAR.CREATE" : "CONTENT.TOOLBAR.SAVE")),
    icon: "lucide-save",
    // saving uploads the metadata file (UploadContent: content:create)
    permissions: ["content:create"],
    disabled: computed(() => !item.value.name || !!nameError.value || (!isNew.value && !form.canSave.value)),
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
