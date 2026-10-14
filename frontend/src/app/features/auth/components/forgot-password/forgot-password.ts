import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'app-forgot-password-form',
    imports: [ReactiveFormsModule],
    templateUrl: './forgot-password.html',
})
export class ForgotPasswordForm {
    private fb = inject(FormBuilder);
    private authService = inject(AuthService);

    isLoading = false;
    errorMessage = '';

    forgotForm = this.fb.nonNullable.group({
        email: ['', Validators.required, Validators.email],
    });

    onSubmit(): void {
        if (this.forgotForm.invalid || this.isLoading) {
            this.forgotForm.markAllAsTouched();
            return;
        }

        this.isLoading = true;
        this.errorMessage = '';

        const { email } = this.forgotForm.getRawValue();

        this.authService.forgot({ email }).subscribe({
            next: (result) => {
                ((this.isLoading = false), console.log('forgot successful', result));
            },

            error: (error) => {
                this.isLoading = false;
                this.errorMessage = error.status === 401 ? 'Invalid email' : 'Unable to forgot password. Please try again';
            },
        });
    }
}
