import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { FooterComponent } from './footer/footer.component';
import { RegisterComponent } from './register/register.component';
import { LoginComponent } from './login/login.component';


export const routes: Routes = [
  { path: '', redirectTo: 'register', pathMatch: 'full' },

  { path: 'Home', loadComponent: () => import('./home/home.component').then(m => m.HomeComponent) },
  { path: 'Footer', loadComponent: () => import('./footer/footer.component').then(m => m.FooterComponent) },
  { path: 'register', loadComponent: () => import('./register/register.component').then(m => m.RegisterComponent) },
  { path: 'login', loadComponent: () => import('./login/login.component').then(m => m.LoginComponent) },

  { path: '**', redirectTo: 'register' }
];




