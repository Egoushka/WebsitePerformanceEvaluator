<template>
  <div class="container">
    <h1>Links</h1>
    <form @submit.prevent="crawlUrl">
      <div class="form-group">
        <label for="url">Enter a URL:</label>
        <input type="text" class="form-control" name="url" v-model="inputUrl" required />
        <span v-if="error" class="text-danger">{{ error }}</span>
      </div>
      <button type="submit" class="btn btn-primary" :disabled="isMakingRequest">Crawl</button>
    </form>

    <div v-if="links.length">
      <table class="table">
        <thead>
        <tr>
          <th>URL</th>
          <th>Date</th>
          <th></th>
        </tr>
        </thead>
        <tbody>
        <tr v-for="link in links" :key="link.id">
          <td>{{ link.url }}</td>
          <td>{{ formatDate(link.createdAt)  }}</td>
          <td>
            <router-link :to="{ name: 'LinkPerformance', params: { id: link.id } }">View</router-link>
          </td>
        </tr>
        </tbody>
      </table>

      <nav aria-label="Page navigation">
        <ul class="pagination justify-content-center">
          <li class="page-item" :class="{ disabled: currentPageIndex === 1 }">
            <button class="page-link" @click="getUrls(currentPageIndex - 1, pageSize.value)">&laquo; Previous</button>
          </li>
          <li class="page-item" v-for="pageIndex in totalPages" :key="pageIndex" :class="{ active: currentPageIndex === pageIndex }">
            <button class="page-link" @click="getUrls(pageIndex, pageSize.value)">{{ pageIndex }}</button>
          </li>
          <li class="page-item" :class="{ disabled: currentPageIndex === totalPages }">
            <button class="page-link" @click="getUrls(currentPageIndex + 1, pageSize.value)">Next &raquo;</button>
          </li>
        </ul>
      </nav>
    </div>

    <div v-else>
      <p>No links found.</p>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref } from 'vue';
import axios from 'axios'
import type { Link } from '@/models/Link'
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'
export default defineComponent({
  data(){
    const baseUrl = import.meta.env.VITE_APP_BASEURL + '/Crawler';

    const inputUrl = ref('');

    const links = ref<Link[]>([]);
    const currentPageIndex = ref(0);
    const pageSize = ref(0);
    const totalPages = ref(0);
    const isMakingRequest = ref(false);

    const error = ref('');

    return {
      baseUrl,
      inputUrl,
      links,
      currentPageIndex,
      pageSize,
      totalPages,
      isMakingRequest,
      error
    }

  },
  created() {
    this.getUrls();
  },
  methods: {
    async crawlUrl() {
      this.isMakingRequest = true;
      this.error = '';
      try {
        const body = {
          url: this.inputUrl,
        };

        await axios.post(`${this.baseUrl}`, body);

      } catch (e) {
        this.error = 'Error occurred while crawling the website.';
      } finally {
        this.isMakingRequest = false;
      }
    },
    async getUrls(page: number = 1, pageSizeNumber: number = 7) {
      this.error = '';
      try {
        const response = await axios.get(`${this.baseUrl}/links&page=${page}&pageSize=${pageSizeNumber}`);
        const data = response.data;

        this.links = data.links;

        this.currentPageIndex = data.currentPageIndex;
        this.pageSize = data.pageSize;
        this.totalPages = data.totalPages;

      } catch (e) {
        this.error = 'Error occurred while crawling the website.';
      } finally {
        this.isMakingRequest = false;
      }
    },
    formatDate(date: string) {
      return new Date(date).toLocaleString()
    }
  }
});
</script>