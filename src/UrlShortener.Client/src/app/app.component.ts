import { Component, inject } from '@angular/core';
import { RegistrationComponent } from './components/registration/registration.component';
import { LoginComponent } from './components/login/login.component';
import { HomeComponent } from './components/home/home.component';
import { RouterModule } from '@angular/router';

import { UrlDetailsComponent } from './components/url-details/url-details.component';
import { ReactiveFormsModule } from '@angular/forms';
import { UrlService } from './services/url.service';
import { HttpClientModule } from '@angular/common/http';
import { AuthService } from './services/auth.service';
import { CommonModule } from '@angular/common';
import { AboutComponent } from './components/about/about.component';

@Component({
  standalone: true,
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  imports:[
    RouterModule, 
    HttpClientModule,
    CommonModule,
    ReactiveFormsModule,
  ],
  providers: [
    UrlService,
    AuthService
  ],
})
export class AppComponent {
  authService: AuthService = inject(AuthService);

  isAuthenticated(): boolean {
    return this.authService.isAuthenticated();
  }

  logout(): void {
    this.authService.logout();
  }
}
