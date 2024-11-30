import { Routes } from '@angular/router';

import { RegistrationComponent } from './components/registration/registration.component';
import { LoginComponent } from './components/login/login.component';
import { UrlDetailsComponent } from './components/url-details/url-details.component';
import { HomeComponent } from './components/home/home.component';
import { AboutComponent } from './components/about/about.component';

export const routes: Routes = [
    { path: '', component: HomeComponent, title: "Home page" },
    { path: 'about', component: AboutComponent, title: "About service" },
    { path: 'url-details/:id', component: UrlDetailsComponent, title: "URL details" },
    { path: 'auth/registration', component: RegistrationComponent, title: "Registration" },
    { path: 'auth/login', component: LoginComponent, title: "Login" },
];