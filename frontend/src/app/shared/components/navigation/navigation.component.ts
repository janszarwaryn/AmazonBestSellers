import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { MenubarModule } from 'primeng/menubar';
import { ButtonModule } from 'primeng/button';
import { MenuItem } from 'primeng/api';
import { AuthService } from '@core/services/auth.service';

@Component({
  selector: 'app-navigation',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MenubarModule,
    ButtonModule
  ],
  templateUrl: './navigation.component.html',
  styleUrl: './navigation.component.css'
})
export class NavigationComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly isAuthenticated = this.authService.isAuthenticated;
  readonly currentUser = this.authService.currentUser;

  menuItems: MenuItem[] = [
    {
      label: 'Products',
      icon: 'pi pi-shopping-bag',
      routerLink: '/products'
    },
    {
      label: 'Favorites',
      icon: 'pi pi-heart',
      routerLink: '/favorites',
      visible: this.isAuthenticated()
    }
  ];

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
