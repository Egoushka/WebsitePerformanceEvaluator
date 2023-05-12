export class Link {
  public id: number;
  public createdAt: string;
  public url: string;
  constructor(id: number, url: string, createdAt: string) {
    this.id = id;
    this.url = url;
    this.createdAt = createdAt;
  }
}