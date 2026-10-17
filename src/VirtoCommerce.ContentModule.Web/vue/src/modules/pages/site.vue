<template>
  <VcBlade
    :title="store?.name ?? param"
    :subtitle="subtitle"
    :toolbar-items="contentInstance?.toolbar ?? []"
    :loading="loading && !store"
    width="70%"
  >
    <div
      v-if="store"
      class="site"
    >
      <component
        :is="content"
        ref="contentInstance"
        :store-id="param"
        :store="store"
      />
    </div>
  </VcBlade>
</template>

<script setup lang="ts">
import { WidgetServiceKey, defineBladeContext, useAsync, useBlade, type IBladeToolbar } from "@vc-shell/framework";
import { VcBlade } from "@vc-shell/framework/ui";
import { computed, inject, markRaw, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import type { Store } from "../../api_client/virtocommerce.store";
import SiteFilesTab from "../widgets/SiteFilesTab.vue";
import { siteHost, siteState, storeDefaultTheme, useContentClients } from "../api";

defineBlade({
  url: "/site",
  name: "SiteDetails",
  permissions: ["content:read"],
});

/**
 * A site's page. Its content comes from a module: registerExternalWidget({ id, title, component,
 * targetBlades: ["SiteDetails:content"] }), today the Page Builder's pages. The component gets the store-id and store
 * props, and defineExpose({ toolbar }) (IBladeToolbar items, or a ref of them) fills this blade's toolbar. Without
 * one, the site's files are the content. Everything else about the site is a widget (targetBlades: ["SiteDetails"]),
 * which reads the store from injectBladeContext().item.
 */
const CONTENT_TARGET = "SiteDetails:content";

const { t } = useI18n({ useScope: "global" });
const { param, exposeToChildren, callParent } = useBlade();
const clients = useContentClients();

const store = ref<Store>();
defineBladeContext({ item: store });

const { loading, action: load } = useAsync(async () => {
  store.value = await (await clients.stores()).getStoreById(param.value!);
});

// the URL and state under the name (the name is the title)
const subtitle = computed(() =>
  store.value ? [siteHost(store.value.url) || t("CONTENT.PAGES.STORES.NO_URL"), t(siteState(store.value).label)].join(" · ") : undefined,
);

// a registration without targets is offered to every blade: only the ones meant for the site's content count
const content =
  (inject(WidgetServiceKey)?.getExternalWidgetsForBlade(CONTENT_TARGET) ?? []).find((x) =>
    x.targetBlades?.includes(CONTENT_TARGET.toLowerCase()),
  )?.component ?? markRaw(SiteFilesTab);
const contentInstance = ref<{ toolbar?: IBladeToolbar[] }>();

onMounted(load);

// A child changed this site (its settings, a theme, a page). The store is re-read (content watching its store prop
// refreshes with it), then the opener: the Sites gallery re-reads the tile, the Store module's store blade (this blade
// opened from its widget) its widget badges, and its dynamic properties when the active theme changed. A parent
// without the method ignores the call.
exposeToChildren({
  async reload() {
    const theme = storeDefaultTheme(store.value);
    await Promise.all([
      load(),
      callParent("refreshStore", param.value).catch(() => undefined),
      callParent("reloadWidgets").catch(() => undefined),
    ]);
    if (storeDefaultTheme(store.value) !== theme) await callParent("refreshPart", "dynamicProperties").catch(() => undefined);
  },
});
</script>

<style lang="scss">
.site {
  padding: 20px 24px 36px;
}
</style>
