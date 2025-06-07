import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ExamService } from '../exam/exam.service'; // assuming this is your "auth service"

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  isLoggedIn = false;

  constructor(
    private examService: ExamService,
    private http: HttpClient,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.examService.validateUser().subscribe({
      next: res => this.isLoggedIn = res.isValid,
      error: _ => this.isLoggedIn = false
    });
  }

  handleAuthClick(): void {
    if (this.isLoggedIn) {
      this.http.post('/api/auth/logout', {}, {
        withCredentials: true
      }).subscribe({
        next: () => {
          this.router.navigate(['/login']);
        },
        error: err => {
          console.error('Logout failed', err);
        }
      });
    } 
  }
}
