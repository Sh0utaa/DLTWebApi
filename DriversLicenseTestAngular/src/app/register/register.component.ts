import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms'; 
import { CommonModule } from '@angular/common'; 
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { Router, RouterModule } from '@angular/router';
import { ExamService } from '../exam/exam.service';
@Component({
  selector: 'app-register',
  standalone: true, 
  imports: [CommonModule, FormsModule, HttpClientModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  user = {
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
        this.router.navigate(['/register'])
      }
    })
  }

  submitForm() {
    console.log('Submitting user:', this.user);

    const apiUrl = 'http://localhost:5279/register'; 

    this.http.post(apiUrl, this.user).subscribe({
      next: (res) => {
        console.log('Registration successful', res);
        alert('User registered successfully!');
        this.router.navigate(['/login']); 
      },
      error: (err) => {
        console.error('Registration failed', err);
        alert('Error registering user. Check console for details.');
      }
    });
  }
}
