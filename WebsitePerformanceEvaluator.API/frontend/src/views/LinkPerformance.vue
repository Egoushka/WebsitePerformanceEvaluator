<template>
  <div class="container">
    <div class="header">
      <h1>Link Performance</h1>
      <router-link :to="{ name: 'Links' }">Back to Links</router-link>
    </div>

    <div class="link-url">

    </div>

    <div>
      <table class="table">
        <thead>
        <tr>
          <th>URL</th>
          <th>Source</th>
          <th>Response Time</th>
        </tr>
        </thead>
        <tbody>
          <tr v-for="link in linkPerformances" :key="link.url">
            <td>{{ link.url }}</td>
            <td>{{ link.crawlingLinkSource }}</td>
            <td>{{ link.timeResponseMs }}</td>

          </tr>
        </tbody>

      </table>
    </div>

    <div v-if="linkPerformances.some(link => link.crawlingLinkSource === CrawlingLinkSource.SiteMap)">
      <h3>URL not found at website</h3>
      <table class="table">
        <thead>
        <tr>
          <th>URL</th>
        </tr>
        </thead>
        <tbody>
        <tr v-for="link in linkPerformances.filter(link => link.crawlingLinkSource === CrawlingLinkSource.SiteMap)"
            :key="link.url">
          <td>{{ link.url }}</td>
        </tr>
        </tbody>
      </table>
    </div>

    <div v-if="linkPerformances.some(link => link.crawlingLinkSource === CrawlingLinkSource.Website)">
      <h3>URL found at website</h3>
      <table class="table">
        <thead>
        <tr>
          <th>URL</th>
        </tr>
        </thead>
        <tbody>
        <tr v-for="link in linkPerformances.filter(link => link.crawlingLinkSource === CrawlingLinkSource.Website)"
            :key="link.url">
          <td>{{ link.url }}</td>
        </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref } from 'vue'
import { LinkPerformance } from '@/models/LinkPerformance'
import axios from 'axios'
import { CrawlingLinkSource } from '@/models/CrawlingLinkSource'

export default defineComponent({
  computed: {
    CrawlingLinkSource() {
      return CrawlingLinkSource
    }
  },
  props: ['id'],
  setup() {
    const baseUrl = "https://localhost:7147/api/Crawler";
    const linkPerformances = ref<LinkPerformance[]>([]);
    const isMakingRequest = ref(false);
    const error = ref('');

    return {
      linkPerformances,
      isMakingRequest,
      baseUrl,
      error,
    };
  },
  created() {
    this.getLinkPerformances();
    console.log(this.linkPerformances)
  },
  methods: {
     async getLinkPerformances() {
       this.isMakingRequest = true;
       this.error = '';
       try {
         console.log(this.id)

         const response = await axios.get(`${this.baseUrl}/links/` + `${this.id}/performance`);

         const data = response.data;

         this.linkPerformances = data.linkPerformances;

         console.log(data)

       } catch (e) {
         this.error = 'Error occurred while crawling the website.';
       } finally {
         this.isMakingRequest = false;
       }
     }
  },
});
</script>