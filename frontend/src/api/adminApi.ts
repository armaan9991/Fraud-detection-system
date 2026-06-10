import { api } from "./axiosInstance";
import type { ApiResponse,AdminStats, PagedResult,User} from "../types/common.types";

export const getAdminStats = async() :Promise<AdminStats> =>{
    const resp = await api.get<ApiResponse<AdminStats>>('/Admin/stats');
    return resp.data.data;
}
export const getAdminUsers = async(page:number, pageSize:number) : Promise<PagedResult<User>> =>{
    const resp = await api.get<ApiResponse<PagedResult<User>>>('/Admin/users',{params:{page,pageSize}});
    return resp.data.data;
}
export const unflagUser = async(userId:number) : Promise<User | null> =>{
    try{
    const resp = await api.patch<ApiResponse<User>>(`/Admin/users/${userId}/unflag`);
    return resp.data.data;
    }
    catch (err:any){
        if(err.response?.status === 404){
            return null;
        }
        throw err;
    }
}

