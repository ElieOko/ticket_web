import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { filter, switchMap } from 'rxjs/operators';
import { ApiService } from '../core/api.service';
import { ICounterModel } from '../core/models/misc';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {

  counterSelectionAction = new Subject<number>();
  counterSelectionAction$ = this.counterSelectionAction.asObservable();


  selectedCounter: ICounterModel = null;

  loading = false;

  counterForm: FormGroup;
  counterTitle = "Details pour le guichet:"

  counters = [];

  newName: string;
  adding = false;

  editEnabled = false;
  editName: string;
  savingChanges = false;

  constructor(public api: ApiService) { }

  ngOnInit(): void {
    this.loadCounters();

    this.counterSelectionAction$.subscribe(selectedId => {
      if (!this.adding) {
        const counter = this.counters.find(c => c.counterId === selectedId);
        this.counterTitle = `Details pour le guichet: ${counter.name}`;
        this.selectedCounter = counter;
        this.editName = this.selectedCounter.name;
      }
      
    });

  }

  addCounter() {
    this.adding = true;
  }

  cancelAdding() {
    this.adding = false;
  }

  onSelectCounter(counterId) {
    this.counterSelectionAction.next(counterId);
  }

  get canAdd() {
    return this.newName !== '' && this.newName !== undefined && this.newName !== null;
  }

  saveNewCounter() {
    const counter = { name: this.newName };
    this.api.createCounter(counter).subscribe(
      () => { this.loadCounters(); this.newName = null; this.adding = false; },
      error => { console.log(error) }
    )
  }


  loadCounters() {
    this.loading = true;

    this.api.getCounters().subscribe(
      res => {
        this.counters = res;
        this.loading = false;
      },
      error => {
        this.loading = false;
      }
    )
  }


  


  save() {
    this.selectedCounter.name = this.editName;
    this.savingChanges = true;
    this.api.updateCounter(this.selectedCounter).subscribe(
      () => {
        this.loadCounters();
        this.unsetSelectedCounter();
      },
      error => { console.log(error); this.savingChanges = false; }
    )
  }

  cancel() {
    this.disableEdit();
  }

  delete() {
    const deleteConfirmed = window.confirm("Voulez-vous vraiment supprimer ce guichet ?");
    if (deleteConfirmed) {
      this.savingChanges = true;
      this.api.deleteCounter(this.selectedCounter.counterId).subscribe(
        () => {
          this.savingChanges = false;
          this.loadCounters();
          this.unsetSelectedCounter();
        },
        error => { console.log(error); this.savingChanges = false; }
      )
    }
  }

  unsetSelectedCounter() {
    this.selectedCounter = null;
    this.editEnabled = false;
    this.editName = null;
    this.counterTitle = "Details pour le guichet:";
  }

  enableEdit() {
    this.editName = this.selectedCounter.name;
    this.editEnabled = true;
  }

  get hasChanged() {
    return this.editName !== this.selectedCounter.name;
  }

  disableEdit() {
    this.editEnabled = false;
  }

}
