import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'app-reset-password-form',
    imports: [ReactiveFormsModule],
    templateUrl: './reset-password.html',
})
export class ResetPasswordForm {
    private fb = inject(FormBuilder);
    private authService = inject(AuthService);

    isLoading = false;
    errorMessage = '';

    resetForm = this.fb.nonNullable.group({
        email: ['', Validators.required, Validators.email],
        password: ['', Validators.required],
    });

    onSubmit(): void {
        if (this.resetForm.invalid || this.isLoading) {
            this.resetForm.markAllAsTouched();
            return;
        }

        this.isLoading = true;
        this.errorMessage = '';

        const { email, password } = this.resetForm.getRawValue();

        this.authService.reset({ email, password }).subscribe({
            next: (result) => {
                ((this.isLoading = false), console.log('reset successful', result));
            },

            error: (error) => {
                this.isLoading = false;
                this.errorMessage = error.status === 401 ? 'Invalid email or password' : 'Unable to reset password. Please try again.';
            },
        });
    }
}
