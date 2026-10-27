<template>
  <!-- ponytail: a storefront mock in the site's colours; a real screenshot (cover image or captured) replaces it later -->
  <span
    class="site-preview"
    aria-hidden="true"
    :style="{ background: brand.paper }"
  >
    <span class="site-preview__bar">
      <span
        class="site-preview__logo"
        :style="{ background: brand.color }"
      />
      <span class="site-preview__line site-preview__line--nav" />
      <span class="site-preview__grow" />
      <span class="site-preview__search" />
    </span>
    <span class="site-preview__body">
      <span class="site-preview__copy">
        <span class="site-preview__line site-preview__line--head" />
        <span class="site-preview__line site-preview__line--head site-preview__line--short" />
        <span class="site-preview__line site-preview__line--text" />
        <span
          class="site-preview__cta"
          :style="{ background: brand.color }"
        />
      </span>
      <span
        class="site-preview__image"
        :style="{ background: brand.tint }"
      />
    </span>
  </span>
</template>

<script setup lang="ts">
import { computed } from "vue";
import type { Store } from "../../api_client/virtocommerce.store";
import { siteBrand } from "../api";

const props = defineProps<{ store?: Store }>();
const brand = computed(() => siteBrand(props.store));
</script>

<style lang="scss">
/* the mock is a light storefront in either theme, like a screenshot would be */
.site-preview {
  aspect-ratio: 16 / 10;
  border-radius: 16px;
  border: 1px solid var(--neutrals-200);
  overflow: hidden;
  display: flex;
  flex-direction: column;
  transition: border-color 0.15s;

  &__bar {
    display: flex;
    align-items: center;
    gap: 8px;
    padding: 10px 14px;
    background: #fff;
    border-bottom: 1px solid #ece7e1;
  }

  &__logo {
    width: 18px;
    height: 18px;
    border-radius: 5px;
  }

  &__grow {
    flex: 1;
  }

  &__search {
    width: 90px;
    height: 14px;
    border-radius: 999px;
    border: 1px solid #e3dbd1;
  }

  &__body {
    display: flex;
    gap: 12px;
    padding: 16px 14px;
    flex: 1;
  }

  &__copy {
    flex: 1;
    display: flex;
    flex-direction: column;
    gap: 7px;
    padding-top: 6px;
  }

  &__line {
    height: 12px;
    border-radius: 4px;
    background: #3a3129;
    width: 80%;

    &--nav {
      width: 60px;
      height: 8px;
      background: #d9d2c9;
    }

    &--short {
      width: 60%;
    }

    &--text {
      width: 70%;
      height: 7px;
      background: #cfc6bb;
    }
  }

  &__cta {
    width: 54px;
    height: 16px;
    border-radius: 999px;
    margin-top: 6px;
  }

  &__image {
    flex: 1;
    border-radius: 10px;
  }
}
</style>
