import { IBranchModel } from './misc';

export class UserDto {
  userId?: number;
  userName: string;
  password?: string;
  fullName: string;
  accessLevel: number;
  locked: boolean;
  minAmount?: number;
  maxAmount?: number;
  branch: IBranchModel;
  permission: AssignedPermission[];
  loading?: boolean;
}

export interface AssignedPermission {
  transferTypeId: number;
  transferTypeName: string;
  assigned: boolean;
}


export class UserListModel {
  users: UserDto[];
}
