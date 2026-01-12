import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { shareReplay } from 'rxjs/operators';
import { Product } from '@core/models';
import { environment } from '@environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProductsService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/products`;

  private readonly bestsellersCache$ = this.http
    .get<Product[]>(`${this.apiUrl}/bestsellers`)
    .pipe(
      shareReplay({ bufferSize: 1, refCount: true })
    );

  getBestsellers(): Observable<Product[]> {
    return this.bestsellersCache$;
  }
}
