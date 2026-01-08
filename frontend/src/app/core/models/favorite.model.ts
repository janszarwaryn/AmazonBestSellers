export interface Favorite {
  id: number;
  asin: string;
  title: string;
  price?: string;
  imageUrl?: string;
  productUrl: string;
  rating?: number;
  createdAt: Date;
}

export interface CreateFavorite {
  ASIN: string;
  Title: string;
  ProductUrl: string;
  Price?: string;
  ImageUrl?: string;
  Rating?: number;
}
