import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MiscModel, ICounterModel } from './models/misc';
import { UserListModel, UserDto } from './models/user';
import { ITransferDto, TransferListModel, TransferCallDto, TransferCreated } from './models/transfer';
import { map, shareReplay } from 'rxjs/operators';
import { zip } from 'rxjs';

const misc_baseUrl = "api/misc";
const users_baseUrl = "api/users";
const transfers_baseUrl = "api/transfers";
const counters_baseUrl = "api/counters";

const transferIndex = (item: any, data: any[]): number => {
  for (let idx = 0; idx < data.length; idx++) {
    if (data[idx].transferId === item.transferId) {
      return idx;
    }
  }

  return -1;
};

const cloneData = (data: any[]) => data.map(item => Object.assign({}, item));

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  public transferLoading = false;
  public transfers: ITransferDto[] = [];
  private updatedTransfers: ITransferDto[] = [];
  private originalTransfers: any[] = [];

  constructor(private http: HttpClient) { }

  getMisc(zone?) {
    let url = misc_baseUrl;
    if (zone !== null && zone !== undefined) {
      url += `?zone=${zone}`;
    }
    return this.http.get<MiscModel>(misc_baseUrl)
  }

  

  getTransfers(startDate, endDate, type?, status?) {
    let url = `${transfers_baseUrl}?startDate=${startDate}&endDate=${endDate}T23:59:59`;
    if (type !== null) {
      url += `&transferType=${type}`;
    }
    if (status !== null) {
      url += `&status=${status}`;
    }
    return this.http.get<TransferListModel>(url).pipe(
      map(res => {
        if (res) {
          this.transfers = res.transfers;
          this.originalTransfers = cloneData(this.transfers);
        }
      })
    )
  }

  fakeRead(data) {
    this.transfers = data;
    this.originalTransfers = cloneData(this.transfers);
  }

  public update(item: any): void {
    if (!this.isNew(item)) {
      const index = transferIndex(item, this.updatedTransfers);
      if (index !== -1) {
        this.updatedTransfers.splice(index, 1, item);
      } else {
        this.updatedTransfers.push(item);
      }
    }
  }

  public isNew(item: any): boolean {
    return !item.transferId;
  }

  public hasChanges(): boolean {
    return Boolean(this.updatedTransfers.length);
  }

  public saveChanges(startDate, endDate, type?, status?): void {
    if (!this.hasChanges()) {
      return;
    }

    const completed = [];

    if (this.updatedTransfers.length) {
      completed.push(this.updateTransfers(this.updatedTransfers));
    }


    this.reset();

    zip(...completed).subscribe(() => this.getTransfers(startDate, endDate, type, status).subscribe());
  }

  public cancelChanges(): void {
    this.reset();

    this.transfers = this.originalTransfers;
    this.originalTransfers = cloneData(this.originalTransfers);
  }

  private reset() {
    this.transfers = [];
    this.updatedTransfers = [];
  }

  updateTransfers(transfers) {
    return this.http.put(transfers_baseUrl, transfers);
  }

  updateTransferStatus(transfer) {
    const url = `${transfers_baseUrl}/status`;
    return this.http.put(url, transfer)
  }

  createTransferCall(data) {
    const url = `${transfers_baseUrl}/call`;
    return this.http.post<TransferCallDto>(url, data)
  }

  createSimpleTransfer(data) {
    const url = `${transfers_baseUrl}/create`;
    return this.http.post<TransferCreated>(url, data)
  }

  updateSimpleTransfer(data) {
    const url = `${transfers_baseUrl}/update`;
    return this.http.put<TransferCreated>(url, data);
  }

  markTransferAsPaid(data) {
    const url = `${transfers_baseUrl}/pay`;
    return this.http.put(url, data);
  }

  markTransferAsProccessed(data) {
    const url = `${transfers_baseUrl}/complete`;
    return this.http.put(url, data)
  }

  createCounter(data) {
    return this.http.post(counters_baseUrl, data);
  }

  getCounters() {
    return this.http.get<ICounterModel[]>(counters_baseUrl);
  }

  updateCounter(data) {
    return this.http.put(counters_baseUrl, data)
  }

  deleteCounter(counterId) {
    const url = `${counters_baseUrl}/${counterId}`;
    return this.http.delete(url);
  }

}
