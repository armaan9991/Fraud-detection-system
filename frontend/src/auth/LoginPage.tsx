import { useNavigate } from "react-router-dom";
import { useAuthStore } from "../store/authstore";
import { useState } from "react";
import z from "zod";
import {login} from '../api/authApi';

const schema = z.object({
    email: z.string().email('enter valid email!'),
    password : z.string().min(1,'Password is Required'),
});
type LoginForm = z.infer<typeof schema>;

export default function LoginPage(){
    const naviagte = useNavigate();
    const setAuth = useAuthStore((state) =>state.setAuth);
    const {serverError,setServerError} = useState<string | null>(null);

    const onSubmit = async (data :LoginForm) => {
        setServerError(null);
        try{
            const result = await login(data.email,data.password)
            setAuth(result.accessToken, result.refreshToken, result.email, result.role);
            naviagte('/dashboard');
        }
        catch{
            setServerError('Invalid Email or password');
        }
    }
}