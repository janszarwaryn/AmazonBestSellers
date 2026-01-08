import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Favorite, CreateFavorite } from '@core/models';
import { environment } from '@environments/environment';

@Injectable({
  providedIn: 'root'
})
export class FavoritesService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/favorites`;

  getFavorites(): Observable<Favorite[]> {
    return this.http.get<Favorite[]>(this.apiUrl);
  }

  addFavorite(favorite: CreateFavorite): Observable<Favorite> {
    return this.http.post<Favorite>(this.apiUrl, favorite);
  }

  removeFavorite(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
