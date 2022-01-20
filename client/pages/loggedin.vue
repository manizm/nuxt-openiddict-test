<template>
  <div>
Logged In yo!

  </div>
</template>

<script>
export default {
  name: 'IndexPage',
  auth: true,
  mounted() {
    console.log(this);
  }
}
</script>
