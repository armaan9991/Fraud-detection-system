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
