import { Component, OnInit, AfterViewChecked } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { ApiService } from '../../core/api.service';
import { DateFormatter } from '../../core/date-formatter';
import { MiscModel } from '../../core/models/misc';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CompleteTransferComponent } from '../../components/complete-transfer/complete-transfer.component';

@Component({
  selector: 'app-ticket-list',
  templateUrl: './ticket-list.component.html',
  styleUrls: ['./ticket-list.component.scss']
})
export class TicketListComponent implements OnInit{

  enableFilter = false;
  advanced = false;

  loading = false;

  transferForm: FormGroup;

  misc = new MiscModel();
  startDate = new FormControl(new Date());
  endDate = new FormControl(new Date());
  type = new FormControl();
  status = new FormControl();

  transferStatuses = [];

  constructor(public api: ApiService, public dialog: MatDialog) { }

  ngOnInit(): void {
    this.api.transfers = []
    this.loadMisc();
  }

  openDialog(dataItem): void {
    const dialogRef = this.dialog.open(CompleteTransferComponent, {
      width: '300px',
      hasBackdrop: true,
      disableClose: true
      //data: { name: this.name, animal: this.animal }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result.completeNote != undefined) {
        const data = {
          transferId: dataItem.transferId,
          completeNote: result.completeNote
        };

        this.api.markTransferAsProccessed(data).subscribe(
          () => { this.onLoadTransfers() },
          error => { console.error(error) }
        )
      }
    });
  }

  loadMisc() {
    this.api.getMisc().subscribe(
      res => {
        this.misc = res;
        this.transferStatuses = this.misc.transferStatuses.filter(x => x.transferStatusId != 5)
      },
      error => { console.log(error) }
    )
  }

  toggleFilter() {
    this.enableFilter = !this.enableFilter;
  }

  toggleAdvanced() {
    this.advanced = !this.advanced;
  }

  onLoadTransfers() {
    //this.loadTransfers();
    const startDate = DateFormatter.shortFormat(this.startDate.value);
    const endDate = DateFormatter.shortFormat(this.endDate.value);
    const type = this.type.value;
    const status = this.status.value;

    this.loading = true;

    this.api.getTransfers(startDate, endDate, type, status).subscribe(
      () => { this.loading = false },
      error => {
        this.loading = false;
        console.log(error)
      }
    )
  }

  public cellClickHandler({ sender, rowIndex, columnIndex, dataItem, isEdited }) {
    if (!isEdited) {
      this.transferForm = this.createFormGroup(dataItem);
      sender.editCell(rowIndex, columnIndex, this.transferForm);
    }
  }

  public cellCloseHandler(args: any) {
    const { formGroup, dataItem } = args;

    if (!formGroup.valid) {
      // prevent closing the edited cell if there are invalid values.
      args.preventDefault();
    } else if (formGroup.dirty) {
      Object.assign(dataItem, formGroup.value)
      //this.editService.assignValues(dataItem, formGroup.value);
      this.api.update(dataItem);
    }
  }

  public cancelHandler({ sender, rowIndex }) {
    sender.closeRow(rowIndex);
  }

  public saveChanges(grid: any): void {
    grid.closeCell();
    grid.cancelCell();

    const startDate = DateFormatter.shortFormat(this.startDate.value);
    const endDate = DateFormatter.shortFormat(this.endDate.value);
    const type = this.type.value;
    const status = this.status.value;

    this.api.saveChanges(startDate, endDate, type, status);
  }

  public cancelChanges(grid: any): void {
    grid.cancelCell();

    this.api.cancelChanges();
  }

  public createFormGroup(dataItem: any): FormGroup {
    return new FormGroup({
      transferId: new FormControl(dataItem.transferId),
      transferTypeId: new FormControl(dataItem.transferTypeId),
      amount: new FormControl(dataItem.amount),
      name: new FormControl(dataItem.name),
      phone: new FormControl(dataItem.phone),
      transferStatus: new FormControl(dataItem.transferStatus),
      completeNote: new FormControl(dataItem.completeNote)
    });
  }


}
