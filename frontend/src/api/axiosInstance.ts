import axois from 'axios';           // to call  http req.
import { useAuthStore } from '../store/authstore';

export const api = axois.create({
    baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5297/api',
});

api.interceptors.request.use((config) =>{
    const token = useAuthStore.getState().accessToken;
    if(token){
        config.headers.Authorization= `Bearer ${token}`;
    }
    return config;
});

api.interceptors.response.use(                // here response has 2 values (successHandler , errorHandler)
    (response) => response,
    async (error) =>{
        const original = error.config;
        if(error.response?.status == 401 && !original._retry){            //if 401 error try to refresh and check if we had tried earlier or not.
            original._retry = true;
            const refreshToken = useAuthStore.getState().refreshToken;       // get refresh token
            if (refreshToken){                                               // if refresh token is actually present
                try{    
                    const {data} = await axois.post('http://localhost:5297/api/Auth/refresh', {refreshToken});        // try to refresh  
                    const newToken = data.data.accessToken;                                       // get accesstoken
                    useAuthStore.getState().setAccessToken(newToken);                               // set new access token
                    original.headers.Authorization = `Bearer ${newToken}`;                          // new jwt token
                    return api(original);
                }catch{
                    useAuthStore.getState().logout();
                    window.location.href='/login';
                }
            }
            else{
                useAuthStore.getState().logout();
                window.location.href = '/login';
            }
        }
        return Promise.reject(error);
    }
)