


export interface ITransferDto {
  transferId: number;
  orderNumber: number;
  transferTypeId: number;
  transferType: string;
  amount: number;
  currency: string;
  name: string;
  phone: string;
  createdTime: string;
  processedTime: string;
  durationS: number;
  paidTime: string;
  durationP: number;
  tranferStatusId: number;
  transferStatus: string;
  completeNote: string;
  completed?: boolean;
  userS: string;
  userP: string;
  imagePath?: string;
  signature?: string;
}

export class TransferListModel {
  transfers: ITransferDto[] = []
}


export interface TransferCallDto {
  token: number;
  time: Date;
}

export interface TransferSimpleCreate {
  transferId?: number;
  amount: number;
  currencyId: number;
  transferTypeId: number;
  note: string;
  phone: string;
}

export interface TransferCreated {
  transferId: number;
  transferTypeId: number;
  transferTypeName: string;
  fromBranchId?: number;
  fromBranchName?: string;
  toBranchId?: number;
  toBranchName?: string;
  amount?: number;
  currencyId: number;
  currencyCode: string;
  senderName: string;
  senderPhone: string;
  receiverName: string;
  receiverPhone: string;
  address: string;
  note: string;
  code: string;
  orderNumber: string;
  barcode: string;
  intervalId?: number;
  cardId?: number;
  cardName: string;
  cardExpiryDate?: Date;
  dateCreated?: Date;
}
