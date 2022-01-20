<template>
  <div>
login...

  </div>
</template>

<script>
export default {
  name: 'IndexPage',
  auth: true,
  mounted() {
    // if(!this.$auth.loggedIn){
    //   this.$auth.loginWith('oidc')
    // }
    // else {
    //   this.$router.push('/')
    // }
    console.log(this.$auth);
  }
}
</script>
