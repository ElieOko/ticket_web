import { Component, OnInit, Inject } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormGroup, FormBuilder } from '@angular/forms';
@Component({
  selector: 'app-complete-transfer',
  templateUrl: './complete-transfer.component.html',
  styleUrls: ['./complete-transfer.component.scss']
})
export class CompleteTransferComponent implements OnInit {

  completeForm: FormGroup;

  constructor(public dialogRef: MatDialogRef<CompleteTransferComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private fb: FormBuilder) { }

  ngOnInit(): void {
    this.completeForm = this.fb.group({
      completeNote:['']
    })
  }

  onCloseDialog() {
    this.dialogRef.close();
  }

  submit() {
    this.dialogRef.close(this.completeForm.value);
  }

}
