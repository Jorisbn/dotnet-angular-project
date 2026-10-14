import { Component } from '@angular/core';
import { RegisterForm } from '../../features/auth/components/register/register';

@Component({
    selector: 'app-register-page',
    imports: [RegisterForm],
    templateUrl: './register.html',
})
export class Register {}
