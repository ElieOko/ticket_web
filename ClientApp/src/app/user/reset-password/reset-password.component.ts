import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { passwordMatchValidator } from '../custom-validators';
import { UserDto } from '../user';
import { UserService } from '../user.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent implements OnInit {

  userToEdit: UserDto;
  resetForm: FormGroup;

  submitting = false;
  error;
  passwordInputType = "password";
  showPassword = false;

  constructor(private route: ActivatedRoute, private userSvc: UserService,
    private fb: FormBuilder, private location: Location) { }

  ngOnInit(): void {
    this.userToEdit = this.route.snapshot.data.user;
    this.buildForm();
  }

  buildForm() {
    this.resetForm = this.fb.group({
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required]
    }, { validators: passwordMatchValidator })
  }

  get password() {
    return this.resetForm.get('password');
  }

  get confirmPassword() {
    return this.resetForm.get('confirmPassword');
  }

  onShowPassword(value) {
    this.showPassword = value;
    if (this.showPassword === true) {
      this.passwordInputType = "text";
    } else {
      this.passwordInputType = "password";
    }
  }

  cancelHandler() {
    this.goBack();
  }

  goBack() {
    this.location.back();
  }

  submitHandler() {
    this.submitting = true;
    const data = {
      userId: this.userToEdit.userId,
      password: this.resetForm.value.password
    };

    this.userSvc.resetUserPassword(this.userToEdit.userId, data).subscribe(
      () => { this.goBack() },
      error => { this.error = error; this.submitting = false; }
    );
  }

}
