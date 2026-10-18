import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";
import type { Store } from "../../api_client/virtocommerce.store";
import { useContentClients } from "../api";

const ALL = "";

/**
 * The sites, and which one a cross-site list (Blogs, Menus) shows: all of them, or one picked in its Site filter.
 * `addTo` is where Add puts a new item: the picked site, or the only one there is.
 */
export function useAllSites() {
  const { t } = useI18n({ useScope: "global" });
  const clients = useContentClients();
  const sites = ref<Store[]>([]);
  const siteId = ref(ALL);

  async function loadSites() {
    // ponytail: one page of 100, sites are few
    sites.value = (await (await clients.stores()).searchStores({ take: 100, sort: "name" })).results ?? [];
    return sites.value;
  }

  const siteOptions = computed(() => [{ id: ALL, name: t("CONTENT.SITES.ALL") }, ...sites.value]);
  const shows = (storeId?: string) => siteId.value === ALL || siteId.value === storeId;
  const addTo = computed(() => {
    if (siteId.value !== ALL) return sites.value.find((x) => x.id === siteId.value);
    return sites.value.length === 1 ? sites.value[0] : undefined;
  });

  return { sites, siteId, siteOptions, loadSites, shows, addTo };
}
