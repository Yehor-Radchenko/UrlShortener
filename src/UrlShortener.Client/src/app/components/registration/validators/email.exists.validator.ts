import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function emailExistsValidatorOld(emailExists: boolean): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const valid = !emailExists;
    return valid ? null : { emailExists: true };
  };
}

export function emailExistsValidator(emailExists: () => boolean): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const valid = !emailExists();
    return valid ? null : { emailExists: true };
  };
}