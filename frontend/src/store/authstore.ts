import { email, set } from 'zod';
import {create} from 'zustand';
import { persist } from 'zustand/middleware';

interface AuthState{
    accessToken : string | null;
    refreshToken : string | null;
    email :string | null;
    role :string | null;
    isAuthenticated : boolean;
    setAuth :(accessToken : string, refrehToken:string,email:string,role:string) => void;
    setAccessToken : (token:string) => void;
    logout : () => void;
}
export const useAuthStore = create<AuthState>()(
    persist(
        (set) => ({
            accessToken : null,
            refreshToken: null,
            email : null,
            role : null,
            isAuthenticated : false,
            setAuth:(accessToken,refreshToken,email,role) => set({accessToken,refreshToken,email,role,isAuthenticated:true}),
            setAccessToken: (token) => set({accessToken : token}),
            logout: () => set({accessToken:null,refreshToken:null, email:null,role:null,isAuthenticated:false}),
            }),
        {name: ' auth-storage'}
    )
);

