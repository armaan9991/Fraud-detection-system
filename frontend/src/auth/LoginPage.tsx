import { useNavigate } from "react-router-dom";
import { useAuthStore } from "../store/authstore";
import { useState } from "react";
import z from "zod";
import type { schema } from "@hookform/resolvers/ajv/src/__tests__/__fixtures__/data.js";

const schema = z.object({
    email: z.string().email('enter valid email!'),
    password : z.string().min(1,'Password is Required'),
});
type LoginForm = z.infer<typeof schema>;

export default function LoginPage(){
    const naviagte = useNavigate();
    const setAuth = useAuthStore((state) =>state.setAuth);
    const {serverError,setServerError} = useState<string | null>(null);

}