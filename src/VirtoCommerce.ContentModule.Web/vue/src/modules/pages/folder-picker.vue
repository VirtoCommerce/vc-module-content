<template>
  <VcBlade
    :title="$t('CONTENT.PAGES.MOVE.TITLE')"
    :subtitle="$t('CONTENT.PAGES.MOVE.SUBTITLE', { count: moving.length })"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="40%"
  >
    <VcDataTable
      data-key="relativeUrl"
      :items="folders"
      :column-switcher="false"
      :empty-state="{ icon: 'lucide-folder', title: $t('CONTENT.PAGES.MOVE.NO_FOLDERS') }"
      @row-click="(e: { data: ContentFile }) => navigate(e.data.relativeUrl!)"
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
        :title="$t('CONTENT.PAGES.MOVE.FOLDER')"
        always-visible
      >
        <template #body="{ data }">
          <span class="content-file">
            <VcIcon
              icon="lucide-folder"
              size="s"
            />
            <span>{{ data.name }}</span>
          </span>
        </template>
      </VcColumn>
    </VcDataTable>
  </VcBlade>
</template>

<script setup lang="ts">
// The AngularJS "paste mode" of the pages list: browse to a folder, then move the selected items there.
import { useAsync, useBlade, useLoading, type IBladeToolbar } from "@vc-shell/framework";
import { VcBlade, VcBreadcrumbs, VcColumn, VcDataTable, VcIcon } from "@vc-shell/framework/ui";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { folderCrumbs, isFolder, undraft, useContentClients, type ContentFile, type ContentType } from "../api";

defineBlade({
  url: "/content-move",
  name: "ContentFolderPicker",
  // the move endpoint needs content:update
  permissions: ["content:update"],
});

const { t } = useI18n({ useScope: "global" });
const { param, options, closeSelf, callParent } = useBlade<{
  contentType?: ContentType;
  /** browsing can't go above this folder (a theme's root) */
  root?: string;
  rootTitle?: string;
  /** the folder the items are in now */
  from?: string;
  items?: ContentFile[];
}>();
const clients = useContentClients();

const contentType = computed<ContentType>(() => options.value?.contentType ?? "pages");
const root = computed(() => options.value?.root ?? "");
const moving = computed(() => options.value?.items ?? []);
const folderUrl = ref(root.value);
const folders = ref<ContentFile[]>([]);

const crumbs = computed(() => folderCrumbs(root.value, folderUrl.value, options.value?.rootTitle ?? t("CONTENT.PAGES.FILES.ROOT"), navigate));

/** a folder can't go into itself or below itself, and items can't "move" to where they are */
const movingFolders = computed(() => moving.value.filter(isFolder).map((x) => x.relativeUrl!));
const canMoveHere = computed(
  () =>
    !!moving.value.length &&
    folderUrl.value !== (options.value?.from ?? "") &&
    !movingFolders.value.some((f) => folderUrl.value === f || folderUrl.value.startsWith(f + "/")),
);

const { loading: listLoading, action: load } = useAsync(async () => {
  const items = (await (await clients.content()).searchContent(contentType.value, param.value!, folderUrl.value || undefined)) ?? [];
  folders.value = items.filter(isFolder);
});

function navigate(url: string) {
  folderUrl.value = url;
  load();
}

const { loading: moveLoading, action: moveHere } = useAsync(async () => {
  const client = await clients.content();
  // all at once, as the AngularJS paste did
  await Promise.all(
    moving.value.map((item) => client.moveContent(contentType.value, param.value!, undraft(item.relativeUrl), `${folderUrl.value}/${undraft(item.name)!}`)),
  );
  await callParent("reload").catch(() => undefined);
  await closeSelf();
});

const loading = useLoading(listLoading, moveLoading);

const bladeToolbar = ref<IBladeToolbar[]>([
  {
    id: "moveHere",
    title: t("CONTENT.PAGES.MOVE.MOVE_HERE"),
    icon: "lucide-folder-input",
    disabled: computed(() => !canMoveHere.value),
    clickHandler: () => moveHere(),
  },
]);

onMounted(load);
</script>
