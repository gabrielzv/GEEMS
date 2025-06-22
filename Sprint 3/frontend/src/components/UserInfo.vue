<template>
  <div class="flex flex-col" role="group" :aria-label="label">
    <span class="text-xl font-semibold text-gray-700">{{ label }}</span>
    <span class="text-xl text-gray-900">{{ value }}</span>
  </div>
</template>

<script>
export default {
  name: "UserInfo",
  props: {
    label: {
      type: String,
      required: true,
    },
    value: {
      type: String,
      required: true,
    },
  },
};
</script>
