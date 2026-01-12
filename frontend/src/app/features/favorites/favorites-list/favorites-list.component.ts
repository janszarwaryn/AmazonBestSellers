import { Component, OnInit, inject, signal, ChangeDetectionStrategy, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { MessageService, ConfirmationService } from 'primeng/api';
import { FavoritesService } from '@core/services/favorites.service';
import { Favorite } from '@core/models';
import { ErrorMessages, MessageSeverity } from '@core/constants';

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
  templateUrl: './favorites-list.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FavoritesListComponent implements OnInit {
  private readonly favoritesService = inject(FavoritesService);
  private readonly messageService = inject(MessageService);
  private readonly confirmationService = inject(ConfirmationService);
  private readonly destroyRef = inject(DestroyRef);

  favorites = signal<Favorite[]>([]);
  loading = signal(true);
  errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.loadFavorites();
  }

  loadFavorites(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.favoritesService.getFavorites()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (favorites) => {
          this.favorites.set(favorites);
          this.loading.set(false);
        },
        error: (error) => {
          this.loading.set(false);
          this.errorMessage.set(error.error?.message || ErrorMessages.FAILED_TO_LOAD_FAVORITES);
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
    this.favoritesService.removeFavorite(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.favorites.update(favorites => favorites.filter(f => f.id !== id));
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

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }
}
