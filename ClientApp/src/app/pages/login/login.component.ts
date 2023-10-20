import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../core/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  loginForm: FormGroup;
  loading = false;
  error
  returnUrl: string;

  passwordInputType = "password";
  showPassword = false;

  constructor(private fb: FormBuilder,
    private auth: AuthService,
    private route: ActivatedRoute,
    private router: Router) {
    // redirect to home if already logged in
    if (this.auth.currentUser) {
      this.router.navigate(['/app']);
    }
  }

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    })

    // get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/app';
  }

  public get username() {
    return this.loginForm.get('username');
  }

  public get password() {
    return this.loginForm.get('password');
  }

  onShowPassword(value) {

    this.showPassword = value;

    if (this.showPassword === true) {
      this.passwordInputType = "text";
    } else {
      this.passwordInputType = "password";
    }
  }

  submitHandler() {
    this.error = null;

    this.loading = true;
    const request = {
      username: this.loginForm.value.username,
      password: this.loginForm.value.password,
      grant_type: 'password'
    };
    this.auth.login(request).subscribe(
      () => {
        this.router.navigate([this.returnUrl])
      },
      error => {
        this.loading = false;
        this.error = error
        setTimeout(() => { this.error = null }, 3000)
      }
    )
  }
}
