import { Component } from '@angular/core';
import { LoginForm } from '../../features/auth/components/login/login';

@Component({
    selector: 'app-login-page',
    imports: [LoginForm],
    templateUrl: './login.html',
})
export class Login {}
