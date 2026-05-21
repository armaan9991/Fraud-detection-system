import { Navigate, Outlet } from "react-router-dom";
import { useAuthStore } from "../store/authstore";

export default function AdminRoute(){
    const role = useAuthStore((state) => state.role);
    return role == 'Admin' ? <Outlet/> : <Navigate to = '/dashboard' replace />;
}