import { api } from "./axiosInstance";
import { ApiResponse, AuthResponse } from '../types/common.types';

export const login = async(email:string,password:string) : Promise<ApiResponse> =>{
    const response = await api.post<ApiResponse<AuthResponse>>('/Auth/login', {email,password});
    return response.data.data;
};

export const register = async (name:string, email:string,password:string): Promise<ApiResponse> =>{
    const response = await api.post<ApiResponse<AuthResponse>>('/Auth/register',{name,email,password});
    return response.data.data;
};

export const logout = async (refreshToken :string): Promise<void> =>{
    await api.post('/Auth/logout',{refreshToken});
};