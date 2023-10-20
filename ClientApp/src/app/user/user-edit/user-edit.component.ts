import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { MiscModel } from '../../core/models/misc';
import { ApiService } from '../../core/api.service';
import { UserService } from '../user.service';
import { uniqueUserName } from '../custom-validators';
import { UserDto } from '../user';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-user-edit',
  templateUrl: './user-edit.component.html',
  styleUrls: ['./user-edit.component.scss']
})
export class UserEditComponent implements OnInit {

  userToEdit: UserDto;

  userForm: FormGroup;
  submitting = false;
  error = null;

  branches = [];

  misc = new MiscModel();

  constructor(private location: Location,
    private api: UserService, private miscSvc: ApiService, private fb: FormBuilder,
    private route: ActivatedRoute  ) { }

  ngOnInit(): void {
    this.userToEdit = this.route.snapshot.data.user;
    this.getMisc();
    this.buildForm();
  }

  buildForm() {
    this.userForm = this.fb.group({
      userId: [this.userToEdit.userId],
      fullName: [this.userToEdit.fullName],
      branch: [this.userToEdit.branchId, Validators.required],
      accessLevel: [this.userToEdit.accessLevel, Validators.required],
      permissions: [this.userToEdit.permissions]
    });

  }

  public get userName() {
    return this.userForm.get('userName');
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

  goBack() {
    this.location.back();
  }

  submitHandler() {
    const user = this.userForm.value;
    this.error = null;
    this.submitting = true;

    this.api.updateUser(user).subscribe(
      () => {
        this.goBack();
      },
      error => {
        this.error = error;
        this.submitting = false;
      }
    )
  }


}
