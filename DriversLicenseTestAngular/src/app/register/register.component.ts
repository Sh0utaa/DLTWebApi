import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms'; 
import { CommonModule } from '@angular/common'; 
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { RouterModule } from '@angular/router';
@Component({
  selector: 'app-register',
  standalone: true, 
   imports: [CommonModule, FormsModule, HttpClientModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  user = {
    email: '',
    password: ''
  };

  constructor(private http: HttpClient) {}

  submitForm() {
    console.log('Submitting user:', this.user);

    const apiUrl = 'http://localhost:5279/register'; 
    this.http.post(apiUrl, this.user).subscribe({
      next: (res) => {
        console.log('Registration successful', res);
        alert('User registered successfully!');
      },
      error: (err) => {
        console.error('Registration failed', err);
        alert('Error registering user. Check console for details.');
      }
    });
  }
}
