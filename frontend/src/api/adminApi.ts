import { api } from "./axiosInstance";
import type { ApiResponse,AdminStats, PagedResult,User } from "../types/common.types";

export const getAdminStats = async() :Promise<AdminStats> =>{
    const resp = await api.get<ApiResponse<AdminStats>>('/Admin/stats');
    return resp.data.data;
}
export const getAdminUser = async(page:number, pagesize:number) : Promise<PagedResult<User>> =>{
    const resp = await api.get<ApiResponse<PagedResult<User>>>('/Admin/users',{params:{page,pagesize}});
    return resp.data.data;
}
