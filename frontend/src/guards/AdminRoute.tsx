import { Navigate, Outlet } from "react-router-dom";
import { useAuthStore } from "../store/authstore";

export default function AdminRoute(){
    const role = useAuthStore((state) => state.role);
    return role?.toLowerCase() === 'admin' ? <Outlet/> : <Navigate to = '/dashboard' replace />;
}