import type { CrawlingLinkSource } from '@/models/CrawlingLinkSource'

export class LinkPerformance {
  public url: string;
  public timeResponseMs: number;
  public crawlingLinkSource: CrawlingLinkSource;
  constructor(url: string, timeResponseMs: number, crawlingLinkSource: number) {
    this.url = url;
    this.timeResponseMs = timeResponseMs;
    this.crawlingLinkSource = crawlingLinkSource;
  }

}