import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'app-login-form',
    imports: [ReactiveFormsModule],
    templateUrl: './login.html',
})
export class LoginForm {
    private fb = inject(FormBuilder);
    private authService = inject(AuthService);

    isLoading = signal(false);
    errorMessage = signal('');

    loginForm = this.fb.nonNullable.group({
        email: ['', [Validators.required, Validators.email]],
        password: ['', Validators.required],
    });

    onSubmit(): void {
        if (this.loginForm.invalid || this.isLoading()) {
            this.loginForm.markAllAsTouched();
            return;
        }

        this.isLoading.set(true);
        this.errorMessage.set('');

        const { email, password } = this.loginForm.getRawValue();

        this.authService.login({ email, password }).subscribe({
            next: () => {
                this.isLoading.set(false);
            },

            error: (error) => {
                this.isLoading.set(false);
                this.errorMessage.set(error.status === 401 ? 'invalid email or password' : 'Unable to log in. Please try again');
            },
        });
    }
}
