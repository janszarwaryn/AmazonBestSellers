import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { MessageService, ConfirmationService } from 'primeng/api';
import { FavoritesService } from '@core/services/favorites.service';
import { Favorite } from '@core/models';

@Component({
  selector: 'app-favorites-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    ToastModule,
    ConfirmDialogModule
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './favorites-list.component.html'
})
export class FavoritesListComponent implements OnInit {
  private readonly favoritesService = inject(FavoritesService);
  private readonly messageService = inject(MessageService);
  private readonly confirmationService = inject(ConfirmationService);

  favorites = signal<Favorite[]>([]);
  loading = signal(true);
  errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.loadFavorites();
  }

  loadFavorites(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.favoritesService.getFavorites().subscribe({
      next: (favorites) => {
        this.favorites.set(favorites);
        this.loading.set(false);
      },
      error: (error) => {
        this.loading.set(false);
        this.errorMessage.set(error.error?.message || 'Failed to load favorites. Please try again.');
      }
    });
  }

  confirmRemove(favorite: Favorite): void {
    this.confirmationService.confirm({
      message: `Remove "${favorite.title}" from favorites?`,
      header: 'Confirm Removal',
      icon: 'pi pi-exclamation-triangle',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.removeFavorite(favorite.id);
      }
    });
  }

  private removeFavorite(id: number): void {
    this.favoritesService.removeFavorite(id).subscribe({
      next: () => {
        this.favorites.update(favorites => favorites.filter(f => f.id !== id));
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Product removed from favorites'
        });
      },
      error: (error) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: error.error?.message || 'Failed to remove from favorites'
        });
      }
    });
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString('pl-PL', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }
}
