import { Component } from '@angular/core';
import { LogoutForm } from '../../features/auth/components/logout/logout';

@Component({
    selector: 'app-logout',
    imports: [LogoutForm],
    templateUrl: './logout.html',
})
export class Logout {}
