<template>
  <div class="container">
    <div class="header">
      <h1>Link Performance</h1>
       <router-link :to="{ name: 'Links' }" class="btn btn-primary">Back to Links</router-link>
    </div>

    <div class="link-url">

      <h3>{{ url }}</h3>

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
  data() {
    return {
      baseUrl : import.meta.env.VITE_APP_BASEURL + '/Crawler',
      linkPerformances : [] as LinkPerformance[],
      url : '',
      error : ''
    };
  },
  created() {
    this.getLinkPerformances();
  },
  methods: {
     async getLinkPerformances() {
       this.error = '';

       try {
         const response = await axios.get(`${this.baseUrl}/links/` + `${this.id}/performance`);

         const data = response.data;

         Object.assign(this, {
            linkPerformances: data.linkPerformances,
            url: data.url,
          });
       }
       catch (e) {
         this.error = 'Error occurred while crawling the website.';
       }
     }
  },
});
</script>