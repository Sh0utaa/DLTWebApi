import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-leaderboard',
  standalone: true,
  imports: [CommonModule, HttpClientModule],
  templateUrl: './leaderboard.component.html',
  styleUrls: ['./leaderboard.component.css']
})
export class LeaderboardComponent implements OnInit {
  leaderboard: any[] = [];
  loading = true;
  error: string = '';

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.http.get<any[]>('http://localhost:5279/api/leaderboards/rates', {
      withCredentials: true
    }).subscribe({
      next: (data) => {
        this.leaderboard = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error fetching leaderboard:', err);
        this.error = 'Failed to load leaderboard.';
        this.loading = false;
      }
    });
  }
}
