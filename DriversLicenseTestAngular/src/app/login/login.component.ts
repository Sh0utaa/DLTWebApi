import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
 imports: [CommonModule, FormsModule, HttpClientModule, RouterModule], 
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginData = {
    email: '',
    password: ''
  };

  constructor(private http: HttpClient) {}

  submitLogin() {
    const loginUrl = 'http://localhost:5279/login?useCookies=true'; 

    this.http.post(loginUrl, this.loginData, {
      withCredentials: true 
    }).subscribe({
      next: (res) => {
        console.log('✅ Login successful:', res);
        alert('Login successful!');
      },
      error: (err) => {
        console.error('❌ Login failed:', err);
        alert('Login failed: incorrect email or password.');
      }
    });
  }
}
