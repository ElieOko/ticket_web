
export interface IManageResponse {
  userId: number;
  branch: string;
  userProfile: IUserProfileDto;
}

export interface IUserProfileDto {
  userName: string;
  fullName: string;
}
