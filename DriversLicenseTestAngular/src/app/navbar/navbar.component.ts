import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {
  navOpen = false;
  isLoggedIn = false;

  constructor(public authService: AuthService, private router: Router) {
    this.authService.loggedIn$.subscribe(status => {
      this.isLoggedIn = status;
    });
  }

  toggleNav() {
    this.navOpen = !this.navOpen;
  }

  handleAuthClick() {
    if (this.isLoggedIn) {
      this.authService.setLoginStatus(false);
      alert('Logged out!');
      this.router.navigate(['/login']);
    } else {
      this.router.navigate(['/login']);
    }
  }
}
