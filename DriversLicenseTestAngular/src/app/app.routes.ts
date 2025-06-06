import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { LoginComponent } from './login/login.component';
import { AuthGuard } from './auth/auth.guard';
import { AlreadyLoggedInGuard } from './auth/already-logged.guard';
import { LeaderboardComponent } from './leaderboard/leaderboard.component';


export const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  { path: 'register', component: RegisterComponent, canActivate: [AlreadyLoggedInGuard] },
  { path: 'login', component: LoginComponent, canActivate: [AlreadyLoggedInGuard] },
   { path: 'leaderboards', component: LeaderboardComponent },
];
