import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  private authSubscription: Subscription | undefined;
  private authService: AuthService = inject(AuthService);
  private router: Router = inject(Router);
  
  AuthenticationForm!: FormGroup;
  email!: FormControl;
  password!: FormControl;
  rememberMe!: Boolean;
  errorMessage: string = '';

  ngOnInit(): void {
    this.createFormControls();
    this.createForm();
  }
  
  ngOnDestroy(): void {
    if (this.authSubscription) {
      this.authSubscription.unsubscribe();
    }
  }

  createFormControls() {
    this.email = new FormControl("", [Validators.required, Validators.email]);
    this.password = new FormControl("", [Validators.required, Validators.minLength(6)]);
  }

  createForm() {
    this.AuthenticationForm = new FormGroup({
      email: this.email,
      password: this.password,
    })
  }

  onSubmit() {
    if (this.AuthenticationForm?.valid) {
      this.login();
    }
  }

  login(): void {
    this.errorMessage = ''; // Clear any previous error messages
    this.authSubscription = this.authService.login(this.AuthenticationForm?.value).subscribe({
      next: resp => {
        localStorage.setItem('accessToken', resp.token);
        localStorage.setItem('userId', resp.userId);
        localStorage.setItem('userRole', resp.userRole);
        this.router.navigate(["/"]);
      },
      error: (errorMsg: string) => {
        this.errorMessage = errorMsg;
        console.error('Login error:', errorMsg);
      }
    });
  }
}