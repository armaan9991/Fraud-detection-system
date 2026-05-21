import axois from 'axios';           // to call  http req.
import { useAuthStore } from '../store/authstore';
import { config } from 'zod/v4/core';

const api = axois.create({
    baseURL :'http://localhost:5297/api',
});

api.interceptors.request.use((config) =>{
    const token = useAuthStore.getState().accessToken;
    if(token){
        config.headers.Authorization= `Bearer ${token}`;
    }
    return config;
});