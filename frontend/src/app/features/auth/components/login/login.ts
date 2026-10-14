import { Component, inject } from '@angular/core';
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

    isLoading = false;
    errorMessage = '';

    loginForm = this.fb.nonNullable.group({
        email: ['', [Validators.required, Validators.email]],
        password: ['', Validators.required],
    });

    onSubmit(): void {
        if (this.loginForm.invalid || this.isLoading) {
            this.loginForm.markAllAsTouched();
            return;
        }

        this.isLoading = true;
        this.errorMessage = '';

        const { email, password } = this.loginForm.getRawValue();

        this.authService.login({ email, password }).subscribe({
            next: (result) => {
                ((this.isLoading = false), console.log('login successful', result));
            },

            error: (error) => {
                this.isLoading = false;
                this.errorMessage = error.status === 401 ? 'invalid email or password' : 'Unable to log in. Please try again';
            },
        });
    }
}
