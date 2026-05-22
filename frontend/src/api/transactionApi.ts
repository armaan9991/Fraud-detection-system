import { api } from "./axiosInstance";
import type{ ApiResponse,AuthResponse,PagedResult,Transaction } from "../types/common.types";
import type { TreemapNode } from "recharts";

export const getTransaction = async (page =1,pageSize =20):Promise<PagedResult<Transaction>> =>{
    const response = await api.get<ApiResponse<PagedResult<Transaction>>>(
        `/Transaction?page=${page}&pageSize=${pageSize}`
    );
    return response.data.data;
};

export const createTransaction = async(amount:number,currency :string , country:string):
Promise<Transaction> => {
    const resp = await api.post<ApiResponse<Transaction>>('Transaction',{amount,currency,country});
    return resp.data.data;
}
