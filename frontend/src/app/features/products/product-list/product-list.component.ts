import { Component, OnInit, inject, signal, computed, ChangeDetectionStrategy, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { DataViewModule } from 'primeng/dataview';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { ProductsService } from '@core/services/products.service';
import { FavoritesService } from '@core/services/favorites.service';
import { AuthService } from '@core/services/auth.service';
import { Product, Favorite } from '@core/models';
import { ErrorMessages, MessageSeverity } from '@core/constants';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [
    CommonModule,
    DataViewModule,
    ToastModule
  ],
  providers: [MessageService],
  templateUrl: './product-list.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProductListComponent implements OnInit {
  private readonly productsService = inject(ProductsService);
  private readonly favoritesService = inject(FavoritesService);
  private readonly authService = inject(AuthService);
  private readonly messageService = inject(MessageService);
  private readonly destroyRef = inject(DestroyRef);

  products = signal<Product[]>([]);
  favorites = signal<Favorite[]>([]);
  loading = signal(true);
  errorMessage = signal<string | null>(null);

  readonly isAuthenticated = this.authService.isAuthenticated;

  ngOnInit(): void {
    this.loadProducts();
    if (this.isAuthenticated()) {
      this.loadFavorites();
    }
  }

  loadProducts(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.productsService.getBestsellers()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (products) => {
          this.products.set(products);
          this.loading.set(false);
        },
        error: (error) => {
          this.loading.set(false);
          this.errorMessage.set(error.error?.message || ErrorMessages.FAILED_TO_LOAD_PRODUCTS);
        }
      });
  }

  loadFavorites(): void {
    this.favoritesService.getFavorites()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (favorites) => {
          this.favorites.set(favorites);
        },
        error: (error) => {
          this.messageService.add({
            severity: MessageSeverity.WARN,
            summary: 'Warning',
            detail: ErrorMessages.FAILED_TO_LOAD_FAVORITES
          });
        }
      });
  }

  isFavorite(asin: string): boolean {
    return this.favorites().some(fav => fav.asin === asin);
  }

  getFavoriteId(asin: string): number | undefined {
    return this.favorites().find(fav => fav.asin === asin)?.id;
  }

  addToFavorites(product: Product): void {
    if (!this.isAuthenticated()) {
      this.messageService.add({
        severity: MessageSeverity.WARN,
        summary: 'Authentication Required',
        detail: 'Please login to add favorites'
      });
      return;
    }

    const favorite = {
      ASIN: product.asin,
      Title: product.title,
      ProductUrl: product.productUrl || '',
      Price: product.price,
      ImageUrl: product.imageUrl,
      Rating: product.rating
    };

    this.favoritesService.addFavorite(favorite)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (newFavorite) => {
          this.favorites.update(favs => [...favs, newFavorite]);
          this.messageService.add({
            severity: MessageSeverity.SUCCESS,
            summary: 'Success',
            detail: 'Product added to favorites'
          });
        },
        error: (error) => {
          this.messageService.add({
            severity: MessageSeverity.ERROR,
            summary: 'Error',
            detail: error.error?.message || ErrorMessages.FAILED_TO_ADD_FAVORITE
          });
        }
      });
  }

  removeFromFavorites(asin: string): void {
    const favoriteId = this.getFavoriteId(asin);
    if (!favoriteId) return;

    this.favoritesService.removeFavorite(favoriteId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.favorites.update(favs => favs.filter(f => f.id !== favoriteId));
          this.messageService.add({
            severity: MessageSeverity.SUCCESS,
            summary: 'Success',
            detail: 'Product removed from favorites'
          });
        },
        error: (error) => {
          this.messageService.add({
            severity: MessageSeverity.ERROR,
            summary: 'Error',
            detail: error.error?.message || ErrorMessages.FAILED_TO_REMOVE_FAVORITE
          });
        }
      });
  }
}
