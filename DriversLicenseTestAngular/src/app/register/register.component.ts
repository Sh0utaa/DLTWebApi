import { HttpClient } from '@angular/common/http'; 

export class RegisterComponent {
  constructor(private http: HttpClient) {} 

  onSubmit(form: any) {
    if (form.valid) {
      this.http.post('https://localhost:5001/api/register', form.value).subscribe({
        next: (res) => {
          console.log('Registration successful', res);
          alert('User registered successfully');
        },
        error: (err) => {
          console.error('Registration failed', err);
          alert('Registration failed');
        }
      });
    }
  }
}
