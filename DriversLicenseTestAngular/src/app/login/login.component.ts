import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html'
})
export class LoginComponent {
  constructor(private http: HttpClient) {}

  onSubmit(form: any) {
    const loginData = {
      username: form.value.username,
      password: form.value.password
    };

    this.http.post('https://yourapi.com/api/auth/login', loginData)
      .subscribe({
        next: response => console.log('Login success', response),
        error: err => console.error('Login failed', err)
      });
  }
}