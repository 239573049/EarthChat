export interface GetUserDto {
    id: string;
    account: string;
    avatar: string;
    name: string;
    location: string;
}

export interface ChatGroupDto {
    id: string;
    name: string;
    avatar: string;
    description: string;
    group: boolean;
    default: boolean;
    lastMessage: string;
    creator:string;
    creationTime: Date;
    type:string;   
}

export interface CreateGroupDto {
    name: string;
    avatar: string;
    description: string;
}

export interface FriendRegistrationInput {
    description: string;
    beAppliedForId: string;
}