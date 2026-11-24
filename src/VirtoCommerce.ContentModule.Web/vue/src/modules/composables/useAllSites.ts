import type { GlobalFilterConfig } from "@vc-shell/framework";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";
import type { Store } from "../../api_client/virtocommerce.store";
import { useContentClients } from "../api";

/** The search bar's value for a single select field, or undefined when nothing is picked. */
export const pickedValue = (value: unknown) => (typeof value === "string" && value ? value : undefined);

/**
 * The sites, and which one a cross-site list (Blogs, Menus) shows: all of them, or the one picked in its search bar's Site filter.
 * `addTo` is where Add puts a new item: the picked site, or the only one there is.
 */
export function useAllSites() {
  const { t } = useI18n({ useScope: "global" });
  const clients = useContentClients();
  const sites = ref<Store[]>([]);
  const siteId = ref<string>();

  async function loadSites() {
    // ponytail: one page of 100, sites are few
    sites.value = (await (await clients.stores()).searchStores({ take: 100, sort: "name" })).results ?? [];
    return sites.value;
  }

  // a single select: Add puts a new item into the picked site. No filter when there is one site.
  const siteFilters = computed<GlobalFilterConfig[]>(() =>
    sites.value.length > 1
      ? [{ id: "siteId", label: t("CONTENT.SITES.FILTER"), filter: { options: sites.value.map((x) => ({ value: x.id!, label: x.name ?? x.id! })) } }]
      : [],
  );
  const onSiteFilter = (filters: Record<string, unknown>) => (siteId.value = pickedValue(filters.siteId));
  const shows = (storeId?: string) => !siteId.value || siteId.value === storeId;
  const addTo = computed(() => {
    if (siteId.value) return sites.value.find((x) => x.id === siteId.value);
    return sites.value.length === 1 ? sites.value[0] : undefined;
  });

  return { sites, siteFilters, onSiteFilter, loadSites, shows, addTo };
}
