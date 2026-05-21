export interface ApiResponse<T>{
    success:boolean;
    message: string;
    data : T;
}
export interface PagedResult<T>{
    items : T[];
    page : number;
    pageSize : number;
    totalRecords : number;
    totalPages: number;
}
export interface AuthResponse{
    accessToken: string;
    refrehToken :string;
    email : string;
    role: string;
}

export interface Transaction{
    transactionId: number;
    userId : number;
    amount : number;
    currency : string;
    country : string;
    fraudScore: number;
    transactionTime : string;
}

export interface Device{
    deviceId : number;
    userId: number;
    deviceName: string;
    ipAddress: string;
    lastUsed:string;
}
export interface FraudAlert{
    fraudAlertId: number;
    transactionId: number;
    riskLevel: string;
    reason:string;
    createdAt: string;
}

export interface User{
    userId:number;
    name:string;
    email:string;
    role:string;
    createdAt:string;
    isFlagged:boolean;
    flagReason?:string;
}