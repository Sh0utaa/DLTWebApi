import { Component, OnInit } from '@angular/core';
import { ExamService } from './exam.service';
import { CommonModule } from '@angular/common'; // Add this for *ngIf, *ngFor

interface Question {
  id: number;
  image: string | null;
  questionContent: string;
  answers: Answer[];
}

interface Answer {
  id: number;
  text: string;
}

interface UserAnswer {
  questionId: number;
  answerId: number;
}

@Component({
  selector: 'app-exam',
  standalone: true, // Add this for standalone component
  imports: [CommonModule], // Add CommonModule for directives
  templateUrl: './exam.component.html',
  styleUrl: './exam.component.css'
})
export class ExamComponent implements OnInit {
  questions: Question[] = [];
  currentQuestionIndex = 0;
  userAnswers: UserAnswer[] = [];
  isLoading = false;
  examSubmitted = false;
  result: any;

  // Inject ExamService via constructor
  constructor(private examService: ExamService) {}

  ngOnInit(): void {
    this.loadQuestions();
  }

  loadQuestions(): void {
    this.isLoading = true;
    this.examService.getExamQuestions().subscribe({
      next: (data) => {
        this.questions = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching questions:', err);
        this.isLoading = false;
      }
    });
  }

  onSelectAnswer(questionId: number, answerId: number): void {
    const existingAnswerIndex = this.userAnswers.findIndex(
      a => a.questionId === questionId
    );
    
    if (existingAnswerIndex >= 0) {
      this.userAnswers[existingAnswerIndex].answerId = answerId;
    } else {
      this.userAnswers.push({ questionId, answerId });
    }
  }

  nextQuestion(): void {
    if (this.currentQuestionIndex < this.questions.length - 1) {
      this.currentQuestionIndex++;
    }
  }

  prevQuestion(): void {
    if (this.currentQuestionIndex > 0) {
      this.currentQuestionIndex--;
    }
  }

  submitExam(): void {
    this.isLoading = true;
    
    this.examService.submitAnswers(this.userAnswers).subscribe({
      next: (response) => {
        this.result = response;
        this.examSubmitted = true;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error submitting exam:', err);
        this.isLoading = false;
      }
    });
  }

  isAnswerSelected(questionId: number, answerId: number): boolean {
    const answer = this.userAnswers.find(a => a.questionId === questionId);
    return answer ? answer.answerId === answerId : false;
  }
}