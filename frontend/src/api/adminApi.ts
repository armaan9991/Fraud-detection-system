import { api } from "./axiosInstance";
import type { ApiResponse,AdminStats } from "../types/common.types";

export const getAdminStats = async() :Promise<AdminStats> =>{
    const resp = await api.get<ApiResponse<AdminStats>>('/Admin/stats');
    return resp.data.data;
}