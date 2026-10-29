import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'app-register-form',
    imports: [ReactiveFormsModule],
    templateUrl: './register.html',
})
export class RegisterForm {
    private fb = inject(FormBuilder);
    private authService = inject(AuthService);

    isLoading = signal(false);
    errorMessage = signal('');
    message = signal('');

    registerForm = this.fb.nonNullable.group({
        firstName: ['', Validators.required],
        lastName: ['', Validators.required],
        email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required, Validators.minLength(12), Validators.maxLength(128)]],
    });

    onSubmit(): void {
        if (this.registerForm.invalid || this.isLoading()) {
            this.registerForm.markAllAsTouched();
            return;
        }

        this.isLoading.set(true);
        this.errorMessage.set('');
        this.message.set('');

        const { firstName, lastName, email, password } = this.registerForm.getRawValue();

        this.authService.register({ firstName, lastName, email, password }).subscribe({
            next: () => {
                this.isLoading.set(false);
                this.message.set('User has been created. Go to login');
            },
            error: (error) => {
                this.isLoading.set(false);

                this.errorMessage.set(error.status === 409 ? (error.error?.error ?? 'A user with this email already exists') : error.status === 401 ? 'Invalid email or password' : 'Unable to register. Please try again');
            },
        });
    }
}
