import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiService } from '../../core/api.service';
import { MiscModel } from '../../core/models/misc';
import { AuthService } from '../../core/auth.service';

@Component({
  selector: 'app-call',
  templateUrl: './call.component.html',
  styleUrls: ['./call.component.scss']
})
export class CallComponent implements OnInit {

  callForm: FormGroup;
  callLoading = false;
  callError = null;
  successCall = null;


  messageForm: FormGroup;
  message: string;
  messageLoading = false;
  messageError = null;


  misc = new MiscModel();

  constructor(private fb: FormBuilder, private api: ApiService, private auth: AuthService) { }

  loadMisc() {
    this.api.getMisc().subscribe(
      res => this.misc = res,
      error => console.log(error)
    );
  }

  ngOnInit(): void {

    this.loadMisc();

    this.callForm = this.fb.group({
      orderNumber: [null, Validators.required],
      counter: [null, Validators.required],
      note:[''],
      forPayment:[false]
    })

    this.messageForm = this.fb.group({
      message: ['', Validators.required]
    })

    
  }

  get orderNumber() {
    return this.callForm.get('orderNumber')
  }

  get counter() {
    return this.callForm.get('counter');
  }

  get forPayment() {
    return this.callForm.get('forPayment');
  }

  onCallForPayment() {
    this.forPayment.patchValue(true);
  }

  onSubmit() {
    //set loading state
    this.callLoading = true;
    this.callError = null;
    this.successCall = null;
    //get form data
    const data = this.callForm.value;

    this.api.createTransferCall(data).subscribe(
      res => {
        this.callSocket(data, res)
      },
      error => {
        this.callError = error;
        this.callLoading = false;
        this.forPayment.setValue(false);
      }
    );
  }

  callSocket(data, res) {
    const call = {
      counter: data.counter.name,
      token: res.token,
      time: res.time
    };

    const url = `wss://sockets.customers.soficomit.com/Notifier?branch=${this.auth.currentUser.branchId}`;
    let socket = new WebSocket(url);

    socket.onopen = (e) => {
      //connection established"
      socket.send(JSON.stringify(call));
      this.successCall = `Appel en cours ticket ${res.token}`;
      this.callLoading = false;
      this.callForm.reset({ orderNumber: null, counter: data.counter, note: null, forPayment: false });
      this.forPayment.setValue(false);
      socket.close();
    };
    socket.onmessage = (event) => {
      //console.log("message");
    };
    socket.onclose = (event) => {
      //console.log("closed");
    };
    socket.onerror = (event) => {
      this.callLoading = false;
      this.callError = "Echec de connexion";
      this.forPayment.setValue(false);
    };
  }

  get canSendMessage() {
    return this.message && this.message !== '';
  }

  
  onSendMessage() {
    this.messageSocket(this.message)
  }


  messageSocket(message) {

    this.messageError = null;
    this.messageLoading = true;

    const url = `wss://sockets.customers.soficomit.com/Message?branch=${this.auth.currentUser.branchId}`;

    let socket = new WebSocket(url);

    socket.onopen = (e) => {
      socket.send(JSON.stringify(message));
      socket.close();
      this.messageLoading = false;
    };

    socket.onmessage = (event) => {
      //console.log("message");
    };

    socket.onclose = (event) => {
      //console.log("closed");
    };

    socket.onerror = (event) => {
      //console.log("error")
      this.messageError = "Echec de connexion";
      this.messageLoading = false;
    };
  }

  clearMessage() {
    this.message = undefined;
    let message = {
      message: ""
    };
    this.messageSocket(message)
  }


}
