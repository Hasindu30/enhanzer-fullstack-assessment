import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent {
  loginForm: FormGroup;
  isLoading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  get email() { return this.loginForm.get('email')!; }
  get password() { return this.loginForm.get('password')!; }

  onSubmit(): void {
    if (this.loginForm.invalid || this.isLoading) return;

    this.isLoading = true;
    this.errorMessage = '';

    this.authService.login(this.loginForm.value).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success) {
          this.router.navigate(['/purchase-bill']);
        } else {
          this.errorMessage = response.message || 'Login failed. Please try again.';
        }
      },
      error: (err: HttpErrorResponse) => {
        this.isLoading = false;
        if (err.status === 401) {
          this.errorMessage = err.error?.message || 'Invalid email or password.';
        } else if (err.status === 503) {
          this.errorMessage = 'Authentication service is unavailable. Please try again later.';
        } else {
          this.errorMessage = 'An unexpected error occurred. Please try again.';
        }
      }
    });
  }
}
