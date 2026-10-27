import { computed, defineComponent, h, markRaw } from "vue";
import { useI18n } from "vue-i18n";
import { injectBladeContext, useBlade } from "@vc-shell/framework";
import { VcWidget } from "@vc-shell/framework/ui";

/** A widget on a site page (SiteDetails) that opens a blade for the site. */
function siteWidget(icon: string, title: string, blade: string) {
  return markRaw(
    defineComponent({
      setup() {
        const { t } = useI18n({ useScope: "global" });
        // the store shared by SiteDetails via defineBladeContext({ item })
        const ctx = injectBladeContext();
        const storeId = computed(() => (ctx.value.item as { id?: string } | undefined)?.id);
        const { openBlade } = useBlade();
        return () =>
          h(VcWidget, {
            icon,
            title: t(title),
            disabled: !storeId.value,
            onClick: () => storeId.value && openBlade({ name: blade, param: storeId.value }),
          });
      },
    }),
  );
}

export const SiteFilesWidget = siteWidget("lucide-folder-tree", "CONTENT.SITE_TAB.FILES", "SiteFiles");
export const SiteSettingsWidget = siteWidget("lucide-sliders-horizontal", "CONTENT.PAGES.SITE_SETTINGS.TITLE", "SiteSettings");
