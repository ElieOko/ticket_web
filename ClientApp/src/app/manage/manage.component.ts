import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ManageService } from './manage.service';
import { IManageResponse } from './models';


export const passwordMatchValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const password = control.get('password');
  const confirmPassword = control.get('confirmPassword');

  return password && confirmPassword && password.value !== confirmPassword.value ? { passwordMatch: true } : null;
};

@Component({
  selector: 'app-manage',
  templateUrl: './manage.component.html',
  styleUrls: ['./manage.component.scss']
})
export class ManageComponent implements OnInit {

  showPassword = false;
  passwordInputType: string = "password";



  currentUser: IManageResponse;

  userProfileForm: FormGroup;
  profileError: any;
  profileSuccess: string;
  profileLoading = false;



  changePasswordForm: FormGroup;
  passwordLoading = false;
  passwordSuccess: string;
  passwordError: any;

  constructor(private manageSvc: ManageService,
    private route: ActivatedRoute,
    private fb: FormBuilder) { }

  ngOnInit(): void {
    this.currentUser = this.route.snapshot.data.user;
    this.buildForms();
  }

  buildForms() {
    this.userProfileForm = this.fb.group({
      userName: [this.currentUser.userProfile.userName, Validators.required],
      fullName: [this.currentUser.userProfile.fullName]
    });

    this.changePasswordForm = this.fb.group({
      currentPassword: ['', Validators.required],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required]
    }, { validators: passwordMatchValidator })
  }

  get userName() {
    return this.userProfileForm.get('userName');
  }
  get currentPassword() {
    return this.changePasswordForm.get('currentPassword');
  }
  get password() {
    return this.changePasswordForm.get('password');
  }
  get confirmPassword() {
    return this.changePasswordForm.get('confirmPassword');
  }

  onShowPassword(value: boolean) {
    this.showPassword = value;
    if (this.showPassword === true) {
      this.passwordInputType = "text";
    } else {
      this.passwordInputType = "password";
    }
  }

  onSubmitProfile() {
    this.profileLoading = true;

    const data = this.userProfileForm.value;

    this.manageSvc.updateUserProfile(data).subscribe(
      () => {
        this.profileLoading = false;
        this.setProfileSuccessMessage("Votre profile a été mis à jour");
      },
      error => {
        this.profileLoading = false;
        this.setProfileErrorMessage(error)
      }
    );

  }


  onSubmitPasword() {

    this.passwordLoading = true;

    const data = {
      currentPassword: this.changePasswordForm.value.currentPassword,
      newPassword: this.changePasswordForm.value.password
    };

    this.manageSvc.changeUserPassword(data).subscribe(
      () => {
        this.passwordLoading = false;
        this.changePasswordForm.reset();
        this.setPasswordSuccessMessage("Votre mot de passe a été changé. Utilisez-le prochenaiment")
      },
      error => {
        this.passwordLoading = false;
        this.setPasswordErrorMessage(error)
      }
    );


  }

  setProfileSuccessMessage(message) {
    this.profileSuccess = message;

    setTimeout(() => { this.profileSuccess = undefined; }, 6000)
  }

  setProfileErrorMessage(message) {
    this.profileError = message;
    setTimeout(() => { this.profileError = undefined; }, 6000)
  }

  setPasswordSuccessMessage(message) {
    this.passwordSuccess = message;

    setTimeout(() => { this.passwordSuccess = undefined; }, 6000)
  }

  setPasswordErrorMessage(message) {
    this.passwordError = message;
    setTimeout(() => { this.passwordError = undefined; }, 6000)
  }

}
