import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { MiscModel } from '../../core/models/misc';
import { ApiService } from '../../core/api.service';
import { UserService } from '../user.service';
import { passwordMatchValidator, uniqueUserName } from '../custom-validators';


@Component({
  selector: 'app-user-form',
  templateUrl: './user-form.component.html',
  styleUrls: ['./user-form.component.scss']
})
export class UserFormComponent implements OnInit {

  userForm: FormGroup;
  submitting = false;
  submitAndNew = false;
  showPassword = false;
  passwordInputType: string = "password";
  error = null;

  branches = [];
  isEditMode = true;

  misc = new MiscModel();

  constructor(private location: Location,
    private api: UserService, private miscSvc: ApiService, private fb: FormBuilder) { }

  ngOnInit(): void {
    this.buildForm()
    this.getMisc();
  }

  buildForm() {
    this.userForm = this.fb.group({
      userName: ['', { updateOn: 'blur', validators: Validators.required, asyncValidators: uniqueUserName(this.api) }],
      fullName: [''],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required],
      branch: [null, Validators.required],
      accessLevel: [null, Validators.required],
      permissions: [null]
    }, { validators: passwordMatchValidator });

  }

  onShowPassword(value: boolean) {
    this.showPassword = value;
    if (this.showPassword === true) {
      this.passwordInputType = "text";
    } else {
      this.passwordInputType = "password";
    }
  }

  public get userName() {
    return this.userForm.get('userName');
  }

  public get password() {
    return this.userForm.get('password');
  }

  public get confirmPassword() {
    return this.userForm.get('confirmPassword');
  }


  public get branch() {
    return this.userForm.get('branch');
  }

  public get accessLevel() {
    return this.userForm.get('accessLevel');
  }

  public get permissions() {
    return this.userForm.get('permissions');
  }

  handleFilter(value) {
    this.branches = this.misc.branches.filter((s) => s.branchName.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  getMisc() {
    this.miscSvc.getMisc("local").subscribe(res => {
      this.misc = res;
      this.branches = this.misc.branches.slice();
    },
      error => { console.log(error) })
  }

  cancelHandler() {
    this.goBack();
  }

  onSubmitAndNew() {
    this.submitAndNew = true;
  }

  goBack() {
    this.location.back();
  }


  submitHandler() {
    const user = this.userForm.value;
    this.error = null;
    this.submitting = true;

    this.api.createUser(user).subscribe(
      () => {
        if (this.submitAndNew) {
          this.userForm.reset();
          this.submitAndNew = false;
          this.submitting = false;
        } else {
          this.goBack();
        }
      },
      error => {
        this.error = error;
        this.submitAndNew = false;
        this.submitting = false;
      }
    )
  }
 

}


