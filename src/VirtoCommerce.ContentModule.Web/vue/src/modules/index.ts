import * as blades from "./pages";
import * as locales from "./locales";
import { defineAppModule, getBladeConfig, registerExternalWidget } from "@vc-shell/framework";
import { markRaw } from "vue";
import StoreContentWidget from "./widgets/StoreContentWidget.vue";
import { SiteFilesWidget, SiteSettingsWidget } from "./widgets/siteWidgets";
import "./styles.css";

// The AngularJS module adds a CMS widget to the store blade; same here, by blade name.
registerExternalWidget({
  id: "StoreContent",
  title: "Content",
  component: markRaw(StoreContentWidget),
  targetBlades: ["StoreDetails"],
  // not on the "New store" blade (no param): there is no saved store to have content yet
  isVisible: (blade) => !!blade?.param,
});

// a site page's widgets: its files (when the Page Builder's pages are the page's content, else the files are) and settings
registerExternalWidget({
  id: "SiteFiles",
  title: "Files",
  component: SiteFilesWidget,
  targetBlades: ["SiteDetails"],
  isVisible: () => !!getBladeConfig("PageBuilderPages"),
});
registerExternalWidget({ id: "SiteSettings", title: "Settings", component: SiteSettingsWidget, targetBlades: ["SiteDetails"] });

export default defineAppModule({
  blades,
  locales,
});

export * from "./pages";
