import { api } from "./axiosInstance";
import type{ ApiResponse,PagedResult,Transaction } from "../types/common.types";

export const getTransactions = async (page =1,pageSize =20):Promise<PagedResult<Transaction>> =>{   // means it will return this promise
    const response = await api.get<ApiResponse<PagedResult<Transaction>>>(   //send  get request 
        `/Transactions?page=${page}&pageSize=${pageSize}`   // this template literal.
    );
    return response.data.data;
};

export const createTransaction = async(amount:number,currency :string , country:string):
Promise<Transaction> => {
    const resp = await api.post<ApiResponse<Transaction>>('/Transactions',{amount,currency,country});
    return resp.data.data;
}


