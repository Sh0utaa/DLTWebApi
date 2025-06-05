import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { FooterComponent } from './footer/footer.component';


export const routes: Routes = [
  { path: 'HomePage', component: HomeComponent },
  { path: 'Footer', component: FooterComponent},
];