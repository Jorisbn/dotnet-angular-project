import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'app-logout-form',
    imports: [],
    templateUrl: './logout.html',
})
export class LogoutForm {
    private authService = inject(AuthService);
    private router = inject(Router);

    isLoading = false;
    errorMessage = '';

    onSubmit(): void {
        if (this.isLoading) return;

        this.isLoading = true;
        this.errorMessage = '';

        this.authService.logout().subscribe({
            next: () => {
                this.isLoading = false;
                this.router.navigate(['/login']);
            },
            error: () => {
                this.isLoading = false;
                this.errorMessage = 'Unable to sign out. Please try again.';
            },
        });
    }
}
