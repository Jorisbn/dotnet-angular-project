import { Component, inject } from '@angular/core';
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

    isLoading = false;
    errorMessage = '';

    registerForm = this.fb.nonNullable.group({
        firstName: ['', Validators.required],
        lastName: ['', Validators.required],
        email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required, Validators.minLength(12), Validators.maxLength(128)]],
    });

    onSubmit(): void {
        if (this.registerForm.invalid || this.isLoading) {
            this.registerForm.markAllAsTouched();
            return;
        }

        this.isLoading = true;
        this.errorMessage = '';

        const { firstName, lastName, email, password } = this.registerForm.getRawValue();

        this.authService.register({ firstName, lastName, email, password }).subscribe({
            next: (result) => {
                ((this.isLoading = false), console.log('register successful', result));
            },

            error: (error) => {
                this.isLoading = false;
                this.errorMessage = error.status === 401 ? 'Invalid email or password' : 'Unable to register. Please try again';
            },
        });
    }
}
