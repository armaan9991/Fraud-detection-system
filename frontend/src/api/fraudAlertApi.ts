import { api } from "./axiosInstance";
import type { ApiResponse,FraudAlert,PagedResult} from "../types/common.types";

export const getFraudAlerts = async(page =1, pageSize =20):Promise<PagedResult<FraudAlert>> => {
    const response = await api.get<ApiResponse<PagedResult<FraudAlert>>>(
        `/FraudAlert?page=${page}&pageSize=${pageSize}`        
    );
    return response.data.data;
}