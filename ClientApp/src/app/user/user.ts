

export interface IBranchModel {
  branchId: number;
  branchName: string;
  branchZone: string;
}


export class UserDto {
  userId?: number;
  userName: string;
  password?: string;
  fullName?: string;
  accessLevel: number;
  type: string;
  locked: boolean;
  minAmount?: number;
  maxAmount?: number;
  branch?: IBranchModel;
  branchId?: number;
  permissions?: number[];
  loading?: boolean;
}



export class UserListModel {
  users: UserDto[];
}

export class UserData {
  data: UserDto[];
  query: string;
  totalCounts: number;
  totalPages: number;
  currentPage: number;
  pageSize: number;
  hasPrevious: boolean;
  hasNext: boolean;
}


export class PaginationLink {
  key: number;
  url: string;
  text: string;
}


export class UserForm {
  constructor(
    public userId?: number,
    public userName?: string,
    public password?: string,
  ) { }
}
