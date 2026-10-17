<template>
  <VcBlade
    :title="$t('CONTENT.SITE_TAB.FILES')"
    :subtitle="store?.name"
    width="50%"
  >
    <div class="site">
      <SiteFilesTab
        v-if="store"
        :store-id="param!"
        :store="store"
      />
    </div>
  </VcBlade>
</template>

<script setup lang="ts">
import { useAsync, useBlade } from "@vc-shell/framework";
import { VcBlade } from "@vc-shell/framework/ui";
import { onMounted, ref } from "vue";
import type { Store } from "../../api_client/virtocommerce.store";
import SiteFilesTab from "../widgets/SiteFilesTab.vue";
import { useContentClients } from "../api";

// a site's files (themes, file pages, blogs, link lists), from its Files widget
defineBlade({
  url: "/site-files",
  name: "SiteFiles",
  permissions: ["content:read"],
});

const { param, exposeToChildren, callParent } = useBlade();
const clients = useContentClients();

const store = ref<Store>();
const { action: load } = useAsync(async () => {
  store.value = await (await clients.stores()).getStoreById(param.value!);
});
onMounted(load);

// a child saved: the store is re-read (the counts follow it), then the site page
exposeToChildren({
  async reload() {
    await Promise.all([load(), callParent("reload").catch(() => undefined)]);
  },
});
</script>
