import * as blades from "./pages";
import * as locales from "./locales";
import { defineAppModule, registerExternalWidget } from "@vc-shell/framework";
import { markRaw } from "vue";
import StoreContentWidget from "./widgets/StoreContentWidget.vue";
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

export default defineAppModule({
  blades,
  locales,
});

export * from "./pages";
