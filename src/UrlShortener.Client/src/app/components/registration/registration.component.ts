import { Component, OnInit, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Subscription, debounceTime, distinctUntilChanged, of, switchMap } from 'rxjs';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { emailValidator } from './validators/email.validator';
import { emailExistsValidator } from './validators/email.exists.validator';
import { passwordMatchValidator } from './validators/password.match';

@Component({
  selector: 'app-registration',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
  ],
  templateUrl: './registration.component.html',
  styleUrl: './registration.component.css'
})
export class RegistrationComponent implements OnInit {
  private authSubscription: Subscription | undefined;
  private checkEmailSubscription: Subscription | undefined;

  private authService: AuthService = inject(AuthService);
  private router: Router = inject(Router);

  emailExists = false;
  isCheckingEmail = false;
  
  registrationForm!: FormGroup;
  firstName!: FormControl;
  lastName!: FormControl;
  email!: FormControl;
  password!: FormControl;
  confirmPassword!: FormControl;

  ngOnInit(): void {
    this.createFormControls();
    this.createForm();
  }

  createFormControls() {
    this.firstName = new FormControl("", Validators.required);
    this.lastName = new FormControl("");
    this.email = new FormControl("", [Validators.required, emailValidator(), emailExistsValidator(() => this.emailExists)]);
    this.password = new FormControl("", [
      Validators.required, 
      Validators.minLength(8),
      Validators.pattern(/^(?=.*[a-zA-Z])(?=.*\d).{8,}$/)
    ]);
    this.confirmPassword = new FormControl("", Validators.required);
  }

  createForm() {
    this.registrationForm = new FormGroup({
      firstName: this.firstName,
      lastName: this.lastName,
      email: this.email,
      password: this.password,
      confirmPassword: this.confirmPassword,
    }, { validators: passwordMatchValidator() })
  }

  onSubmit() {
    if (this.registrationForm?.valid) {
      this.register();
    }
  }

  updateEmailValidators() {
    const emailControl = this.registrationForm.get('email');
    if (emailControl) {
      emailControl.updateValueAndValidity();
    }
  }

  register() {
    this.authSubscription = this.authService.register(this.registrationForm?.value).subscribe({
      next: obj => {
        console.log(obj);
        this.router.navigate(["auth/login"]);
      },
      error: (error) => {
        if (error.status === 400) {
          alert(`Bad request: ${error.error.detail}`);
        } else if (error.status === 500) {
          alert(`Internal server error: ${error.error.detail}`)
        } else {
          alert(`An error occurred during registration: ${error.error.detail}`);
        }
      }
    });
  }

  ngOnDestroy() {
    if (this.authSubscription) {
      this.authSubscription?.unsubscribe();
      this.checkEmailSubscription?.unsubscribe();
    }
  }
}
