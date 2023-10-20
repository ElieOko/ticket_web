import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MiscModel } from '../../core/models/misc';
import { ApiService } from '../../core/api.service';
import { TransferSimpleCreate, TransferCreated } from '../../core/models/transfer';

@Component({
  selector: 'app-open-ticket',
  templateUrl: './open-ticket.component.html',
  styleUrls: ['./open-ticket.component.scss']
})
export class OpenTicketComponent implements OnInit {

  ticketForm: FormGroup;
  printMode = false;
  error = null;
  submitting = false;
  misc = new MiscModel();
  success = null;
  transfer: TransferCreated = null;

  constructor(private api: ApiService, private fb: FormBuilder) { }

  ngOnInit(): void {
    this.ticketForm = this.fb.group({
      amount: [null, Validators.required],
      currencyId: [null, Validators.required],
      phone: [null, Validators.compose([Validators.required,
        Validators.pattern("^((\\+243-?)|0)?[0-9]{9}$")])],
      transferTypeId: [null, Validators.required],
      note:['']
    });

    this.loadMisc();
  }

  get amount() {
    return this.ticketForm.get('amount');
  }

  get currencyId() {
    return this.ticketForm.get('currencyId');
  }

  get phone() {
    return this.ticketForm.get('phone');
  }

  get transferTypeId() {
    return this.ticketForm.get('transferTypeId');
  }


  loadMisc() {
    this.api.getMisc().subscribe(
      res => {
        this.misc = res
      },
      error => {
        console.log(error)
      }
    )
  }

  onSubmit() {
    this.submitting = true;

    let data: TransferSimpleCreate = this.ticketForm.value;

    if (this.transfer == null) {
      this.api.createSimpleTransfer(data).subscribe(
        res => {
          this.submitting = false;
          this.transfer = res;
          this.printMode = true;
        },
        error => {
          this.error = error
          this.submitting = false;
        }
      )
    } else {

      data.transferId = this.transfer.transferId;

      this.api.updateSimpleTransfer(data).subscribe(
        res => {
          this.submitting = false;
          this.transfer = res;
          this.printMode = true;
        },
        error => {
          this.error = error
          this.submitting = false;
        }
      )
    }
    
  }

  onNew() {
    this.transfer = null;
    this.ticketForm.reset();
    this.printMode = false;
    this.success = null;
  }

  onPrint() {
    window.print();
  }

  onGoBack() {
    this.printMode = false;
    this.success = null;
  }

  onMarkAsPaid() {
    const data = {
      transferId: this.transfer.transferId
    }

    this.api.markTransferAsPaid(data).subscribe(
      () => {
        this.success = "Opération reussie";
      },
      error => {
        console.log(error)
      }
    )
  }

}
