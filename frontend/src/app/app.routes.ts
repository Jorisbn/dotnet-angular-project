import { Routes } from '@angular/router';

import { MainLayout } from './layouts/main-layout/main-layout';
import { Home } from './pages/home/home';
import { Invoices } from './pages/invoices/invoices';
import { Payments } from './pages/payments/payments';
import { Users } from './pages/users/users';
import { Register } from './pages/register/register';
import { Login } from './pages/login/login';
import { Logout } from './pages/logout/logout';

export const routes: Routes = [
    {
        path: '',
        component: MainLayout,

        children: [
            {
                path: '',
                component: Home,
            },
            {
                path: 'invoices',
                component: Invoices,
            },
            {
                path: 'payments',
                component: Payments,
            },
            {
                path: 'users',
                component: Users,
            },
            {
                path: 'register',
                component: Register,
            },
            {
                path: 'login',
                component: Login,
            },
            {
                path: 'logout',
                component: Logout,
            },
        ],
    },
];
