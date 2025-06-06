import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private loggedInSubject = new BehaviorSubject<boolean>(false);
  public loggedIn$ = this.loggedInSubject.asObservable();

  constructor() {
    if (typeof window !== 'undefined') {
      const isLoggedIn = !!localStorage.getItem('user');
      this.loggedInSubject.next(isLoggedIn);
    }
  }

  isLoggedIn(): boolean {
    return this.loggedInSubject.value;
  }

  setLoginStatus(status: boolean) {
    this.loggedInSubject.next(status);

    if (typeof window !== 'undefined') {
      if (status) {
        localStorage.setItem('user', 'true');
      } else {
        localStorage.removeItem('user');
      }
    }
  }
}
