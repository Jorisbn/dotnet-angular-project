import { Component, inject, signal } from '@angular/core';
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

    isLoading = signal(false);
    errorMessage = signal('');

    onSubmit(): void {
        if (this.isLoading() === true) return;

        this.isLoading.set(true);
        this.errorMessage.set('');

        this.authService.logout().subscribe({
            next: () => {
                this.router.navigate(['/login']);
            },
            error: () => {
                this.isLoading.set(false);
                this.errorMessage.set('Unable to sign out. Please try again.');
            },
        });
    }
}
