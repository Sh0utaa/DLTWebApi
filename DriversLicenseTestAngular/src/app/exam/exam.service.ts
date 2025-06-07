// services/question.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Answer {
  id: number;
  text: string;
}

export interface Question {
  id: number;
  image: string | null;
  questionContent: string;
  answers: Answer[];
}

export interface QuestionSubmission {
  questionId: number;
  answerId: number;
}

export interface SubmissionResponse {
  correctAmount: number;
  incorrectAmount: number;
  failed: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class ExamService {
    private apiUrl = 'http://localhost:5279/api/questions';

    private authUrl = 'http://localhost:5279/api/auth'

  constructor(private http: HttpClient) { }

  validateUser(): Observable<{ isValid: boolean}> {
    return this.http.get<{ isValid: boolean }>(`${this.authUrl}/validate-user`, {
      withCredentials: true
    })
  }

  getExamQuestions(): Observable<Question[]> {
    return this.http.get<Question[]>(`${this.apiUrl}/exam-questions`, { withCredentials: true });
  }

    submitAnswers(answers: QuestionSubmission[]): Observable<SubmissionResponse> {
        const headers = new HttpHeaders({
        'Content-Type': 'application/json'
        });

        return this.http.post<SubmissionResponse>(
        `${this.apiUrl}/submit`,
        answers,
        { headers, withCredentials: true }
        );
  }
}