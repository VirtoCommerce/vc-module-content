<template>
  <VcBlade
    :title="$t('CONTENT.PAGES.STORES.TITLE')"
    :loading="loading"
    :width="380"
  >
    <div class="sites">
      <p
        v-if="!loading && !stores.length"
        class="sites__empty"
      >
        {{ $t("CONTENT.PAGES.STORES.EMPTY") }}
      </p>
      <div class="sites__grid">
        <div
          v-for="store in stores"
          :key="store.id"
          class="site-tile"
          :class="{ 'site-tile--active': store.id === activeChildParam, 'site-tile--closed': store.storeState === 'Closed' }"
        >
          <button
            type="button"
            class="site-tile__open"
            :aria-label="store.name"
            @click="openSite(store.id!)"
          >
            <SitePreview :store="store" />
          </button>
          <span class="site-tile__meta">
            <span class="site-tile__text">
              <button
                type="button"
                class="site-tile__name"
                @click="openSite(store.id!)"
              >
                {{ store.name }}
              </button>
              <a
                v-if="store.url"
                class="site-tile__host site-tile__host--link"
                :href="store.url"
                target="_blank"
                rel="noopener"
                :title="$t('CONTENT.PAGES.SITE.VIEW_SITE')"
              >
                {{ siteHost(store.url) }}
                <VcIcon
                  icon="lucide-external-link"
                  size="xs"
                />
              </a>
              <span
                v-else
                class="site-tile__host"
                >{{ $t("CONTENT.PAGES.STORES.NO_URL") }}</span
              >
            </span>
            <SiteState :store="store" />
          </span>
        </div>
      </div>
    </div>
  </VcBlade>
</template>

<script setup lang="ts">
import { useAsync, useBlade } from "@vc-shell/framework";
import { VcBlade, VcIcon } from "@vc-shell/framework/ui";
import { onMounted, ref } from "vue";
import type { Store } from "../../api_client/virtocommerce.store";
import SitePreview from "../components/SitePreview.vue";
import SiteState from "../components/SiteState.vue";
import { siteHost, useContentClients } from "../api";

defineBlade({
  url: "/content",
  name: "ContentStores",
  isWorkspace: true,
  permissions: ["content:access"],
  menuItem: { title: "CONTENT.MENU.TITLE", icon: "lucide-globe", priority: 20 },
});

const { openBlade, query, exposeToChildren, activeChildParam } = useBlade();
const clients = useContentClients();

const stores = ref<Store[]>([]);

const { loading, action: load } = useAsync(async () => {
  // ponytail: one page of 100 and no search, sites are few; add both if a tenant runs many
  stores.value = (await (await clients.stores()).searchStores({ take: 100, sort: "name" })).results ?? [];
});

function openSite(storeId: string) {
  openBlade({ name: "SiteDetails", param: storeId });
}

onMounted(async () => {
  await load();
  // /content?storeId=… (the AngularJS workspace link) opens that site straight away
  if (query.value?.storeId) openSite(query.value.storeId);
});

// a site's settings changed: re-read that tile only
exposeToChildren({
  async refreshStore(storeId: string) {
    const store = await (await clients.stores()).getStoreById(storeId);
    const i = stores.value.findIndex((x) => x.id === storeId);
    if (i >= 0) stores.value[i] = store;
  },
});
</script>

<style lang="scss">
.sites {
  padding: 24px 28px 40px;
  display: flex;
  flex-direction: column;
  gap: 24px;

  &__empty {
    margin: 0;
    color: var(--neutrals-500);
  }

  &__grid {
    display: grid;
    /* tiles keep their size: one site doesn't stretch across the blade */
    grid-template-columns: repeat(auto-fill, minmax(min(260px, 100%), 320px));
    gap: 28px 24px;
  }
}

.site-tile {
  display: flex;
  flex-direction: column;
  gap: 12px;
  color: var(--neutrals-900);

  &--closed .site-preview {
    opacity: 0.55;
  }

  &__open,
  &__name {
    padding: 0;
    border: 0;
    background: none;
    color: inherit;
    font: inherit;
    text-align: left;
    cursor: pointer;
  }

  &__open {
    display: block;

    &:focus-visible {
      outline: none;
    }
  }

  &__open:hover .site-preview,
  &__open:focus-visible .site-preview,
  &--active .site-preview {
    border-color: var(--primary-500);
  }

  &__meta {
    display: flex;
    align-items: flex-start;
    gap: 10px;
  }

  &__text {
    flex: 1;
    min-width: 0;
    display: flex;
    flex-direction: column;
    align-items: flex-start;
  }

  &__name {
    font-size: 16px;
    font-weight: 600;
  }

  &__host {
    max-width: 100%;
    font-size: 13px;
    color: var(--neutrals-500);
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;

    &--link {
      display: inline-flex;
      align-items: center;
      gap: 4px;
      text-decoration: none;

      &:hover {
        color: var(--primary-500);
      }
    }
  }
}
</style>
