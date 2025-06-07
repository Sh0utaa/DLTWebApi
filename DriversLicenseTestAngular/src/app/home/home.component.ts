import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  constructor(private router: Router, private http: HttpClient) {}

  startExam() {
    this.http.get('http://localhost:5279/check-auth', { withCredentials: true }).subscribe({
      next: () => {
        
        this.router.navigate(['/exam']);
      },
      error: () => {
       
        alert('You must be logged in to start the exam.');
        this.router.navigate(['/login']);
      }
    });
  }
}