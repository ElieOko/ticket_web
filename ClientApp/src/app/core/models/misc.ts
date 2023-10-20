
export interface IBranchModel {
  branchId: number;
  branchName: string;
  branchZone: string;
}

export interface ITransferTypeModel {
  transferTypeId: number;
  name: string;
  displayName: string;
}

export interface IStatusModel {
  transferStatusId: number;
  name: string;
}
export interface ICurrencyModel {
  currencyId: number;
  currencyName: string;
  currencyCode: string;
}

export interface ICounterModel {
  counterId: number;
  name: string;
  branchId: number;
}

export class MiscModel {
  branches: IBranchModel[] = [];
  transferTypes: ITransferTypeModel[] = [];
  transferStatuses: IStatusModel[] = [];
  currencies: ICurrencyModel[] = [];
  counters: ICounterModel[] = [];
}
