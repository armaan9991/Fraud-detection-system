import type { ApiResponse, AuthResponse } from "../types/common.types";
import { api } from "./axiosInstance";

export const CreateUserAsync = async (name: string, email: string, password: string): Promise<AuthResponse> => {
  const resp = await api.post<ApiResponse<AuthResponse>>('/Auth/register', { name, email, password });
  return resp.data.data;
}