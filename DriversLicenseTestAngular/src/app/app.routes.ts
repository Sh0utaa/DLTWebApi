import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { LoginComponent } from './login/login.component';
import { ExamComponent } from './exam/exam.component';
import { LeaderboardComponent } from './leaderboard/leaderboard.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'home', component: HomeComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent },
  { path: 'exam', component: ExamComponent },
  { path: 'leaderboards', component: LeaderboardComponent}
];