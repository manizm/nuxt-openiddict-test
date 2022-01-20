<template>
  <div>
<button @click="login">Login</button>

  </div>
</template>

<script>
export default {
  name: 'IndexPage',
  auth: false,
  methods: {
    login() {
      console.log(this.$store);
      console.log(this.$auth);
      this.$auth.loginWith('oidc');
    }
  }
}
</script>
