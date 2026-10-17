<template>
  <VcBlade
    :title="isNew ? $t('CONTENT.PAGES.ASSET.TITLE_NEW') : file!.name"
    :subtitle="$t('CONTENT.PAGES.ASSET.SUBTITLE')"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="70%"
  >
    <VcContainer>
      <VcForm class="content-form tw-space-y-4">
        <VcInput
          v-if="isNew"
          v-model="item.name"
          :label="$t('CONTENT.PAGES.ASSET.FIELDS.NAME')"
          :placeholder="$t('CONTENT.PAGES.ASSET.FIELDS.NAME_PLACEHOLDER')"
          :error-message="nameError"
          required
        />
        <VcField
          v-else
          :label="$t('CONTENT.PAGES.PAGE.FIELDS.PATH')"
          :model-value="file!.relativeUrl"
          copyable
        />
        <div
          v-if="image"
          class="content-asset__image"
        >
          <img
            :src="file!.url"
            :alt="file!.name"
          />
        </div>
        <!-- no code editor component in vc-shell: a monospace textarea keeps Liquid/CSS/JS intact -->
        <VcTextarea
          v-else
          v-model="item.content"
          class="content-code"
          :label="$t('CONTENT.PAGES.ASSET.FIELDS.CONTENT')"
        />
      </VcForm>
    </VcContainer>
  </VcBlade>
</template>

<script setup lang="ts">
import { useAsync, useBlade, useBladeForm, useLoading, usePopup, type IBladeToolbar } from "@vc-shell/framework";
import { VcBlade, VcContainer, VcField, VcForm, VcInput, VcTextarea } from "@vc-shell/framework/ui";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { isImage, parentFolder, readFile, saveFile, useContentClients, type ContentFile } from "../api";

defineBlade({
  url: "/content-asset",
  name: "ContentAssetDetails",
  permissions: ["content:read"],
});

const { t } = useI18n({ useScope: "global" });
const { param, options, closeSelf, callParent } = useBlade<{ folderUrl?: string; item?: ContentFile }>();
const { showConfirmation } = usePopup();
const clients = useContentClients();

const storeId = computed(() => param.value!);
const file = ref<ContentFile | undefined>(options.value?.item);
const isNew = computed(() => !file.value);
const image = computed(() => !!file.value && isImage(file.value));
const item = ref({ name: "", content: "" });

const form = useBladeForm({ data: item, closeConfirmMessage: computed(() => t("CONTENT.ALERTS.CLOSE_CONFIRMATION")) });

const nameError = computed(() =>
  item.value.name && !/^[^\\/:*?"<>|]+\.[A-Za-z0-9-]+$/.test(item.value.name) ? t("CONTENT.VALIDATION.ASSET_NAME") : undefined,
);

const { loading: loadLoading, action: load } = useAsync(async () => {
  if (file.value && !image.value) {
    item.value = { name: file.value.name!, content: await readFile("themes", storeId.value, file.value.relativeUrl!, false) };
  }
  form.setBaseline();
});

const { loading: saveLoading, action: save } = useAsync(async () => {
  const folderUrl = file.value ? parentFolder(file.value.relativeUrl) : (options.value?.folderUrl ?? "");
  const [saved] = await saveFile("themes", storeId.value, folderUrl, item.value.name, item.value.content);
  const wasNew = isNew.value;
  file.value = { ...file.value, ...saved };
  form.setBaseline();
  await callParent("reload").catch(() => undefined);
  if (wasNew) await closeSelf();
});

const { loading: deleteLoading, action: deleteFile } = useAsync(async () => {
  await (await clients.content()).deleteContent("themes", storeId.value, [file.value!.relativeUrl!]);
  form.setBaseline();
  await callParent("reload").catch(() => undefined);
  await closeSelf();
});

const loading = useLoading(loadLoading, saveLoading, deleteLoading);

async function remove() {
  if (await showConfirmation(t("CONTENT.ALERTS.DELETE_FILES", { count: 1 }))) await deleteFile();
}

const bladeToolbar = ref<IBladeToolbar[]>([
  {
    id: "save",
    title: computed(() => t(isNew.value ? "CONTENT.TOOLBAR.CREATE" : "CONTENT.TOOLBAR.SAVE")),
    icon: "lucide-save",
    // saving uploads the file (UploadContent: content:create)
    permissions: ["content:create"],
    isVisible: computed(() => !image.value),
    disabled: computed(() => (isNew.value ? !item.value.name || !!nameError.value : !form.canSave.value)),
    clickHandler: () => save(),
  },
  {
    id: "reset",
    title: t("CONTENT.TOOLBAR.RESET"),
    icon: "lucide-undo-2",
    isVisible: computed(() => !isNew.value && !image.value),
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

<style lang="scss">
.content-asset__image {
  display: flex;
  justify-content: center;
  padding: 16px;
  border: 1px solid var(--color-neutral-200);
  border-radius: 6px;

  img {
    max-width: 100%;
    max-height: 60vh;
  }
}
</style>
