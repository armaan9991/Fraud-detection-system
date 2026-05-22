import { Navigate, Outlet } from "react-router-dom";
import { useAuthStore } from "../store/authstore";

export default function ProtectedRoute(){
    const isAuthenticated = useAuthStore((state) => state.isAuthenticated);
    return isAuthenticated ? <Outlet />    // if it is authenticated then show this page else return to login .
    : <Navigate to = "/login" replace />;
}