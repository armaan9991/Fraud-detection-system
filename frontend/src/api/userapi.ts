import type { ApiResponse,User } from "../types/common.types";
import { api } from "./axiosInstance";
export const CreateUserAsync =async(name:string, email:string,role:string,password:string):
 Promise<User> => {
    const resp = await api.post<ApiResponse<User>>('User',{name,email,role,password});
    return resp.data.data;
 }