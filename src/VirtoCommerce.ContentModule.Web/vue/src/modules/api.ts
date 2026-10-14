import YAML from "yaml";
import type { Ref } from "vue";
import { DynamicPropertiesClient, useApiClient } from "@vc-shell/framework";
import { ContentClient, MenuClient, type ContentItem } from "../api_client/virtocommerce.content";
import { StoreModuleClient, type Store } from "../api_client/virtocommerce.store";

export type ContentType = "pages" | "blogs" | "themes";

/** What the search endpoints really return (the generated ContentItem lacks the ContentFile fields). */
export interface ContentFile extends ContentItem {
  mimeType?: string;
  size?: string;
  published?: boolean;
  hasChanges?: boolean;
}

export function useContentClients() {
  return {
    content: useApiClient(ContentClient).getApiClient,
    menu: useApiClient(MenuClient).getApiClient,
    stores: useApiClient(StoreModuleClient).getApiClient,
    dynamicProperties: useApiClient(DynamicPropertiesClient).getApiClient,
  };
}

// The generated client can't do these two: the GET returns the raw file (not JSON) and the
// POST names the stored file after the multipart field, as the AngularJS resource does.
async function check(res: Response) {
  if (!res.ok) throw new Error(`${res.status} ${res.statusText}: ${await res.text()}`);
  return res;
}

const contentUrl = (contentType: string, storeId: string) =>
  `/api/content/${encodeURIComponent(contentType)}/${encodeURIComponent(storeId)}`;

/** Reads a file; with `draft` the unpublished copy wins when there is one. */
export async function readFile(contentType: string, storeId: string, relativeUrl: string, draft = true) {
  const query = new URLSearchParams({ relativeUrl, draft: String(draft) });
  const res = await check(await fetch(`${contentUrl(contentType, storeId)}?${query}`));
  return res.text();
}

/** Writes one file into `folderUrl`; with `draft` it lands as "<name>-draft" until published. */
export async function saveFile(
  contentType: string,
  storeId: string,
  folderUrl: string,
  fileName: string,
  data: Blob | string,
  draft = false,
): Promise<ContentFile[]> {
  const body = new FormData();
  body.append(fileName, data instanceof Blob ? data : new Blob([data]), fileName);
  const query = new URLSearchParams({ folderUrl, draft: String(draft) });
  const res = await check(await fetch(`${contentUrl(contentType, storeId)}?${query}`, { method: "POST", body }));
  return res.json();
}

/** Lets the server download a file from `url` into `folderUrl` (the AngularJS "upload from URL"). */
export async function uploadFromUrl(contentType: string, storeId: string, folderUrl: string, url: string): Promise<ContentFile[]> {
  const query = new URLSearchParams({ folderUrl, url });
  const res = await check(await fetch(`${contentUrl(contentType, storeId)}?${query}`, { method: "POST" }));
  return res.json();
}

/** The store the blade was given by its opener, else read by id. */
export async function ensureStore(clients: ReturnType<typeof useContentClients>, storeId: string, store: Ref<Store | undefined>) {
  if (!store.value) store.value = await (await clients.stores()).getStoreById(storeId);
}

/** A content count for display: the server reports -1 for a content type whose folder doesn't exist yet. */
export const count = (value?: number) => (value === undefined ? "…" : Math.max(value, 0));

export function storeDefaultTheme(store?: Store) {
  const values = store?.dynamicProperties?.find((x) => x.name === "DefaultThemeName")?.values;
  return (values?.[0]?.value as string | undefined) || undefined;
}

/** Sets the store's DefaultThemeName dynamic property (the active theme) and saves the store. */
export async function setStoreDefaultTheme(clients: ReturnType<typeof useContentClients>, storeId: string, themeName: string) {
  const stores = await clients.stores();
  const store = await stores.getStoreById(storeId);
  store.dynamicProperties ??= [];
  let prop = store.dynamicProperties.find((x) => x.name === "DefaultThemeName");
  if (!prop) {
    const definitions = await (await clients.dynamicProperties()).searchDynamicProperties({
      objectType: store.objectType ?? "VirtoCommerce.StoreModule.Core.Model.Store",
      take: 100,
    });
    const definition = definitions.results?.find((x) => x.name === "DefaultThemeName");
    if (!definition) throw new Error("The store has no DefaultThemeName property");
    prop = { ...definition, objectId: store.id, values: [] } as NonNullable<typeof prop>;
    store.dynamicProperties.push(prop);
  }
  prop!.values = themeName ? [{ value: themeName, objectId: store.id, objectType: store.objectType }] : [];
  await stores.updateStore(store);
}

// ---- files ----

export const trimSlashes = (value?: string) => (value ?? "").replace(/^\/+|\/+$/g, "");
export const undraft = (url?: string) => (url?.endsWith("-draft") ? url.slice(0, -6) : url);
export const isFolder = (item: ContentFile) => item.type === "folder";
export const isDesignedPage = (item: ContentFile) => /\.page(-draft)?$/.test(item.name ?? "");
export const isImage = (item: ContentFile) => !!item.mimeType?.startsWith("image/");
/** Files the text editors open (same rule as the AngularJS markdown handler). */
export const isText = (item: ContentFile) =>
  /\.(md|md-draft|liquid|html|htm|css|scss|js|json|txt|xml|svg|yml|yaml)$/i.test(item.name ?? "") ||
  !!item.mimeType?.startsWith("text/") ||
  !!item.mimeType?.startsWith("application/j");

/** Folder that holds `url` ("/a/b/c.md" -> "/a/b"). */
export const parentFolder = (url?: string) => (url ?? "").replace(/\/[^/]*$/, "");

/** VcBreadcrumbs items from `root` down to `folderUrl`; a crumb's id is its folder url, which a click navigates to. */
export function folderCrumbs(root: string, folderUrl: string, rootTitle: string, navigate: (url: string) => void) {
  const crumbs = [{ id: root, title: rootTitle, clickHandler: navigate }];
  let url = root;
  for (const part of trimSlashes(folderUrl.slice(root.length)).split("/").filter(Boolean)) {
    url = `${url}/${part}`;
    crumbs.push({ id: url, title: part, clickHandler: navigate });
  }
  return crumbs;
}

/** Front matter belongs to page files only: json, svg, xml, yml... stay as typed. */
export const hasFrontMatter = (extension: string) => /^(md|html?|liquid)$/i.test(extension);

/** A blog is "/<name>" with its metadata in "/<name>/<name>.md": a rename moves both. */
export async function renameBlog(client: ContentClient, storeId: string, oldName: string, name: string) {
  await client.moveContent("blogs", storeId, `/${oldName}`, `/${name}`);
  await client.moveContent("blogs", storeId, `/${name}/${oldName}.md`, `/${name}/${name}.md`);
}

// ---- page names: "<page>.<language>.md" ----

export function parsePageName(fileName: string, languages: string[]) {
  const parts = undraft(fileName)!.split(".");
  const extension = parts.length > 1 ? parts.pop()! : "md";
  let language: string | undefined;
  const candidate = parts.length > 1 ? parts[parts.length - 1] : "";
  const match = languages.find((x) => x.toLowerCase() === candidate.toLowerCase());
  if (match) {
    parts.pop();
    language = match;
  }
  return { pageName: parts.join("."), language, extension };
}

export const buildPageName = (pageName: string, language: string | undefined, extension: string) =>
  [pageName, language, extension].filter(Boolean).join(".");

// ---- front matter ----

export type FrontMatter = Record<string, unknown>;

export function parseFrontMatter(raw: string): { metadata: FrontMatter; content: string } {
  const match = /^\s*---\r?\n([\s\S]*?)\r?\n---[ \t]*(?:\r?\n|$)/.exec(raw);
  if (!match) return { metadata: {}, content: raw };
  let metadata: FrontMatter = {};
  try {
    metadata = (YAML.parse(match[1]) as FrontMatter) ?? {};
  } catch {
    // unreadable front matter: keep the text, show no metadata
    return { metadata: {}, content: raw };
  }
  return { metadata, content: raw.slice(match[0].length).trim() };
}

/** Same output as the AngularJS saveWithMetadata: only non-empty values, permalink starts with "/". */
export function buildFrontMatter(metadata: FrontMatter, content: string) {
  const values = Object.fromEntries(
    Object.entries(metadata).filter(([, v]) => (Array.isArray(v) ? v.length > 0 : v !== undefined && v !== null && v !== "")),
  );
  if (typeof values.permalink === "string" && !values.permalink.startsWith("/")) values.permalink = "/" + values.permalink;
  const header = Object.keys(values).length ? YAML.stringify(values) : "";
  return `---\n${header}\n---\n${(content ?? "").trim()}`;
}

/** Opens the Page Builder module's app for a store (what that module's own store widget does). */
export function openPageBuilder(storeId: string) {
  const prefix = window.location.pathname.replace(/\/apps\/.*$/, "").replace(/\/$/, "");
  window.open(`${window.location.origin}${prefix}/apps/page-builder-shell/?storeId=${encodeURIComponent(storeId)}`, "_blank");
}

/** The server's folder (and theme) name rule: 3-63 of [0-9a-z -], no leading, trailing or double dash. */
export const validateFolderName = (name: string) =>
  !/^[0-9a-z -]{3,63}$/.test(name) || name.startsWith("-") || name.endsWith("-") || name.includes("--");
