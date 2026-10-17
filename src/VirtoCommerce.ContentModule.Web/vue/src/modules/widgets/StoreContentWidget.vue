<template>
  <VcWidget
    icon="lucide-code-xml"
    :title="$t('CONTENT.WIDGETS.STORE_CONTENT')"
    :value="pages"
    :disabled="!storeId"
    @click="open"
  />
</template>

<script setup lang="ts">
// The AngularJS storeCMSWidget on the store blade: content of this store (pages count as the badge).
import { computed, ref, watch } from "vue";
import { injectBladeContext, useApiClient, useBlade, useWidgetTrigger } from "@vc-shell/framework";
import { VcWidget } from "@vc-shell/framework/ui";
import { ContentClient } from "../../api_client/virtocommerce.content";

// the store shared by the Store blade via defineBladeContext({ item })
const ctx = injectBladeContext();
const storeId = computed(() => (ctx.value.item as { id?: string } | undefined)?.id);
const pages = ref<number>();

const { getApiClient } = useApiClient(ContentClient);
async function load() {
  if (!storeId.value) return;
  // no stats (e.g. the store isn't saved yet) = no content
  const stats = await (await getApiClient()).getStoreContentStats(storeId.value).catch(() => undefined);
  pages.value = Math.max(stats?.pagesCount ?? 0, 0);
}
watch(storeId, load, { immediate: true });
// the store blade refreshes its widgets, e.g. after a change in the content blades opened from here
useWidgetTrigger({ onRefresh: load });

const { openBlade } = useBlade();
function open() {
  if (storeId.value) openBlade({ name: "SiteDetails", param: storeId.value });
}
</script>
