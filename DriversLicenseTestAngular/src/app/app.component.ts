import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './navbar/navbar.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent ],
  templateUrl: './app.component.html',
})
export class AppComponent {
  title(title: any) {
    throw new Error('Method not implemented.');
  }
}