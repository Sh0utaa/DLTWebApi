import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { Router, RouterModule } from '@angular/router';
import { ExamService } from '../exam/exam.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, HttpClientModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  loginData = {
    email: '',
    password: ''
  };

  constructor(private http: HttpClient, private router: Router, private questionService: ExamService) {}

  ngOnInit(): void {
    this.questionService.validateUser().subscribe({
      next: (response) => {
        if (response.isValid) {
          this.router.navigate(['/'])
        }
      },
      error: (err) => {
        console.error("Validation check fialed, ", err)
        this.router.navigate(['/'])
      }
    })
  }

  submitLogin() {
    const loginUrl = 'http://localhost:5279/login?useCookies=true';

    this.http.post(loginUrl, this.loginData, {
      withCredentials: true 
    }).subscribe({
      next: (res) => {
        console.log('✅ Login successful:', res);
        alert('Login successful!');
        localStorage.setItem('isLoggedIn', 'true'); 
        this.router.navigate(['/home']); 
      },
      error: (err) => {
        console.error('❌ Login failed:', err);
        alert('Login failed: incorrect email or password.');
      }
    });
  }
}
