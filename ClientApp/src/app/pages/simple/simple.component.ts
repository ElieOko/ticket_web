import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiService } from '../../core/api.service';
import { MiscModel } from '../../core/models/misc';

@Component({
  selector: 'app-simple',
  templateUrl: './simple.component.html',
  styleUrls: ['./simple.component.scss']
})
export class SimpleComponent implements OnInit {

  simpleForm: FormGroup;

  submitting = false;
  error = null;
  success = null;
  misc = new MiscModel();
  transferStatuses = [];

  constructor(private fb: FormBuilder, private api: ApiService) { }

  ngOnInit(): void {
    this.simpleForm = this.fb.group({
      orderNumber: [null, Validators.required],
      transferStatusId: [3, Validators.required],
      completeNote:[''],
      completeAndPay:[false]
    });

    this.loadMisc();
  }

  

  onCompleteAndPay() {
    this.simpleForm.get('completeAndPay').setValue(true);
  }

  loadMisc() {
    this.api.getMisc().subscribe(
      res => {
        this.misc = res
        this.transferStatuses = this.misc.transferStatuses.filter(x => x.transferStatusId != 5)
      },
      error => console.log(error)
    );
  }

  get orderNumber() {
    return this.simpleForm.get('orderNumber');
  }

  get transferStatusId() {
    return this.simpleForm.get('transferStatusId')
  }

  onSubmit() {
    this.error = null;
    this.success = null;
    this.submitting = true;
    const data = this.simpleForm.value;

    this.api.updateTransferStatus(data).subscribe(
      () => {
        this.success = "Etat du ticket mis à jour avec succès";
        this.simpleForm.reset({ orderNumber: null, transferStatusId: 3, completeNote: '', completeAndPay: false });
        this.submitting = false;
        setTimeout(() => { this.success = null }, 3000)
      },
      error => {
        this.simpleForm.get('completeAndPay').setValue(false);
        this.error = `Echec: ${error}`;
        this.submitting = false;
        //console.log(error)
      }
    );
  }

}
