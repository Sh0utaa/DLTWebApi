// exam.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { ExamService, Question, QuestionSubmission, SubmissionResponse } from './exam.service';
import { Router } from '@angular/router';
@Component({
  selector: 'app-exam',
  standalone: true,
  imports: [CommonModule, HttpClientModule, FormsModule],
  templateUrl: './exam.component.html',
  styleUrls: ['./exam.component.css']
})
export class ExamComponent implements OnInit {
  questions: Question[] = [];
  currentQuestionIndex = 0;
  selectedAnswerId: number | null = null;
  userAnswers: QuestionSubmission[] = [];
  quizCompleted = false;
  isLoading = true;
  error: string | null = null;
  submissionResult: SubmissionResponse | null = null;

  constructor(private questionService: ExamService, private router: Router) {}

  ngOnInit(): void {
    this.loadQuestions();

    this.questionService.validateUser().subscribe({
      next: (response) => {
        if (!response.isValid) {
          this.router.navigate(['/login'])
        }
      },
      error: (err) => {
        console.error("Validation check fialed, ", err)
        this.router.navigate(['/login'])
      }
    })
  }

  loadQuestions(): void {
    this.isLoading = true;
    this.error = null;
    this.questionService.getExamQuestions().subscribe({
      next: (data) => {
        this.questions = data;
        // Initialize userAnswers array with empty submissions
        this.userAnswers = data.map(question => ({
          questionId: question.id,
          answerId: -1 // -1 indicates unanswered
        }));
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading questions:', err);
        this.error = 'Failed to load questions. Please try again later.';
        this.isLoading = false;
      }
    });
  }

  get currentQuestion(): Question | undefined {
    return this.questions[this.currentQuestionIndex];
  }

  get hasImage(): boolean {
    return !!this.currentQuestion?.image;
  }

  get progressPercentage(): number {
    return ((this.currentQuestionIndex + 1) / this.questions.length) * 100;
  }

  selectAnswer(answerId: number): void {
    this.selectedAnswerId = answerId;
    // Update the userAnswers array
    if (this.currentQuestion) {
      this.userAnswers[this.currentQuestionIndex] = {
        questionId: this.currentQuestion.id,
        answerId: answerId
      };
    }
  }

  nextQuestion(): void {
    if (this.currentQuestionIndex < this.questions.length - 1) {
      this.currentQuestionIndex++;
      // Set the selected answer for the next question
      this.selectedAnswerId = this.userAnswers[this.currentQuestionIndex].answerId !== -1 
        ? this.userAnswers[this.currentQuestionIndex].answerId 
        : null;
    } else {
      // Last question reached - submit all answers
      this.submitAllAnswers();
    }
  }

  prevQuestion(): void {
    if (this.currentQuestionIndex > 0) {
      this.currentQuestionIndex--;
      this.selectedAnswerId = this.userAnswers[this.currentQuestionIndex].answerId !== -1 
        ? this.userAnswers[this.currentQuestionIndex].answerId 
        : null;
    }
  }

  submitAllAnswers(): void {
    // Filter out unanswered questions (if any)
    const answeredQuestions = this.userAnswers.filter(answer => answer.answerId !== -1);
    
    if (answeredQuestions.length !== this.questions.length) {
      this.error = 'Please answer all questions before submitting.';
      return;
    }

    this.isLoading = true;
    this.questionService.submitAnswers(answeredQuestions).subscribe({
      next: (response) => {
        this.submissionResult = response;
        this.quizCompleted = true;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error submitting answers:', err);
        this.error = 'Failed to submit answers. Please try again.';
        this.isLoading = false;
      }
    });
  }

  restartQuiz(): void {
    this.currentQuestionIndex = 0;
    this.selectedAnswerId = null;
    this.quizCompleted = false;
    this.userAnswers = [];
    this.submissionResult = null;
    this.error = null;
    this.loadQuestions();
  }
}