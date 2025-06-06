import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: "root"
})
export class ExamService {
  private apiUrl = "http://localhost:5279/";

  constructor(private http: HttpClient) {}

  getExamQuestions(): Observable<any> {
    return this.http.get(`${this.apiUrl}api/questions/exam-questions`)
  }

  submitAnswers(answers: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/api/questions/submit`, answers);
  }
}

