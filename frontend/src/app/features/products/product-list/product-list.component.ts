import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DataViewModule } from 'primeng/dataview';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { ProductsService } from '@core/services/products.service';
import { FavoritesService } from '@core/services/favorites.service';
import { AuthService } from '@core/services/auth.service';
import { Product } from '@core/models';

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
})
export class ProductListComponent implements OnInit {
  private readonly productsService = inject(ProductsService);
  private readonly favoritesService = inject(FavoritesService);
  private readonly authService = inject(AuthService);
  private readonly messageService = inject(MessageService);

  products = signal<Product[]>([]);
  loading = signal(true);
  errorMessage = signal<string | null>(null);

  readonly isAuthenticated = this.authService.isAuthenticated;

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.productsService.getBestsellers().subscribe({
      next: (products) => {
        this.products.set(products);
        this.loading.set(false);
      },
      error: (error) => {
        this.loading.set(false);
        this.errorMessage.set(error.error?.message || 'Failed to load products. Please try again.');
      }
    });
  }

  addToFavorites(product: Product): void {
    if (!this.isAuthenticated()) {
      this.messageService.add({
        severity: 'warn',
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

    this.favoritesService.addFavorite(favorite).subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Product added to favorites'
        });
      },
      error: (error) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: error.error?.message || 'Failed to add to favorites'
        });
      }
    });
  }
}
